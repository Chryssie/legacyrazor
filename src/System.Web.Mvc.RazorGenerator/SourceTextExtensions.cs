using System.Web.Razor.Text;

using Microsoft.CodeAnalysis.Text;

namespace System.Web.Mvc;

public static class SourceTextExtensions
{
	extension(SourceText sourceText)
	{
		public ITextDocument ToTextDocument()
		{
			if (sourceText is null)
				throw new ArgumentNullException(nameof(sourceText));

			return new SourceTextReader(sourceText);
		}

		public SourceLocation GetSourceLocation(int index)
		{
			if (unchecked((uint)index >= (uint)sourceText.Length))
				return get_EndSourceLocation(sourceText);

			var linePosition = sourceText.Lines.GetLinePosition(index);
			return new SourceLocation(index, linePosition.Line, linePosition.Character);
		}

		public SourceLocation EndSourceLocation
			=> sourceText.Length == 0 ? SourceLocation.Zero : new(sourceText.Length, sourceText.Lines.Count - 1, sourceText.Lines[^1].Span.Length);
	}
}
