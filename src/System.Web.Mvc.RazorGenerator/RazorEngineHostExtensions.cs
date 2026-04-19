using System.Collections.Generic;
using System.Web.Razor;
using System.Web.Razor.Generator;
using System.Web.Razor.Parser;
using System.Web.Razor.Text;

namespace System.Web.Mvc;

public static class RazorEngineHostExtensions
{
	extension(RazorEngineHost host)
	{
		public RazorParser CreateRazorParser()
		{
			var codeParser = host.CodeLanguage.CreateCodeParser();
			var markupParser = host.CreateMarkupParser();

			return new RazorParser(
				codeParser: host.DecorateCodeParser(codeParser),
				markupParser: host.DecorateMarkupParser(markupParser)
			);
		}

		public RazorCodeGenerator CreateRazorCodeGenerator(string className, string? rootNamespace = null, string? sourceFileName = null)
		{
			var codeGenerator = host.CodeLanguage.CreateCodeGenerator(className, rootNamespace, sourceFileName, host);
			return host.DecorateCodeGenerator(codeGenerator);
		}

		public GeneratorResults GenerateCode(ITextDocument input, string? className = null, string? rootNamespace = null, string? sourceFileName = null)
		{
			className ??= host.DefaultClassName;
			rootNamespace ??= host.DefaultNamespace;

			var parser = CreateRazorParser(host);
			var results = parser.Parse(input);

			var generator = CreateRazorCodeGenerator(host, className, rootNamespace, sourceFileName);
			generator.DesignTimeMode = host.DesignTimeMode;
			generator.Visit(results);

			host.PostProcessGeneratedCode(generator.Context);

			var designTimeLineMappings = default(IDictionary<int, GeneratedCodeMapping>);
			if (host.DesignTimeMode)
				designTimeLineMappings = generator.Context.CodeMappings;
			
			return new GeneratorResults(results, generator.Context.CompileUnit, designTimeLineMappings);
		}
	}
}
