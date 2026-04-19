using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System.Web.Mvc;

#if DEBUG

internal static class LoaderHelper
{
	const string CodeDomPath = "S:\\.nuget\\packages\\system.codedom\\4.4.0\\lib\\netstandard2.0\\System.CodeDom.dll";

	[ModuleInitializer]
	public static void ModuleInit()
	{
		AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
	}

	private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	{
		if (args.Name?.StartsWith("System.CodeDom,", StringComparison.OrdinalIgnoreCase) ?? false)
		{
			return Assembly.LoadFile(CodeDomPath);
		}

		return null;
	}
}

#endif
