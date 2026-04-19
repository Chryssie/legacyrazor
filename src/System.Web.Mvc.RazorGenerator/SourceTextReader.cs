using System.Buffers;
using System.Diagnostics.Contracts;
using System.IO;
using System.Web.Razor.Text;

using Microsoft.CodeAnalysis.Text;

namespace System.Web.Mvc;

internal sealed class SourceTextReader(SourceText sourceText) : TextDocumentReaderBase
{
	private readonly SourceText sourceText = sourceText;

	public override SourceLocation Location => this.sourceText.GetSourceLocation(this.Position);

	public override int Length => this.sourceText.Length;

	public override int Position { get; set; }

	public override int Read(char[] buffer, int index, int count)
	{
		if (buffer is null)
		{
			if (index != 0 || count != 0)
				throw new ArgumentOutOfRangeException();

			return 0;
		}

		if (unchecked(((ulong)(uint)index + (uint)count) > (uint)buffer.Length))
			throw new ArgumentOutOfRangeException();

		var position = this.Position;
		var actualCount = unchecked((int)Math.Min((uint)count, (uint)(this.sourceText.Length - position)));

		if (actualCount == 0)
			return 0;

		sourceText.CopyTo(position, buffer, index, actualCount);
		this.Position = unchecked(position + actualCount);

		return actualCount;
	}

	public override string? ReadLine()
	{
		var position = this.Position;

		if (unchecked((uint)position >= (uint)this.sourceText.Length))
			return null;

		var line = this.sourceText.Lines.GetLineFromPosition(position);

		var s = this.GetString(position, line.End - position);

		this.Position = line.EndIncludingLineBreak;
		return s;
	}

	public override string ReadToEnd()
	{
		var position = this.Position;
		var length = this.sourceText.Length;

		if (unchecked((uint)position >= (uint)length))
			return string.Empty;

		var s = this.GetString(position, length);

		this.Position = length;

		return s;
	}

	private string GetString(int startIndex, int count)
	{
		Contract.Assert(unchecked((uint)startIndex < (uint)this.sourceText.Length));
		Contract.Assert(unchecked(((ulong)(uint)startIndex + (uint)count) <= (uint)this.sourceText.Length));

		var buffer = ArrayPool<char>.Shared.Rent(count);
		try
		{
			this.sourceText.CopyTo(startIndex, buffer, 0, count);
			return new ReadOnlySpan<char>(buffer, 0, count).ToString();
		}
		finally
		{
			ArrayPool<char>.Shared.Return(buffer);
		}
	}

	public override int Read()
	{
		var position = this.Position;

		if (unchecked((uint)position >= (uint)this.sourceText.Length))
			return -1;

		var c = this.sourceText[position];
		this.Position = unchecked(position + 1);
		return c;
	}

	public override int Peek()
	{
		var position = this.Position;
		if (unchecked((uint)position >= (uint)this.sourceText.Length))
			return -1;

		return this.sourceText[position];
	}
}
