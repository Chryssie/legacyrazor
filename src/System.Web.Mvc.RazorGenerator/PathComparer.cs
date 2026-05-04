using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.CodeAnalysis.Operations;

namespace System.Web.Mvc;

internal abstract class TextComparer : StringComparer, IEqualityComparer<ReadOnlyMemory<char>>, IComparer<ReadOnlyMemory<char>>, IEqualityComparer<char>, IComparer<char>
{
	public unsafe virtual int Compare(char x, char y) => x == y ? 0 : this.Compare(new ReadOnlySpan<char>(&x, 1), new(&y, 1));
	public unsafe virtual bool Equals(char x, char y) => x == y || this.Equals(new ReadOnlySpan<char>(&x, 1), new(&y, 1));
	public unsafe virtual int GetHashCode(char obj) => this.GetHashCode(new ReadOnlySpan<char>(&obj, 1));

	public virtual int Compare(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) => x.Equals(y) ? 0 : this.Compare(x.Span, y.Span);
	public virtual bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) => x.Equals(y) || this.Equals(x.Span, y.Span);
	public virtual int GetHashCode(ReadOnlyMemory<char> obj) => this.GetHashCode(obj.Span);

	public abstract int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
	public abstract bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y);
	public abstract int GetHashCode(ReadOnlySpan<char> obj);

	public override int Compare(string? x, string? y)
	{
		if (ReferenceEquals(x, y))
			return 0;

		if (x is null)
			return -1;
		if (y is null)
			return 1;

		return this.Compare(x.AsSpan(), y.AsSpan());
	}

	public override bool Equals(string? x, string? y)
	{
		if (ReferenceEquals(x, y))
			return true;

		if (x is null || y is null)
			return false;

		return this.Equals(x.AsSpan(), y.AsSpan());
	}
	public override int GetHashCode(string? obj) => this.GetHashCode(obj.AsSpan());
}

internal sealed class PathComparer : TextComparer
{
	public static PathComparer Instance { get; } = new();

	private PathComparer() { }

	const int MaxUtf16CharsPerRune = 2;

	public override int GetHashCode(ReadOnlyMemory<char> obj) => obj.Length is 0 ? 0 : base.GetHashCode(obj);

	public override int Compare(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
	{
		var xEnumerator = x.EnumerateRunes();
		var yEnumerator = y.EnumerateRunes();

		Span<char> xSpan = stackalloc char[MaxUtf16CharsPerRune];
		Span<char> ySpan = stackalloc char[MaxUtf16CharsPerRune];

		while (xEnumerator.MoveNext())
		{
			if (!yEnumerator.MoveNext())
				return 1;

			var result = xSpan.Slice(0, DecodeForCompare(xEnumerator.Current, xSpan)).CompareTo(ySpan.Slice(0, DecodeForCompare(yEnumerator.Current, ySpan)), StringComparison.OrdinalIgnoreCase);

			if (result != 0)
				return result;
		}

		if (!yEnumerator.MoveNext())
			return -1;

		return 0;
	}
	public override bool Equals(ReadOnlySpan<char> x, ReadOnlySpan<char> y)
	{
		var xEnumerator = x.EnumerateRunes();
		var yEnumerator = y.EnumerateRunes();

		while (xEnumerator.MoveNext())
		{
			if (!yEnumerator.MoveNext())
				return false;


		}

		return !yEnumerator.MoveNext();
	}

	private static int DecodeForCompare(Rune rune, Span<char> destination)
	{
		Contract.Assert(unchecked((uint)destination.Length >= (uint)rune.Utf16SequenceLength));

		if (rune.IsBmp)
		{
			if (unchecked((uint)destination.Length) < 1)
				ThrowDestinationTooSmall();

			var c = unchecked((char)(uint)rune.Value);
			destination[0] = c is '/' or '\\' ? '/' : c;
			return 1;
		}
		else
		{
			if (unchecked((uint)destination.Length) < MaxUtf16CharsPerRune)
				ThrowDestinationTooSmall();

			DeconstructNonBmpRune(rune, out destination[0], out destination[1]);
			return MaxUtf16CharsPerRune;
		}
	}

	private static void ThrowDestinationTooSmall() => throw new ArgumentException();

	public override int GetHashCode(ReadOnlySpan<char> obj)
	{
		if (obj.Length == 0)
			return 0;

		var enumerator = obj.EnumerateRunes();

		if (!enumerator.MoveNext())
			return 0;

		var hashCode = new HashCodeAccumulator();

		do
		{
			hashCode.Add(enumerator.Current);

		} while (enumerator.MoveNext());

		return hashCode.ToHashCode();
	}

	public override int GetHashCode(string? obj)
	{
		if (string.IsNullOrEmpty(obj))
			return 0;

		return this.GetHashCode(obj.AsSpan());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void DeconstructNonBmpRune(Rune rune, out char highSurrogateCodePoint, out char lowSurrogateCodePoint)
	{
		Contract.Assert(Rune.IsValid(rune.Value) && !rune.IsBmp);

		highSurrogateCodePoint = (char)(((uint)rune.Value + ((0xD800u - 0x40u) << 10)) >> 10);
		lowSurrogateCodePoint = (char)(((uint)rune.Value & 0x3FFu) + 0xDC00u);
	}

	private unsafe struct HashCodeAccumulator
	{
		private HashCode hashCode;
		private Accumulator buffer;

		[StructLayout(LayoutKind.Sequential, Size = Size, Pack = sizeof(char))]
		private struct Accumulator
		{
			public const int Size = sizeof(ushort) + sizeof(char) + sizeof(char) + sizeof(char);
			public const int MaxLength = 3;

			private ushort length;
			private char c0;
			private char c1;
			private char c2;

			public void Add(Rune rune, ref HashCode hashCode)
			{
				var length = unchecked((int)this.length);
				Contract.Assert(length is 0 or 1);

				fixed (char* c = &c0)
				{
					length = unchecked(length + Decode(rune, c + unchecked((uint)length), MaxLength - length));

					Contract.Assert(length is >= 0 and <= 3);

					if (unchecked((uint)length >= 2))
					{
						hashCode.Add(Unsafe.ReadUnaligned<int>(c));
						length = unchecked(length - 2);

						if (length != 0)
							c0 = c2;
					}

					this.length = unchecked((ushort)length);
				}
			}


			private static int Decode(Rune rune, char* pointer, int destinationLength)
			{
				Contract.Assert(unchecked((uint)destinationLength >= (uint)rune.Utf16SequenceLength));

				if (rune.IsBmp)
				{
					if (unchecked((uint)destinationLength) < 1)
						ThrowDestinationTooSmall();

					var c = unchecked((char)(uint)rune.Value);
					*pointer = c is '/' or '\\' ? '/' : char.ToUpperInvariant(c);
					return 1;
				}
				else
				{
					if (unchecked((uint)destinationLength) < MaxUtf16CharsPerRune)
						ThrowDestinationTooSmall();

					DeconstructNonBmpRune(rune, out pointer[0], out pointer[1]);

					var span = new Span<char>(pointer, MaxUtf16CharsPerRune);

					return span.ToUpperInvariant(span);
				}
			}

			public void AddRest(ref HashCode hashCode)
			{
				var length = unchecked((int)this.length);
				Contract.Assert(length is 0 or 1);

				if (length != 0)
				{
					hashCode.Add(c0);
					this.length = 0;
				}
			}
		}

		public void Add(Rune rune)
		{
			this.buffer.Add(rune, ref this.hashCode);
		}

		public int ToHashCode()
		{
			this.buffer.AddRest(ref this.hashCode);
			return hashCode.ToHashCode();
		}
	}
}
