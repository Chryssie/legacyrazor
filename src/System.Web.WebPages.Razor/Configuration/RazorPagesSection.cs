// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Web.Configuration;

namespace System.Web.WebPages.Razor.Configuration;

public interface IReadOnlyRazorPagesSection
{
	string PageBaseType { get; }
	IReadOnlyNamespaceCollection Namespaces { get; }
}

public interface IReadOnlyRazorPagesSection<out TNamespaceCollection> : IReadOnlyRazorPagesSection
	where TNamespaceCollection : IReadOnlyNamespaceCollection
{
	new TNamespaceCollection Namespaces { get; }
}

public interface IRazorPagesSection : IReadOnlyRazorPagesSection
{
	new string PageBaseType { get; set; }
	new IReadOnlyNamespaceCollection Namespaces { get; set; }
}

public interface IRazorPagesSection<TNamespaceCollection> : IRazorPagesSection, IReadOnlyRazorPagesSection<TNamespaceCollection>
	where TNamespaceCollection : IReadOnlyNamespaceCollection
{
	new TNamespaceCollection Namespaces { get; set; }
}

public class RazorPagesSection : ConfigurationSection, IRazorPagesSection<NamespaceCollection>
{
	public static readonly string SectionName = RazorWebSectionGroup.GroupName + "/pages";

	private static readonly ConfigurationProperty _pageBaseTypeProperty = new(
		"pageBaseType",
		typeof(string),
		null,
		ConfigurationPropertyOptions.IsRequired
	);

	private static readonly ConfigurationProperty _namespacesProperty = new(
		"namespaces",
		typeof(NamespaceCollection),
		null,
		ConfigurationPropertyOptions.IsRequired
	);

	private bool _pageBaseTypeSet = false;
	private bool _namespacesSet = false;

	[ConfigurationProperty("pageBaseType", IsRequired = true)]
	public string PageBaseType
	{
		get { return _pageBaseTypeSet ? field : (string)this[_pageBaseTypeProperty]; }
		set
		{
			field = value;
			_pageBaseTypeSet = true;
		}
	}

	[ConfigurationProperty("namespaces", IsRequired = true)]
	[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Being able to set this property is extremely useful for third-parties who are testing components which interact with the Razor configuration system")]
	public NamespaceCollection Namespaces
	{
		get { return _namespacesSet ? field : (NamespaceCollection)this[_namespacesProperty]; }
		set
		{
			field = value;
			_namespacesSet = true;
		}
	}

	IReadOnlyNamespaceCollection IRazorPagesSection.Namespaces
	{
		get => ((IReadOnlyRazorPagesSection)this).Namespaces;
		set => this.Namespaces = (NamespaceCollection)value;
	}

	IReadOnlyNamespaceCollection IReadOnlyRazorPagesSection.Namespaces => this.Namespaces;
}
