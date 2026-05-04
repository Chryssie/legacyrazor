// https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/Configuration/NamespaceInfo.cs

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace System.Web.Configuration;

/// <summary>Contains a single configuration namespace reference, similar to the <c>Import</c> directive. This class cannot be inherited.</summary>
/// <remarks>The <see cref="NamespaceCollection"/> contains <see cref="NamespaceInfo"/> objects, which correspond to the <c>add</c> elements within the <c>namespaces</c> section. Each <see cref="NamespaceInfo"/> object is the same as an <c>Import</c> (<c>&lt;%@ Import %&gt;</c>) directive that applies to all pages and controls in the scope of the configuration file. The <c>Import</c> directive allows you to import a namespace into your ASP.NET page, making all its classes available for use on your page.</remarks>
public sealed class NamespaceInfo : ConfigurationElement
{
	internal const string NamespaceAttributeName = "namespace";

	internal static readonly ConfigurationProperty _propNamespace = new(
		name: NamespaceAttributeName,
		type: typeof(string),
		defaultValue: null,
		typeConverter: null,
		validator: new StringValidator(1),
		options: ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey
	);

	private static readonly ConfigurationPropertyCollection _properties = [_propNamespace];

	internal NamespaceInfo()
		: base() { }

	/// <summary>Initializes a new instance of the NamespaceInfo class with the specified namespace reference.</summary>
	/// <param name="name">A namespace reference for the new <see cref="NamespaceInfo"/> object.</param>
	/// <remarks>No validation is performed to verify that the namespace reference is valid.</remarks>
	public NamespaceInfo(string name)
		: this() => Namespace = name;

#nullable enable
	/// <summary>Compares the current instance to the passed <see cref="NamespaceInfo"/> object.</summary>
	/// <param name="namespaceInformation">A <see cref="NamespaceInfo"/> object to compare to.</param>
	/// <returns><see langword="true"/> if the two objects are identical.</returns>
	public override bool Equals([NotNullWhen(true)] object? namespaceInformation)
		=> namespaceInformation is NamespaceInfo other && this.Namespace == other.Namespace;
#nullable restore

	/// <summary>Returns a hash value for the current instance.</summary>
	/// <returns>A hash value for the current instance.</returns>
	public override int GetHashCode()
		=> Namespace.GetHashCode();

	protected override ConfigurationPropertyCollection Properties => _properties;

	/// <summary>Gets or sets the namespace reference.</summary>
	/// <value>A string that specifies the name of the namespace.</value>
	/// <remarks>No validation is performed to verify that the namespace reference is valid.</remarks>
	[ConfigurationProperty(NamespaceAttributeName, IsRequired = true, IsKey = true, DefaultValue = "")]
	[StringValidator(MinLength = 1)]
	public string Namespace
	{
		get => (string)base[_propNamespace];
		set => base[_propNamespace] = value;
	}
}
