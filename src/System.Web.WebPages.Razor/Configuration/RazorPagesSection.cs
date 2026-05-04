// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web.Configuration;

namespace System.Web.WebPages.Razor.Configuration;

/// <summary>Provides configuration system support for the pages configuration section.</summary>
public class RazorPagesSection : ConfigurationSection
{
	internal const string SectionElementName = "pages";

	private const string SectionFullName = $"{RazorWebSectionGroup.GroupElementName}/{SectionElementName}";

	/// <summary>Represents the name of the configuration section for Razor pages.</summary>
	public static readonly string SectionName = SectionFullName;

	internal const string PageBaseTypeAttributeName = "pageBaseType";
	internal const string NamespacesElementName = "namespaces";

	internal static readonly ConfigurationProperty _pageBaseTypeProperty = new(
		PageBaseTypeAttributeName,
		typeof(string),
		null,
		ConfigurationPropertyOptions.IsRequired
	);
	internal static readonly ConfigurationProperty _namespacesProperty = new(
		NamespacesElementName,
		typeof(NamespaceCollection),
		null,
		ConfigurationPropertyOptions.IsRequired
	);

	private bool _pageBaseTypeSet = false;
	private bool _namespacesSet = false;

	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private string _pageBaseType;
	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private NamespaceCollection _namespaces;

	/// <summary>Gets or sets the name of the page base type class.</summary>
	/// <value>The name of the page base type class.</value>
	[ConfigurationProperty(PageBaseTypeAttributeName, IsRequired = true)]
	public string PageBaseType
	{
		get => _pageBaseTypeSet ? _pageBaseType : (string)this[_pageBaseTypeProperty];
		set
		{
			_pageBaseType = value;
			_pageBaseTypeSet = true;
		}
	}

	/// <summary>Gets or sets the collection of namespaces to add to Web Pages pages in the current application.</summary>
	/// <value>The collection of namespaces.</value>
	[ConfigurationProperty(NamespacesElementName, IsRequired = true)]
	[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Being able to set this property is extremely useful for third-parties who are testing components which interact with the Razor configuration system")]
	public NamespaceCollection Namespaces
	{
		get => _namespacesSet ? _namespaces : (NamespaceCollection)this[_namespacesProperty];
		set
		{
			_namespaces = value;
			_namespacesSet = true;
		}
	}
}
