#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System;

static partial class ArgumentNullExceptionExtensions
{
	extension(ArgumentNullException)
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[OverloadResolutionPriority(1)]
		public static void ThrowIfNull<T>([NotNull] T argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
		{
			if (argument is null)
				Throw(paramName);
		}

		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void Throw(string? paramName = null) => throw new ArgumentNullException(paramName: paramName);
	}
}
