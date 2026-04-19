using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;

namespace System.Web.Mvc;

internal static class IncrementalValuesProviderExtensions
{
	public static IncrementalValueProvider<T> WithComparison<T>(this IncrementalValueProvider<T> source, Func<T?, T?, bool> equalityComparison)
		=> source.WithComparer(new ComparisonComparer<T>(equalityComparison));
	public static IncrementalValuesProvider<T> WithComparison<T>(this IncrementalValuesProvider<T> source, Func<T?, T?, bool> equalityComparison)
		=> source.WithComparer(new ComparisonComparer<T>(equalityComparison));

	public static IncrementalValueProvider<bool> Any<T>(this IncrementalValuesProvider<T> source)
		=> source.Collect().Select(static (s, ct) => s.Any());

	public static IncrementalValueProvider<bool> Any<T>(this IncrementalValuesProvider<T> source, Func<T, bool> predicate)
		=> Any(source.Where(predicate));

	public static IncrementalValueProvider<bool> All<T>(this IncrementalValuesProvider<T> source, Func<T, bool> predicate)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		return source.Collect().Select((s, ct) => s.All(predicate));
	}

	public static IncrementalValuesProvider<T> WhereNotNull<T>(this IncrementalValuesProvider<T?> source) where T : struct
		=> source.Where(static s => s.HasValue).Select(static (s, ct) => s.GetValueOrDefault());

	public static IncrementalValuesProvider<T> WhereNotNull<T>(this IncrementalValuesProvider<T?> source) where T : class
		=> source.Where(static s => s is not null)!;

	public static IncrementalValueProvider<T> ReportDiagnostics<T>(this IncrementalValueProvider<(T,
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		)> source, IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(source, ReportDiagnostics);

		return source.Select(GetValue);
	}

	public static IncrementalValueProvider<T> ReportDiagnostics<T>(this IncrementalValueProvider<(T, ImmutableArray<
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		>)> source, IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(source, ReportDiagnostics);

		return source.Select(GetValue);
	}

	public static IncrementalValuesProvider<T> ReportDiagnostics<T>(this IncrementalValuesProvider<(T,
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		)> source, IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(source, ReportDiagnostics);

		return source.Select(GetValue);
	}

	public static IncrementalValuesProvider<T> ReportDiagnostics<T>(this IncrementalValuesProvider<(T, ImmutableArray<
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		>)> source, IncrementalGeneratorInitializationContext context)
	{
		context.RegisterSourceOutput(source, ReportDiagnostics);

		return source.Select(GetValue);
	}

	private static T GetValue<T>((T Value,
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		) value, CancellationToken cancellationToken)
		=> value.Value;
	private static T GetValue<T>((T Value, ImmutableArray<
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		>) value, CancellationToken cancellationToken)
		=> value.Value;

	private static void ReportDiagnostics<T>(SourceProductionContext context, (T,
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		 Diagnostic) value)
	{
		var diagnostic = value.Diagnostic;

		if (diagnostic is not null)
			context.ReportDiagnostic(diagnostic);
	}

	private static void ReportDiagnostics<T>(SourceProductionContext context, (T, ImmutableArray<
#nullable disable annotations
		Diagnostic
#nullable restore annotations
		> Diagnostics) value)
	{
		var diagnostics = value.Diagnostics;

		if (!diagnostics.IsDefault)
		{
			for (var i = 0; i < diagnostics.Length; i++)
			{
				var diagnostic = diagnostics[i];

				if (diagnostic is not null)
					context.ReportDiagnostic(diagnostic);
			}
		}
	}

	private sealed class ComparisonComparer<T> : IEqualityComparer<T>
	{
		private readonly Func<T?, T?, bool> equalityComparison;

		public ComparisonComparer(Func<T?, T?, bool> equalityComparison)
		{
			ArgumentNullException.ThrowIfNull(equalityComparison);
			this.equalityComparison = equalityComparison;
		}

		public bool Equals(T x, T y) => this.equalityComparison(x, y);

		int IEqualityComparer<T>.GetHashCode(T obj)
		{
			Contract.Assert(false, "Should not have executed this.");
			return 0;
		}
	}
}
