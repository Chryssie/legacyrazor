using System.Web.Razor.Parser.SyntaxTree;
using System.Web.Razor.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace System.Web.Mvc;

internal static class LegacyRazorDiagnostics
{
	public static Diagnostic TargetPathNotProvided(string path) => Diagnostic.Create(Definitions.TargetPathNotProvided, Location.None, path);
	public static Diagnostic ParserError(string path, TextLineCollection lines, RazorError razorError) => ParserError(path, lines, razorError.Message, razorError.Location, razorError.Length);
	public static Diagnostic ParserError(string path, TextLineCollection lines, string message, SourceLocation location, int length) => Diagnostic.Create(Definitions.ParserError, CreateLocation(path, lines, location, length), message);
	public static Diagnostic ParserFailure(string path) => Diagnostic.Create(Definitions.ParserFailure, Location.None, path);

	private static Location CreateLocation(string filePath, TextLineCollection lines, SourceLocation location, int length)
	{
		var textSpan = new TextSpan(location.AbsoluteIndex, length);

		return Location.Create(filePath, textSpan, lines.GetLinePositionSpan(textSpan));
	}

	internal static class Definitions
	{
		public static readonly DiagnosticDescriptor TargetPathNotProvided = new(
				Ids.TargetPathNotProvidedRuleId,
				"TargetPathNotProvidedTitle",//new LocalizableResourceString(nameof(RazorSourceGeneratorResources.TargetPathNotProvidedTitle), RazorSourceGeneratorResources.ResourceManager, typeof(RazorSourceGeneratorResources)),
				"TargetPathNotProvidedMessage: {0}",//new LocalizableResourceString(nameof(RazorSourceGeneratorResources.TargetPathNotProvidedMessage), RazorSourceGeneratorResources.ResourceManager, typeof(RazorSourceGeneratorResources)),
				"LegacyRazorSourceGenerator",
				DiagnosticSeverity.Warning,
				isEnabledByDefault: true
		);

		public static readonly DiagnosticDescriptor ParserError = new(
				Ids.ParserError,
				"ParserError",//new LocalizableResourceString(nameof(RazorSourceGeneratorResources.TargetPathNotProvidedTitle), RazorSourceGeneratorResources.ResourceManager, typeof(RazorSourceGeneratorResources)),
				"{0}",//new LocalizableResourceString(nameof(RazorSourceGeneratorResources.TargetPathNotProvidedMessage), RazorSourceGeneratorResources.ResourceManager, typeof(RazorSourceGeneratorResources)),
				"LegacyRazorSourceGenerator",
				DiagnosticSeverity.Error,
				isEnabledByDefault: true
		);

		public static readonly DiagnosticDescriptor ParserFailure = new(
				Ids.ParserFailure,
				"ParserFailure",//new LocalizableResourceString(nameof(RazorSourceGeneratorResources.TargetPathNotProvidedTitle), RazorSourceGeneratorResources.ResourceManager, typeof(RazorSourceGeneratorResources)),
				"ParserFailure",//new LocalizableResourceString(nameof(RazorSourceGeneratorResources.TargetPathNotProvidedMessage), RazorSourceGeneratorResources.ResourceManager, typeof(RazorSourceGeneratorResources)),
				"LegacyRazorSourceGenerator",
				DiagnosticSeverity.Error,
				isEnabledByDefault: true
		);
	}

	internal static class Ids
	{
		public const string InvalidRazorLangVersionRuleId = "RZ3600";
		public const string ReComputingTagHelpersRuleId = "LRSG001";
		public const string TargetPathNotProvidedRuleId = "LRSG002";
		public const string GeneratedOutputFullPathNotProvidedRuleId = "LRSG003";
		public const string CurrentCompilationReferenceNotFoundId = "LRSG004";
		public const string SkippingGeneratedFileWriteId = "LRSG005";
		public const string SourceTextNotFoundId = "LRSG006";
		public const string UnexpectedProjectItemReadCallId = "LRSG007";
		public const string InvalidRazorContextComputedId = "LRSG008";
		public const string MetadataReferenceNotProvidedId = "LRSG009";
		public const string ParserError = "LRSG010";
		public const string ParserFailure = "LRSG011";
	}
}
