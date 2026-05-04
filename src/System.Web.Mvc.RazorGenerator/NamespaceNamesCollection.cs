using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Web.Mvc;

[CollectionBuilder(typeof(NamespaceNamesCollection), nameof(Create))]
public readonly struct NamespaceNamesCollection : IList<string>, IReadOnlyList<string>
{
	public static NamespaceNamesCollection Empty => default;

	public static NamespaceNamesCollection Create() => default;
	public static NamespaceNamesCollection Create(string namespaceName)
	{
		ArgumentNullException.ThrowIfNull(namespaceName);

		return new(namespaceName);
	}
	public static NamespaceNamesCollection Create(params ReadOnlySpan<char> namespaceNames) => namespaceNames.Length switch
	{
		0 => Create(),
		1 => Create(namespaceNames[0]),
		_ => throw new NotImplementedException(),
	};

	internal struct InternalBuilder
	{
		private object? obj;

		internal InternalBuilder(NamespaceNamesCollection collection) => this.obj = collection.obj;

		public int Add(string namespaceName)
		{
			ArgumentNullException.ThrowIfNull(namespaceName);

			var obj = this.obj;

			var i = 0;
			if (obj is null)
			{
				this.obj = namespaceName;
				goto Added;
			}

			ImmutableArray<string>.Builder? builder;

			if (obj is string s)
			{
				if (namespaceName == s)
					goto Found;

				builder = ImmutableArray.CreateBuilder<string>();
				builder.Add(s);
				goto SetBuilderThenAdd;
			}

			builder = obj as ImmutableArray<string>.Builder;

			if (builder is not null)
			{
				i = builder.IndexOf(namespaceName);
				Contract.Assert(i >= -1);

				if (i != -1)
					goto Found;

				goto AddToExistingBuilder;
			}

			Contract.Assert(obj is string[]);
			var array = ImmutableCollectionsMarshal.AsImmutableArray(Unsafe.As<string[]>(obj));

			i = array.IndexOf(namespaceName);
			Contract.Assert(i >= -1);
			if (i == -1)
				goto Found;

			builder = array.ToBuilder();
			goto SetBuilderThenAdd;

		SetBuilderThenAdd:
			this.obj = builder;
			goto AddToExistingBuilder;
		AddToExistingBuilder:
			i = builder.Count;
			builder.Add(namespaceName);
		Added:
			return i;

		Found:
			return ~i;

		}

		public void Clear()
		{
			switch (this.obj)
			{
				case null:

					break;
				case string:
					this.obj = null;
					break;
				case ImmutableArray<string>.Builder builder:
					builder.Clear();
					break;
				case var obj:
					Contract.Assert(obj is string[]);

					this.obj = null;
					break;
			}
		}

		public NamespaceNamesCollection Drain()
		{
			switch (this.obj)
			{
				case null: return default;
				case string s: return new(s);
				case ImmutableArray<string>.Builder builder:
					switch (builder.Count)
					{
						case 0:
							return default;
						case 1:
							var result = new NamespaceNamesCollection(builder[0]);
							builder.Clear();
							return result;
						default:
							var array = builder.DrainToImmutable();
							return array.Length switch
							{
								0 => default,
								1 => new(array[0]),
								_ => new(ImmutableCollectionsMarshal.AsArray(array)),
							};
					}
				case var obj:
					Contract.Assert(obj is string[]);
					this.obj = null;
					return new(obj);
			}
		}
	}

	private readonly object? obj;

	private NamespaceNamesCollection(object? obj)
	{
		Contract.Assert(obj is null or string or string[] { Length: not (0 or 1), });
		this.obj = obj;
	}

	public string this[int index]
	{
		get
		{
			switch (this.obj)
			{
				case string s:
					if (index == 0)
						return s;

					break;
				case { } obj:
					Contract.Assert(obj is string[]);
					var array = Unsafe.As<string[]>(obj);

					return array[index];
			}

			return Array.Empty<string>()[index];
		}
	}

	public int Count
	{
		get
		{
			switch (this.obj)
			{
				case null: return 0;
				case string: return 1;
				case var obj:
					Contract.Assert(obj is string[]);
					var array = Unsafe.As<string[]>(obj);

					return array.Length;
			}
		}
	}

	string IList<string>.this[int index]
	{
		get => this[index];
		set => throw NotSupportedCollectionIsReadOnly();
	}

	private static Exception NotSupportedCollectionIsReadOnly() => new NotSupportedException("Collection is read-only.");

	public int IndexOf(string item)
	{
		if (item is null)
			return -1;

		if (GetObjOrSpan(out scoped var namespaceNames) is { } ns)
			namespaceNames = [ns];

		return namespaceNames.IndexOf(item);
	}

	void IList<string>.Insert(int index, string item) => throw NotSupportedCollectionIsReadOnly();
	void IList<string>.RemoveAt(int index) => throw NotSupportedCollectionIsReadOnly();
	void ICollection<string>.Add(string item) => throw NotSupportedCollectionIsReadOnly();
	void ICollection<string>.Clear() => throw NotSupportedCollectionIsReadOnly();
	bool ICollection<string>.Remove(string item) => throw NotSupportedCollectionIsReadOnly();

	public bool Contains(string item) => this.IndexOf(item) != -1;

	public void CopyTo(string[] array, int arrayIndex)
	{
		switch (this.obj)
		{
			case null:
				ValidateCopyToArgs(array, arrayIndex, 0);
				break;
			case string s:
				ValidateCopyToArgs(array, arrayIndex, 1);
				array[arrayIndex] = s;
				break;
			case var obj:
				Contract.Assert(obj is string[]);
				var source = ImmutableCollectionsMarshal.AsImmutableArray(Unsafe.As<string[]>(obj));
				source.CopyTo(array, arrayIndex);
				break;
		}
	}
	private static void ValidateCopyToArgs(string[] array, int arrayIndex, int count, [CallerArgumentExpression(nameof(array))] string? arrayParamName = null, [CallerArgumentExpression(nameof(arrayIndex))] string? arrayIndexParamName = null)
	{
		ArgumentNullException.ThrowIfNull(array, arrayParamName);

		ValidateCopyToRange(array.Length, arrayIndex, count, arrayIndexParamName: arrayIndexParamName);
	}
	private static void ValidateCopyToRange(int arrayLength, int arrayIndex, int count, [CallerArgumentExpression(nameof(arrayIndex))] string? arrayIndexParamName = null)
	{
		if (unchecked(((ulong)(uint)arrayIndex + (uint)count) > (uint)arrayLength))
		{
			if (unchecked((uint)arrayIndex > (uint)arrayLength))
				ThrowArgumentOutOfRangeException(arrayIndexParamName);
			else
				ThrowArgumentOffsetPlusOutOfBoundsException();
		}
	}

	[DoesNotReturn]
	private static void ThrowArgumentOutOfRangeException(string? paramName) => throw new ArgumentOutOfRangeException(paramName: paramName);
	[DoesNotReturn]
	private static void ThrowArgumentOffsetPlusOutOfBoundsException(string? paramName = null) => throw new ArgumentException(paramName: paramName, message: "Offset plus count was out of bounds of the array.");


	private static IEnumerator<string> GetEnumerator<TSource>(TSource source) where TSource : struct, IEnumerable<string>
		=> source.GetEnumerator();
	private readonly IEnumerator<string> GetEnumeratorObject()
	{
		ImmutableArray<string> array;
		switch (this.obj)
		{
			case null:
				array = ImmutableArray<string>.Empty;
				break;
			case string s:
				return new SingleEnumerator(s);
			case var obj:
				Contract.Assert(obj is string[]);
				array = ImmutableCollectionsMarshal.AsImmutableArray(Unsafe.As<string[]>(obj));
				Contract.Assert(array.Length != 0);
				break;
		}

		Contract.Assert(array.Length != 1);
		return GetEnumerator(array);
	}

	public Enumerator GetEnumerator() => new(this);
	[SuppressMessage("Style", "IDE0305:Simplify collection initialization", Justification = "Don't want to potentially change the meaning of this.")]
	IEnumerator<string> IEnumerable<string>.GetEnumerator() => this.GetEnumeratorObject();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator(this);

	bool ICollection<string>.IsReadOnly => true;

	public override bool Equals([NotNullWhen(true)] object? obj) => obj is NamespaceNamesCollection other && this.Equals(other);
	public readonly bool Equals(NamespaceNamesCollection other)
	{
		if (this.GetObjOrSpan(out scoped var namespaceNames) is { } ns)
			namespaceNames = [ns];

		if (other.GetObjOrSpan(out scoped var otherNamespaceNames) is { } otherNs)
			otherNamespaceNames = [otherNs];

		return namespaceNames.SequenceEqual(otherNamespaceNames);
	}
	public override int GetHashCode()
	{
		if (this.GetObjOrSpan(out scoped var namespaceNames) is { } ns23)
			namespaceNames = [ns23];

		if (namespaceNames.Length == 0)
			return 0;

		var hashCode = new HashCode();
		var i = 0;
		do
		{
			var namespaceName = namespaceNames[i];

			hashCode.Add(string.IsNullOrEmpty(namespaceName) ? 0 : namespaceName.GetHashCode());
		} while (unchecked((uint)++i < (uint)namespaceNames.Length));

		return hashCode.ToHashCode();
	}

	private readonly string? GetObjOrSpan(out ReadOnlySpan<string> span)
	{
		var obj = this.obj;

		if (obj is null)
		{
			span = default;
			return null;
		}

		if (obj is string s)
		{
			span = default;
			return s;
		}

		Contract.Assert(obj is string[]);
		var array = Unsafe.As<string[]>(obj);
		Contract.Assert(array.Length is not (0 or 1));

		span = array;
		return null;
	}

	internal bool AppendTo(StringBuilder sb)
	{
		if (this.GetObjOrSpan(out scoped var namespaceNames) is { } ns)
			namespaceNames = [ns];

		if (namespaceNames.Length == 0)
			return false;

		sb.Append(namespaceNames[0]);

		if (namespaceNames.Length != 1)
		{
			var i = 1;
			do
			{
				sb.Append(',').Append(' ').Append(namespaceNames[i]);
			} while (unchecked((uint)++i < (uint)namespaceNames.Length));
		}

		return true;
	}

	public override string ToString()
	{
		if (this.GetObjOrSpan(out scoped var namespaceNames) is { } ns)
			namespaceNames = [ns];

		if (namespaceNames.Length == 0)
			return string.Empty;

		if (namespaceNames.Length == 1)
			return namespaceNames[0] ?? string.Empty;

		var sb = new StringBuilder();
		sb.Append(namespaceNames[0]);

		var i = 1;
		do
		{
			sb.Append(',').Append(' ').Append(namespaceNames[i]);
		} while (unchecked((uint)++i < (uint)namespaceNames.Length));

		return sb.ToString();
	}

	internal readonly InternalBuilder ToInternalBuilder()
	{
		return new InternalBuilder(this);
	}

	public struct Enumerator
	{
		private readonly NamespaceNamesCollection collection;
		private int index;

		internal Enumerator(NamespaceNamesCollection collection)
		{
			this.collection = collection;
			this.index = -1;
		}

		public readonly string Current => this.collection[this.index];

		public bool MoveNext()
		{
			var index = unchecked(this.index + 1);
			var count = this.collection.Count;

			if (unchecked((uint)index >= (uint)count))
			{
				Contract.Assert(count != -1);

				this.index = count;
				return false;
			}

			this.index = index;
			return true;
		}

		public void Reset() => this.index = -1;
	}

	private sealed class SingleEnumerator : IEnumerator<string>
	{
		private enum State : byte
		{
			NotStarted,
			Started,
			Finished,
		}

		private readonly string value;
		private State state = State.NotStarted;

		internal SingleEnumerator(string value)
		{
			Contract.Assert(value is not null);
			this.value = value!;
		}

		public string Current
		{
			get
			{
				var state = this.state;
				if (state is not State.Started)
					ThrowEnumerationException(state);

				return this.value;
			}
		}

		object IEnumerator.Current => this.Current;

		public bool MoveNext()
		{
			var state = unchecked(this.state + 1);

			if (state >= State.Finished)
			{
				this.state = State.Finished;
				return false;
			}

			this.state = state;
			return true;
		}

		public void Reset()
			=> this.state = State.NotStarted;

		void IDisposable.Dispose() { }

		[DoesNotReturn]
		private static void ThrowEnumerationException(State state)
			=> throw new InvalidOperationException(state is State.NotStarted ? "Enumeration has not started. Call MoveNext" : "Enumeration has already finished.");
	}
}