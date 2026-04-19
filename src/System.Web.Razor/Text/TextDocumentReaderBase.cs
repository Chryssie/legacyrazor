// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace System.Web.Razor.Text
{
	public abstract class TextDocumentReaderBase() : TextDocumentReader()
	{
		internal override ITextDocument Document
		{
			get => throw new NotImplementedException("Not implemented by-design.");
			private protected init => throw new NotImplementedException("Not implemented by-design.");
		}

		public abstract override SourceLocation Location { get; }
		public abstract override int Length { get; }
		public override abstract int Position { get; set; }
		public abstract override int Read();
		public abstract override int Peek();
	}
}
