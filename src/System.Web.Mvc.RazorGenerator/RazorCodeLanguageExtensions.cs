using System.CodeDom.Compiler;
using System.Web.Razor;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace System.Web.Mvc;

internal static class RazorCodeLanguageExtensions
{

#nullable enable
	public static CodeDomProvider CreateCodeDomProvider(this RazorCodeLanguage razorCodeLanguage)
	{
		var codeDomProviderType = razorCodeLanguage.CodeDomProviderType;

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
#nullable restore

}
