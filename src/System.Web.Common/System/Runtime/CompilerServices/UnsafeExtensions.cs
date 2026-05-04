using System.ComponentModel;

namespace System.Runtime.CompilerServices;

static partial class UnsafeExtensions
{
	extension(Unsafe)
	{
		/// <inheritdoc cref="Unsafe.Add{T}(ref T, intIntPtr"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T Add<T>(ref readonly T source, IntPtr elementOffset) => ref Unsafe.Add(ref Unsafe.AsRef(in source), elementOffset);
		/// <inheritdoc cref="Unsafe.Add{T}(ref T, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T Add<T>(ref readonly T source, int elementOffset) => ref Unsafe.Add(ref Unsafe.AsRef(in source), elementOffset);
		/// <inheritdoc cref="Unsafe.Add{T}(ref  T, nuint)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T Add<T>(ref readonly T source, nuint elementOffset) => ref Unsafe.Add(ref Unsafe.AsRef(in source), elementOffset);
		/// <inheritdoc cref="Unsafe.AddByteOffset{T}(ref T, nuint)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T AddByteOffset<T>(ref readonly T source, nuint byteOffset) => ref Unsafe.Add(ref Unsafe.AsRef(in source), byteOffset);
		/// <inheritdoc cref="Unsafe.AddByteOffset{T}(ref T, IntPtr)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T AddByteOffset<T>(ref readonly T source, IntPtr byteOffset) => ref Unsafe.Add(ref Unsafe.AsRef(in source), byteOffset);

		/// <inheritdoc cref="Unsafe.Subtract{T}(ref T, intIntPtr"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T Subtract<T>(ref readonly T source, IntPtr elementOffset) => ref Unsafe.Subtract(ref Unsafe.AsRef(in source), elementOffset);
		/// <inheritdoc cref="Unsafe.Subtract{T}(ref T, int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T Subtract<T>(ref readonly T source, int elementOffset) => ref Unsafe.Subtract(ref Unsafe.AsRef(in source), elementOffset);
		/// <inheritdoc cref="Unsafe.Subtract{T}(ref  T, nuint)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T Subtract<T>(ref readonly T source, nuint elementOffset) => ref Unsafe.Subtract(ref Unsafe.AsRef(in source), elementOffset);
		/// <inheritdoc cref="Unsafe.SubtractByteOffset{T}(ref T, nuint)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T SubtractByteOffset<T>(ref readonly T source, nuint byteOffset) => ref Unsafe.Subtract(ref Unsafe.AsRef(in source), byteOffset);
		/// <inheritdoc cref="Unsafe.SubtractByteOffset{T}(ref T, IntPtr)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref readonly T SubtractByteOffset<T>(ref readonly T source, IntPtr byteOffset) => ref Unsafe.Subtract(ref Unsafe.AsRef(in source), byteOffset);

		/// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)"/>
		public static ref readonly TTo As<TFrom, TTo>(ref readonly TFrom source) => ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in source));

#if !NET8_0_OR_GREATER
		/// <inheritdoc cref="Unsafe.ByteOffset{T}(ref T, ref T)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr ByteOffset<T>(ref readonly T origin, ref readonly T target) => Unsafe.ByteOffset(ref Unsafe.AsRef(in origin), ref Unsafe.AsRef(in target));

		/// <inheritdoc cref="Unsafe.Copy{T}(void*, ref T)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void Copy<T>(void* destination, ref readonly T source) => Unsafe.Copy(destination, ref Unsafe.AsRef(in source));

		/// <inheritdoc cref="Unsafe.CopyBlock(ref byte, ref byte, uint)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlock(ref byte destination, ref readonly byte source, uint byteCount) => Unsafe.CopyBlock(ref destination, ref Unsafe.AsRef(in source), byteCount);

		/// <inheritdoc cref="Unsafe.CopyBlockUnaligned(ref byte, ref byte, uint)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlockUnaligned(ref byte destination, ref readonly byte source, uint byteCount) => Unsafe.CopyBlockUnaligned(ref destination, ref Unsafe.AsRef(in source), byteCount);

		/// <inheritdoc cref="Unsafe.IsNullRef{T}(ref T)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullRef<T>(ref readonly T source) => Unsafe.IsNullRef(ref Unsafe.AsRef(in source));

		/// <inheritdoc cref="Unsafe.ReadUnaligned{T}(ref byte)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadUnaligned<T>(ref readonly byte source) => Unsafe.ReadUnaligned<T>(ref Unsafe.AsRef(in source));

		/// <inheritdoc cref="Unsafe.AreSame{T}(ref T, ref T)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AreSame<T>(ref readonly T left, ref readonly T right) => Unsafe.AreSame(ref Unsafe.AsRef(in left), ref Unsafe.AsRef(in right));

		/// <inheritdoc cref="Unsafe.IsAddressGreaterThan{T}(ref T, ref T)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressGreaterThan<T>(ref readonly T left, ref readonly T right) => Unsafe.IsAddressGreaterThan(ref Unsafe.AsRef(in left), ref Unsafe.AsRef(in right));

		/// <inheritdoc cref="Unsafe.IsAddressLessThan{T}(ref T, ref T)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressLessThan<T>(ref readonly T left, ref readonly T right) => Unsafe.IsAddressLessThan(ref Unsafe.AsRef(in left), ref Unsafe.AsRef(in right));
#endif
	}

#if NET8_0_OR_GREATER

	/// <inheritdoc cref="Unsafe.ByteOffset{T}(ref readonly T, ref readonly T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static IntPtr ByteOffset<T>(ref readonly T origin, ref readonly T target) => Unsafe.ByteOffset(in origin, in target);

	/// <inheritdoc cref="Unsafe.Copy{T}(void*, ref readonly T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static unsafe void Copy<T>(void* destination, ref readonly T source) => Unsafe.Copy(destination, in source);

	/// <inheritdoc cref="Unsafe.CopyBlock(ref readonly byte, ref readonly byte, uint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static void CopyBlock(ref byte destination, ref readonly byte source, uint byteCount) => Unsafe.CopyBlock(ref destination, in source, byteCount);

	/// <inheritdoc cref="Unsafe.CopyBlockUnaligned(ref readonly byte, ref readonly byte, uint)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static void CopyBlockUnaligned(ref byte destination, ref readonly byte source, uint byteCount) => Unsafe.CopyBlockUnaligned(ref destination, in source, byteCount);

	/// <inheritdoc cref="Unsafe.IsNullRef{T}(ref readonly T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static bool IsNullRef<T>(ref readonly T source) => Unsafe.IsNullRef(in source);

	/// <inheritdoc cref="Unsafe.ReadUnaligned{T}(ref readonly byte)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static T ReadUnaligned<T>(ref readonly byte source) => Unsafe.ReadUnaligned<T>(in source);

	/// <inheritdoc cref="Unsafe.AreSame{T}(ref readonly T, ref readonly T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static bool AreSame<T>(ref readonly T left, ref readonly T right) => Unsafe.AreSame(in left, in right);

	/// <inheritdoc cref="Unsafe.IsAddressGreaterThan{T}(ref readonly T, ref readonly T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static bool IsAddressGreaterThan<T>(ref readonly T left, ref readonly T right) => Unsafe.IsAddressGreaterThan(in left, in right);

	/// <inheritdoc cref="Unsafe.IsAddressLessThan{T}(ref readonly T, ref readonly T)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never), Obsolete("Use built-in method.")]
	public static bool IsAddressLessThan<T>(ref readonly T left, ref readonly T right) => Unsafe.IsAddressLessThan(in left, in right);
#endif
}