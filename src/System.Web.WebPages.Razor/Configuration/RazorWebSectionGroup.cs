// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace System.Web.WebPages.Razor.Configuration;

/// <summary>Provides configuration system support for the <c>system.web.webPages.razor</c> configuration section.</summary>
public class RazorWebSectionGroup : ConfigurationSectionGroup
{
	internal const string GroupElementName = "system.web.webPages.razor";

	/// <summary>Represents the name of the configuration section for Razor Web section. Contains the static, read-only string <c>"system.web.webPages.razor"</c>.</summary>
	public static readonly string GroupName = GroupElementName;

	// Use flags instead of null values since tests may want to set the property to null
	private bool _hostSet = false;
	private bool _pagesSet = false;

	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private HostSection _host;
	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private RazorPagesSection _pages;

	/// <summary>Gets or sets the <c>host</c> value for <c>system.web.webPages.razor</c> section group.</summary>
	/// <value>The host value.</value>
	[ConfigurationProperty(HostSection.SectionElementName, IsRequired = false)]
	public HostSection Host
	{
		get { return _hostSet ? _host : (HostSection)Sections[HostSection.SectionElementName]; }
		set
		{
			_host = value;
			_hostSet = true;
		}
	}

	/// <summary>Gets or sets the value of the <c>pages</c> element for the <c>system.web.webPages.razor</c> section.</summary>
	/// <value>The pages element value.</value>
	[ConfigurationProperty(RazorPagesSection.SectionElementName, IsRequired = false)]
	public RazorPagesSection Pages
	{
		get { return _pagesSet ? _pages : (RazorPagesSection)Sections[RazorPagesSection.SectionElementName]; }
		set
		{
			_pages = value;
			_pagesSet = true;
		}
	}
}
