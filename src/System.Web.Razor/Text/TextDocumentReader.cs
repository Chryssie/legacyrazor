// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Web.Razor.Text
{
	public class TextDocumentReader : TextReader, ITextDocument
	{
		private protected TextDocumentReader() => Contract.Assert(this is TextDocumentReaderBase);

		[Obsolete($"Do not create instances of this class, instead, either inherit from {nameof(TextDocumentReaderBase)}", error: true)]
		public TextDocumentReader(ITextDocument source) => this.Document = source;

		internal virtual ITextDocument Document
		{
			get
			{
				ref var document = ref DocumentHolder.GetDocumentRefOrNullRef(this);

				return Unsafe.IsNullRef(ref document) ? null : document;
			}
			private protected init
			{
				if (value is null)
				{
					ref var document = ref DocumentHolder.GetDocumentRefOrNullRef(this);

					if (!Unsafe.IsNullRef(ref document))
						document = null;
				}
				else
				{
					ref var document = ref DocumentHolder.GetOrCreateDocumentRef(this);

					document = value;
				}
			}
		}

		public virtual SourceLocation Location => Document.Location;

		public virtual int Length => Document.Length;

		public virtual int Position
		{
			get => Document.Position;
			set => Document.Position = value;
		}

		public override int Read() => Document.Read();

		public override int Peek() => Document.Peek();
	}

	/// <remarks>Allows us to store the <see cref="TextDocumentReader.Document"/> for the base class externally so that legacy usages work the same but new implementations don't include an unnecessary field. Whilst there is a cost associated with this, it shouldn't actually come into play as we won't construct instances of it ourselves and other code is unlikely to have either.</remarks>
	file sealed class DocumentHolder
	{
#nullable enable
		private static ConditionalWeakTable<TextDocumentReader, DocumentHolder>? documentHolders;

		private static ConditionalWeakTable<TextDocumentReader, DocumentHolder> GetOrInitDocumentHolders() => documentHolders ?? InitDocumentHolders();

		private static ConditionalWeakTable<TextDocumentReader, DocumentHolder> InitDocumentHolders()
		{
			var newDocuments = new ConditionalWeakTable<TextDocumentReader, DocumentHolder>();

			var documents = Interlocked.CompareExchange(ref documentHolders, comparand: null, value: newDocuments);

			return documents ?? newDocuments;
		}

		private static readonly ConditionalWeakTable<TextDocumentReader, DocumentHolder>.CreateValueCallback createValueCallback = (r) => new();
#nullable restore

		private ITextDocument document;

		private DocumentHolder()
		{

		}



		public static ref ITextDocument GetOrCreateDocumentRef(TextDocumentReader obj)
		{
			if (obj is null)
				throw new ArgumentNullException(nameof(obj));

			var documentHolders = GetOrInitDocumentHolders();

			var documentHolder = documentHolders.GetValue(obj, createValueCallback);

			return ref documentHolder.document;
		}

		public static ref ITextDocument GetDocumentRefOrNullRef(TextDocumentReader obj)
		{
			if (obj is null)
				throw new ArgumentNullException(nameof(obj));

			var documentHolders = Volatile.Read(ref DocumentHolder.documentHolders);

			if (documentHolders is null || !documentHolders.TryGetValue(obj, out var documentHolder))
				return ref Unsafe.NullRef<ITextDocument>();

			return ref documentHolder.document;
		}
	}
}
