namespace System.Web.Mvc;

public record struct LegacyRazorSourceGenerationOptions
{
	public required string RootNamespace { readonly get; init; }
}
