using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Web.Mvc.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace System.Web.Mvc;

public record class RazorContext
{
	public RazorContext()
	{

	}

	[SetsRequiredMembers]
	public RazorContext(string VirtualPath, string PhysicalPath)
	{
		this.VirtualPath = VirtualPath;
		this.PhysicalPath = PhysicalPath;
	}

	public required string VirtualPath
	{
		get;
		init
		{
			ArgumentNullException.ThrowIfNull(value);

			if (field != value)
			{
				field = value;
				this.Host = null;
				this.AppRelativePath = $"~/{value.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Replace('\\', '/')}";
			}
		}
	}
	public string AppRelativePath { get; private init; }
	public required string PhysicalPath
	{
		get;
		init
		{
			ArgumentNullException.ThrowIfNull(value);

			if (field != value)
			{
				field = value;
				this.Host = null;
			}
		}
	}
	public bool DesignTimeMode
	{
		get;
		init
		{
			if (value != field)
			{
				field = value;
				this.Host = null;
			}
		}
	}
	public bool GenerateLinePragmas
	{
		get;
		init;
	} = true;

	[AllowNull]
	private MvcWebPageRazorHost Host
	{
		get
		{
			return Volatile.Read(ref field) ?? InitHost();

			MvcWebPageRazorHost InitHost()
			{
				var newHost = new MvcWebPageRazorHost(this.VirtualPath, this.PhysicalPath)
				{
					DesignTimeMode = this.DesignTimeMode,
				};

				var host = Interlocked.CompareExchange(ref field, comparand: null, value: newHost);

				return host ?? newHost;
			}
		}
		set => Volatile.Write(ref field, value);
	}

	public CodeDomProvider CodeDomProvider
	{
		get
		{
			var provider = field;
			var codeDomProviderType = this.Host.CodeLanguage.CodeDomProviderType;

			if (provider is null || provider.GetType() != codeDomProviderType)
			{
				var newProvider = CreateCodeDomProvider(codeDomProviderType);

				var comparand = provider;

				provider = Interlocked.CompareExchange(ref field, comparand: comparand, value: newProvider);

				if (provider is null || provider == comparand || provider.GetType() != codeDomProviderType)
					provider = newProvider;
			}

			return provider;

			static CodeDomProvider CreateCodeDomProvider(Type codeDomProviderType)
			{
				if (codeDomProviderType == null)
					return null!;

				if (codeDomProviderType == typeof(CSharpCodeProvider))
					return new CSharpCodeProvider();

				if (codeDomProviderType == typeof(VBCodeProvider))
					return new VBCodeProvider();

				if (!typeof(CodeDomProvider).IsAssignableFrom(codeDomProviderType))
					throw new InvalidCastException();

				var codeDomProvider = Activator.CreateInstance(codeDomProviderType);
				return (CodeDomProvider)codeDomProvider;
			}
		}
	}

	public RazorParser CreateRazorParser()
	{
		var host = this.Host;

		var razorParser = host.CreateRazorParser();
		razorParser.DesignTimeMode = host.DesignTimeMode;
		return razorParser;
	}

	public GenerateCodeResult GenerateCode(Block document, string? className = null, string? rootNamespace = null, string? sourceFileName = null, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(document);

		var host = this.Host;

		var generator = host.CreateRazorCodeGenerator(className ?? host.DefaultClassName, rootNamespace ?? host.DefaultNamespace, sourceFileName ?? host.PhysicalPath);

		generator.DesignTimeMode = host.DesignTimeMode;
		generator.GenerateLinePragmas = this.GenerateLinePragmas;

		if (cancellationToken.CanBeCanceled)
			generator.CancelToken = cancellationToken;

		generator.Visit(document);

		host.PostProcessGeneratedCode(generator.Context);

		return new(generator.Context);
	}
}

public record struct GenerateCodeResult
{
	[SetsRequiredMembers]
	internal GenerateCodeResult(CodeGeneratorContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		this.SourceFile = context.SourceFile;
		this.CompileUnit = context.CompileUnit;
		this.Namespace = context.Namespace;
		this.GeneratedClass = context.GeneratedClass;
		this.CodeMappings = context.CodeMappings;
	}

	public string SourceFile { readonly get; set; }
	public CodeCompileUnit CompileUnit { readonly get; set; }
	public CodeNamespace Namespace { readonly get; set; }
	public CodeTypeDeclaration GeneratedClass { readonly get; set; }
	public IDictionary<int, GeneratedCodeMapping> CodeMappings { readonly get; set; }
}
