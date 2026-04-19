// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace System.Web.Razor.Text
{
    public interface ITextBuffer
    {
        int Length { get; }
        int Position { get; set; }
        int Read();
        int Peek();
    }

    // TextBuffer with Location tracking
    public interface ITextDocument : ITextBuffer
    {
        SourceLocation Location { get; }
    }

	public static class ITextDocumentExtensions
	{
		public static TextDocumentReader AsTextDocumentReader(this ITextDocument textDocument)
		{
			ArgumentNullException.ThrowIfNull(textDocument);

			if (textDocument is not TextDocumentReader reader)
				reader = new DefaultTextDocumentReader(textDocument);

			return reader;
		}

		private sealed class SealedDefaultTextDocumentReader(ITextDocument textDocument) : DefaultTextDocumentReader(textDocument)
		{
			public override bool Equals(object obj) => (this == obj) || (obj is SealedDefaultTextDocumentReader other && this.Document.Equals(other.Document));
			public override int GetHashCode() => this.Document.GetHashCode();
		}
	}
}
