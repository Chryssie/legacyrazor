// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace System.Web.WebPages.Razor.Configuration;

/// <summary>Provides configuration system support for the host configuration section.</summary>
public class HostSection : ConfigurationSection
{
	internal const string SectionElementName = "host";
	private const string SectionFullName = $"{RazorWebSectionGroup.GroupElementName}/{SectionElementName}";
	internal const string FactoryTypeAttributeName = "factoryType";

	/// <summary>Represents the name of the configuration section for a Razor host environment.</summary>
	public static readonly string SectionName = SectionFullName;

	internal static readonly ConfigurationProperty _typeProperty = new(
		name: FactoryTypeAttributeName,
		type: typeof(string),
		defaultValue: null,
		options: ConfigurationPropertyOptions.IsRequired
	);

	private bool _factoryTypeSet = false;
	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private string _factoryType;

	/// <summary>Gets or sets the host factory.</summary>
	/// <value>The host factory.</value>
	[ConfigurationProperty(FactoryTypeAttributeName, IsRequired = true, DefaultValue = null)]
	public string FactoryType
	{
		get { return _factoryTypeSet ? _factoryType : (string)this[_typeProperty]; }
		set
		{
			_factoryType = value;
			_factoryTypeSet = true;
		}
	}
}
