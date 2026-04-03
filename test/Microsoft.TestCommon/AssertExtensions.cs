using System;
using System.Text;

using Microsoft.TestCommon;

namespace Xunit;

public static class AssertExtensions
{
	extension(Assert)
	{
		public static ArgumentNullException ThrowsArgumentNull(Action testCode, string paramName)
			=> Assert.Throws<ArgumentNullException>(paramName: paramName, testCode);

		/// <summary>
		/// Verifies that the code throws an ArgumentNullException with the expected message that indicates that the value cannot
		/// be null or empty string.
		/// </summary>
		/// <param name="testCode">A delegate to the code to be tested</param>
		/// <param name="paramName">The name of the parameter that should throw the exception</param>
		/// <returns>The exception that was thrown, when successful</returns>
		/// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
		public static ArgumentException ThrowsArgumentNullOrEmptyString(Action testCode, string paramName)
		{
			var ex = Assert.ThrowsAny<ArgumentException>(testCode);

			Assert.Equal(expected: paramName, ex.ParamName);
			Assert.Equal(expected: AppendExpectedParamName("Value cannot be null or an empty string.", paramName), actual: ex.Message);

			return ex;
		}

		/// <summary>
		/// Verifies that the code throws an <see cref="ArgumentException"/> (or optionally any exception which derives from it).
		/// </summary>
		/// <param name="testCode">A delegate to the code to be tested</param>
		/// <param name="paramName">The name of the parameter that should throw the exception</param>
		/// <returns>The exception that was thrown, when successful</returns>
		/// <exception cref="ThrowsException">Thrown when an exception was not thrown, or when an exception of the incorrect type is thrown</exception>
		public static ArgumentException ThrowsArgument(Action testCode, string paramName)
			=> Assert.Throws<ArgumentException>(paramName: paramName, testCode);

		public static ArgumentOutOfRangeException ThrowsArgumentOutOfRange(Action testCode, string paramName, string exceptionMessage, object actualValue = null)
		{
			var ex = Assert.Throws<ArgumentOutOfRangeException>(paramName, testCode);

			Assert.Equal(expected: GetExpectedArgumentOutOfRangeExceptionMessage(expectedParamName: paramName, expectedMessage: exceptionMessage, actualValue: actualValue, CultureReplacer.DefaultCulture), actual: ex.Message);
			Assert.Equal(expected: paramName, actual: ex.ParamName);

			return ex;
		}

		public static ArgumentOutOfRangeException ThrowsArgumentOutOfRange(Action testCode, string paramName, object actualValue = null)
		{
			var ex = Assert.Throws<ArgumentOutOfRangeException>(paramName, testCode);

			Assert.Equal(expected: GetExpectedArgumentOutOfRangeExceptionMessage(expectedParamName: paramName, actualValue: actualValue, CultureReplacer.DefaultCulture), actual: ex.Message);
			Assert.Equal(expected: paramName, actual: ex.ParamName);

			return ex;
		}

		private static string GetExpectedArgumentOutOfRangeExceptionMessage(string expectedParamName, object actualValue = null, IFormatProvider formatProvider = null)
			=> GetExpectedArgumentOutOfRangeExceptionMessage(expectedParamName, "Specified argument was out of range of the valid values.", actualValue, formatProvider);
		private static string GetExpectedArgumentOutOfRangeExceptionMessage(string expectedParamName, string expectedMessage, object actualValue = null, IFormatProvider formatProvider = null)
		{
			expectedMessage ??= "Exception of type 'System.ArgumentOutOfRangeException' was thrown.";

			if (string.IsNullOrEmpty(expectedParamName) && actualValue is null)
				return expectedMessage;

			var sb = new StringBuilder(expectedMessage);

			AppendExpectedParamName(sb, expectedParamName);

			if (actualValue is not null)
			{
				sb.Append(Environment.NewLine).Append("Actual value was ");

				if (actualValue is IFormattable formattable)
					sb.Append(formattable.ToString(format: null, formatProvider));
				else
					sb.Append(actualValue);
			}

			return sb.ToString();
		}

		private static string AppendExpectedParamName(string message, string expectedParamName)
		{
			if (string.IsNullOrEmpty(expectedParamName))
				return message;

			var sb = new StringBuilder(message);
			AppendExpectedParamName(sb, expectedParamName);
			return sb.ToString();
		}

		private static void AppendExpectedParamName(StringBuilder builder, string expectedParamName)
		{
			if (!string.IsNullOrEmpty(expectedParamName))
			{
#if NETCOREAPP3_1_OR_GREATER
				builder.Append(" (Parameter '").Append(expectedParamName).Append("')");
#else
				builder.Append(Environment.NewLine).Append("Parameter name: ").Append(expectedParamName);
#endif
			}
		}
	}
}
