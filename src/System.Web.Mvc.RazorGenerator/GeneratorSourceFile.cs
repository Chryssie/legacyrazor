using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace System.Web.Mvc;

public abstract class GeneratorSourceFile : AdditionalText
{
	public abstract string RelativePath { get; }
}

public static class AdditionalTextExtensions
{
	extension(AdditionalText additionalText)
	{
		public GeneratorSourceFile ToGeneratorSourceFile(string relativePath)
		{
			ArgumentNullException.ThrowIfNull(additionalText);
			ArgumentNullException.ThrowIfNull(relativePath);

			return new ConcreteGeneratorSourceFile(additionalText, relativePath);
		}
	}
}

file sealed class ConcreteGeneratorSourceFile : GeneratorSourceFile
{

	private readonly AdditionalText additionalText;

	internal ConcreteGeneratorSourceFile(AdditionalText additionalText, string relativePath)
	{
		Contract.Assert(additionalText is not null);
		Contract.Assert(relativePath is not null);

		this.additionalText = additionalText is ConcreteGeneratorSourceFile other ? other.additionalText : additionalText!;
		this.RelativePath = relativePath!;
	}

	public override string Path => this.additionalText.Path;
	public override string RelativePath { get; }

	public override SourceText? GetText(CancellationToken cancellationToken = default) => this.additionalText.GetText(cancellationToken);

	public override bool Equals(object obj)
		=> this == obj || (obj is GeneratorSourceFile other && additionalText.Equals(other) && this.RelativePath == other.RelativePath);
	public override int GetHashCode()
	{
		var hashCode = this.additionalText.GetHashCode();

		if (this.RelativePath is { Length: not 0, } relativePath)
			hashCode = HashCode.Combine(hashCode, relativePath.GetHashCode());

		return hashCode;
	}

	public override string ToString() => $"RelativePath = {this.RelativePath}, {this.additionalText}";
}