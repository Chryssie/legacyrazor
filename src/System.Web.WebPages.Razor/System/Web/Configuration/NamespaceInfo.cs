// https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/Configuration/NamespaceInfo.cs
//------------------------------------------------------------------------------
// <copyright file="NamespaceInfo.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Configuration;

namespace System.Web.Configuration;

public interface IReadOnlyNamespaceInfo
{
	string Namespace { get; }
}

public interface INamespaceInfo : IReadOnlyNamespaceInfo
{
	new string Namespace { get; set; }
}

public sealed class NamespaceInfo : ConfigurationElement, INamespaceInfo
{
	private static readonly ConfigurationProperty _propNamespace = new(
		name: "namespace",
		type: typeof(string),
		defaultValue: null,
		typeConverter: null,
		validator: new StringValidator(1),
		options: ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey
	);

	private static readonly ConfigurationPropertyCollection _properties = [_propNamespace];

	internal NamespaceInfo()
		: base() { }

	public NamespaceInfo(string name)
		: this() => Namespace = name;

	public override bool Equals(object namespaceInformation)
		=> namespaceInformation is NamespaceInfo ns && Namespace == ns.Namespace;

	public override int GetHashCode()
		=> Namespace.GetHashCode();

	protected override ConfigurationPropertyCollection Properties => _properties;

	[ConfigurationProperty("namespace", IsRequired = true, IsKey = true, DefaultValue = "")]
	[StringValidator(MinLength = 1)]
	public string Namespace
	{
		get => (string)base[_propNamespace];
		set => base[_propNamespace] = value;
	}
}
