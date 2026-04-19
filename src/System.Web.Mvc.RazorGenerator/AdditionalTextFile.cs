using System.Diagnostics.Contracts;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace System.Web.Mvc;

public sealed class AdditionalTextFile : AdditionalText
{
	private readonly AdditionalText additionalText;

	public AdditionalTextFile(AdditionalText additionalText)
	{
		ArgumentNullException.ThrowIfNull(additionalText);

		this.additionalText = additionalText;
	}

	public override string Path => this.additionalText.Path;
	public required string RelativePath
	{
		get => field;
		init
		{
			ArgumentNullException.ThrowIfNull(value);

			if (value != field)
			{
				field = value;
				AppRelativePath = null!;
			}
		}
	}

	public string AppRelativePath
	{
		get
		{
			return field ?? InitAppRelativePath();

			string InitAppRelativePath()
			{
				var newValue = ComputeAppRelativePath(this.RelativePath);
				var value = Interlocked.CompareExchange(ref field, comparand: null, value: newValue);

				return value ?? newValue;
			}
		}
		private init
		{
			Contract.Assert(value is null);

			field = value;
		}
	}

	public override SourceText? GetText(CancellationToken cancellationToken = default) => this.text.GetText(cancellationToken);

	public override bool Equals(object obj)
		=> this == obj || (obj is AdditionalTextFile other && additionalText.Equals(other) && this.RelativePath == other.RelativePath);
	public override int GetHashCode()
	{
		var hashCode = this.additionalText.GetHashCode();

		if (this.RelativePath is { Length: not 0, } relativePath)
			hashCode = HashCode.Combine(hashCode, relativePath.GetHashCode());

		return hashCode;
	}

	public override string ToString() => $"RelativePath = {this.RelativePath}, {this.additionalText}";

	private static string ComputeAppRelativePath(string path)
	{
		const string Prefix = "~/";

		if (path is not null)
		{
			for (var i = 0; i < path.Length; i++)
			{
				if (path[i] is '\\' or '/')
					continue;

				var sb = new StringBuilder(Prefix.Length + (path.Length - i)).Append(Prefix.Length).Append(path, i, path.Length - i);

				sb.Replace('\\', '/', Prefix.Length, sb.Length - 2);

				return sb.ToString();
			}
		}

		return Prefix;
	}
}
