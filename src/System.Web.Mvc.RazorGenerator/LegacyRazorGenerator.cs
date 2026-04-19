using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Razor;

using Microsoft.CodeAnalysis;

namespace System.Web.Mvc;

[Generator]
public partial class LegacyRazorGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
#if DEBUG
		var paths = context.AdditionalTextsProvider.Select((at, ct) => at.Path)
			.Collect().Select((p, ct) =>
			{
#pragma warning disable RS1035 // Do not use APIs banned for analyzers // Should be okay for new line and also this is only for debugging.
				var newLine = Environment.NewLine;
#pragma warning restore RS1035 // Do not use APIs banned for analyzers

				var sb = new StringBuilder();

				foreach(var s in p)
				{
					sb.Append("// ").Append(s).Append(newLine);
				}

				return sb.ToString();
			});

		context.RegisterSourceOutput(paths, static (context, paths) => context.AddSource("_paths_", paths));
#endif

		var legacyRazorGeneratorOptions = context.AnalyzerConfigOptionsProvider
			.Combine(context.ParseOptionsProvider)
			.Select(ComputeLegacyRazorSourceGeneratorOptions)
			.ReportDiagnostics(context);

		var sourceItems = context.AdditionalTextsProvider
			.Where(s => s.Path is { } path && RazorCodeLanguage.Languages.ContainsKey(Path.GetExtension(path).TrimStart('.')))
			.Combine(context.AnalyzerConfigOptionsProvider)
			.Select(ComputeProjectItems)
			.ReportDiagnostics(context)
			.WhereNotNull();

		var parsedItems = sourceItems
			.Select(ParseDocument)
			.ReportDiagnostics(context)
			.WhereNotNull();

		var outputs = parsedItems
			.Combine(legacyRazorGeneratorOptions)
			.Select(GenerateCode)
			.ReportDiagnostics(context)
			.WhereNotNull();

		context.RegisterSourceOutput(outputs, static (context, item) => context.AddSource(item.HintName, item.SourceText));
	}
}
