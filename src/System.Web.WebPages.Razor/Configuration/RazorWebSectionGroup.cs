// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Configuration;

namespace System.Web.WebPages.Razor.Configuration;

public interface IReadOnlyRazorWebSectionGroup
{
	IReadOnlyHostSection Host { get; }
	IReadOnlyRazorPagesSection Pages { get; }
}

public interface IReadOnlyRazorWebSectionGroup<out THostSection, out TRazorPagesSection> : IReadOnlyRazorWebSectionGroup
	where THostSection : IReadOnlyHostSection
	where TRazorPagesSection : IReadOnlyRazorPagesSection
{
	new THostSection Host { get; }
	new TRazorPagesSection Pages { get; }
}

public interface IRazorWebSectionGroup : IReadOnlyRazorWebSectionGroup
{
	new IReadOnlyHostSection Host { get; set; }
	new IReadOnlyRazorPagesSection Pages { get; set; }

	IReadOnlyHostSection GetOrInitHost();
	IReadOnlyRazorPagesSection GetOrInitPages();
}

public interface IRazorWebSectionGroup<THostSection, TRazorPagesSection> : IRazorWebSectionGroup, IReadOnlyRazorWebSectionGroup<THostSection, TRazorPagesSection>
	where THostSection : IReadOnlyHostSection
	where TRazorPagesSection : IReadOnlyRazorPagesSection
{
	new THostSection Host { get; set; }
	new TRazorPagesSection Pages { get; set; }
}

public class RazorWebSectionGroup : ConfigurationSectionGroup, IRazorWebSectionGroup<HostSection, RazorPagesSection>
{
	public static readonly string GroupName = "system.web.webPages.razor";

	// Use flags instead of null values since tests may want to set the property to null
	private bool _hostSet = false;
	private bool _pagesSet = false;

	[ConfigurationProperty("host", IsRequired = false)]
	public HostSection Host
	{
		get { return _hostSet ? field : (HostSection)Sections["host"]; }
		set
		{
			field = value;
			_hostSet = true;
		}
	}

	[ConfigurationProperty("pages", IsRequired = false)]
	public RazorPagesSection Pages
	{
		get { return _pagesSet ? field : (RazorPagesSection)Sections["pages"]; }
		set
		{
			field = value;
			_pagesSet = true;
		}
	}

	IReadOnlyHostSection IRazorWebSectionGroup.Host
	{
		get => ((IReadOnlyRazorWebSectionGroup)this).Host;
		set => this.Host = (HostSection)value;
	}

	IReadOnlyHostSection IReadOnlyRazorWebSectionGroup.Host => this.Host;

	IReadOnlyRazorPagesSection IRazorWebSectionGroup.Pages
	{
		get => ((IReadOnlyRazorWebSectionGroup)this).Pages;
		set => this.Pages = (RazorPagesSection)value;
	}

	IReadOnlyRazorPagesSection IReadOnlyRazorWebSectionGroup.Pages => this.Pages;

	IReadOnlyHostSection IRazorWebSectionGroup.GetOrInitHost()
	{
		throw new NotImplementedException();
	}

	IReadOnlyRazorPagesSection IRazorWebSectionGroup.GetOrInitPages()
	{
		throw new NotImplementedException();
	}
}
