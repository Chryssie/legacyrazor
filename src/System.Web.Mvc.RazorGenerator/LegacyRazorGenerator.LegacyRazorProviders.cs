using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Text;
using System.Threading;
using System.Web.Mvc.Razor;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.WebPages.Razor.Configuration;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace System.Web.Mvc;

partial class LegacyRazorGenerator
{
	private static (LegacyRazorSourceGenerationOptions Options, ImmutableArray<Diagnostic> Diagnostics) ComputeLegacyRazorSourceGeneratorOptions((AnalyzerConfigOptionsProvider, ParseOptions) value, CancellationToken cancellationToken = default)
	{
		var (optionsProvider, parseOptions) = value;
		var globalOptions = optionsProvider.GlobalOptions;

		_ = globalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);

		return (
			Options: new LegacyRazorSourceGenerationOptions
			{
				RootNamespace = rootNamespace ?? "ASP",
			},
			Diagnostics: []
		);
	}

	private static (GeneratorSourceFile?, Diagnostic?) ComputeGeneratorSourceFile((AdditionalText, AnalyzerConfigOptionsProvider) value, CancellationToken cancellationToken = default)
	{
		var (additionalText, optionsProvider) = value;

		var options = optionsProvider.GetOptions(additionalText);

		var relativePath = default(string?);
		var hasTargetPath = options.TryGetValue("build_metadata.AdditionalFiles.TargetPath", out var encodedRelativePath);

		if (hasTargetPath && !string.IsNullOrWhiteSpace(encodedRelativePath))
		{
			relativePath = Encoding.UTF8.GetString(Convert.FromBase64String(encodedRelativePath));
		}
		else if (optionsProvider.GlobalOptions.TryGetValue("build_property.projectdir", out var projectPath)
			&& projectPath is { Length: not 0, }
			&& additionalText.Path.StartsWith(projectPath, StringComparison.OrdinalIgnoreCase))
		{
			// Fallback, when TargetPath isn't specified but we know about the project directory, we can do our own calulation of
			// the project relative path, and use that as the target path. This is an easy way for a project that isn't using the
			// Razor SDK to still get TargetPath functionality without the complexity of specifying metadata on every item.
			relativePath = additionalText.Path.Substring(projectPath.Length).TrimStart(['/', '\\']);
		}
		else if (!hasTargetPath)
		{
			// If the TargetPath is not provided, it could be a Misc Files situation, or just a project that isn't using the
			// Web or Razor SDK. In this case, we just use the physical path.
			relativePath = additionalText.Path;
		}

		if (relativePath is null)
			return (null, LegacyRazorDiagnostics.TargetPathNotProvided(additionalText.Path));

		return (additionalText.ToGeneratorSourceFile(relativePath), null);
	}

	private static ((AdditionalText, RazorContext)?, Diagnostic?) ComputeProjectItems(GeneratorSourceFile source, CancellationToken cancellationToken = default)
	{
		var razorContext = new RazorContext(source.RelativePath, source.Path)
		{
			DesignTimeMode = false,
		};
		return ((source, razorContext), null);
	}

	private static ((Block, RazorContext)?, ImmutableArray<Diagnostic>) ParseDocument((AdditionalText, RazorContext) item, CancellationToken cancellationToken = default)
	{
		var (additionalText, razorContext) = item;

		if (additionalText.GetText(cancellationToken) is not { } text)
			return (null, []);

		var parser = razorContext.CreateRazorParser();

		var result = parser.Parse(text.ToTextDocument());

		var errors = result.ParserErrors;

		ImmutableArray<Diagnostic> diagnostics = [];

		if (errors is { Count: var count and not 0, })
		{
			var diagnosticsBuilder = ImmutableArray.CreateBuilder<Diagnostic>(count);

			foreach (var error in errors)
			{
				diagnosticsBuilder.Add(LegacyRazorDiagnostics.ParserError(additionalText.Path, text.Lines, error));
			}

			diagnostics = diagnosticsBuilder.DrainToImmutable();
		}

		if (diagnostics.IsEmpty && !result.Success)
		{
			diagnostics = [LegacyRazorDiagnostics.ParserFailure(additionalText.Path)];
		}

		if (result.Document is { } document)
			return ((document, razorContext), diagnostics);

		return (null, diagnostics);
	}

	private static ((string HintName, string SourceText)?, Diagnostic?) GenerateCode(((Block, RazorContext), LegacyRazorSourceGenerationOptions) value, CancellationToken cancellationToken = default)
	{
		var ((document, razorContext), options) = value;

		var result = razorContext.GenerateCode(document, rootNamespace: options.RootNamespace, cancellationToken:cancellationToken);

		result.GeneratedClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Runtime.CompilerServices.CompilerGenerated"));
		result.GeneratedClass.CustomAttributes.Add(new CodeAttributeDeclaration("System.Web.WebPages.PageVirtualPathAttribute", new CodeAttributeArgument(new CodePrimitiveExpression(ComputeAppRelativePath(razorContext.VirtualPath)))));

		if (result.CompileUnit is null)
			return (null, null);

		var codeDomProvider = razorContext.CodeDomProvider;

		var sourceWriter = new StringWriter();

		codeDomProvider.GenerateCodeFromCompileUnit(result.CompileUnit, sourceWriter, new CodeGeneratorOptions()
		{
			IndentString = "\t",
			VerbatimOrder = true,
			BlankLinesBetweenMembers = false,
			BracingStyle = "C",
			ElseOnClosing = false,
		});

		var hintName = $"{razorContext.VirtualPath}.g.cs";
		var source = sourceWriter.ToString();

		return ((hintName, source), null);
	}

	private static string ComputeAppRelativePath(string path)
	{
		const string Prefix = "~/";

		if (path is not null)
		{
			for (var i = 0; i < path.Length; i++)
			{
				if (path[i] is '\\' or '/')
					continue;

				var sb = new StringBuilder(Prefix.Length + (path.Length - i)).Append(Prefix.Length).Append(path, i, path.Length - i);

				sb.Replace('\\', '/', Prefix.Length, sb.Length - 2);

				return sb.ToString();
			}
		}

		return Prefix;
	}
}
