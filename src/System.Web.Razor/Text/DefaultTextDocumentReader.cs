// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace System.Web.Razor.Text
{
	public class DefaultTextDocumentReader(ITextDocument document) : TextDocumentReaderBase()
	{
		internal override ITextDocument Document { get; private protected init; } = document;

		public override SourceLocation Location => Document.Location;

		public override int Length => Document.Length;

		public override int Position
		{
			get => Document.Position;
			set => Document.Position = value;
		}

		public override int Read() => Document.Read();

		public override int Peek() => Document.Peek();
	}
}
