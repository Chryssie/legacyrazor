using System.Diagnostics.Contracts;

namespace System.Runtime.CompilerServices;

static partial class RuntimeHelpersExtensions
{
	extension(RuntimeHelpers)
	{
		internal static bool IsBitwiseEquatable<T>()
		{
			var type = typeof(T);

			if (typeof(T).IsEnum)
			{
				type = typeof(T).GetEnumUnderlyingType();
				Contract.Assert(type is not null);
			}

			return type!.IsPrimitive && type != typeof(float) && type != typeof(double);
		}
	}
}