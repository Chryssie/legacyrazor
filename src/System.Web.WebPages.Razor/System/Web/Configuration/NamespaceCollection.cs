// https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/Configuration/NamespaceCollection.cs
//------------------------------------------------------------------------------
// <copyright file="NamespaceCollection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using System.Configuration;
using System.Web.UI;

namespace System.Web.Configuration;

public interface IReadOnlyNamespaceCollection
{
	bool AutoImportVBNamespace { get; }

	IReadOnlyNamespaceInfo this[int index] { get; }
	int Count { get; }
}

public interface IReadOnlyNamespaceCollection<out TNamespaceInfo> : IReadOnlyNamespaceCollection
	where TNamespaceInfo : IReadOnlyNamespaceInfo
{
	new TNamespaceInfo this[int index] { get; }
}

public interface INamespaceCollection : IReadOnlyNamespaceCollection
{
	new bool AutoImportVBNamespace { get; set; }

	new IReadOnlyNamespaceInfo this[int index] { get; set; }

	IReadOnlyNamespaceInfo Add(string namespaceInformationName);
	void Add(IReadOnlyNamespaceInfo namespaceInformation);
	void Remove(string s);
	void RemoveAt(int index);
}

public interface INamespaceCollection<TNamespaceInfo> : INamespaceCollection, IReadOnlyNamespaceCollection<TNamespaceInfo>
	where TNamespaceInfo : IReadOnlyNamespaceInfo
{
	new TNamespaceInfo this[int index] { get; set; }

	void Add(TNamespaceInfo namespaceInformation);
	TNamespaceInfo Add(string namespaceInformationName);
}

[ConfigurationCollection(typeof(NamespaceInfo))]
public sealed class NamespaceCollection : ConfigurationElementCollection, INamespaceCollection<NamespaceInfo>
{
	private static readonly ConfigurationProperty _propAutoImportVBNamespace = new(
		"autoImportVBNamespace",
		typeof(bool),
		true,
		ConfigurationPropertyOptions.None
	);
	private static readonly ConfigurationPropertyCollection _properties = [_propAutoImportVBNamespace];

	protected override ConfigurationPropertyCollection Properties => _properties;

	[ConfigurationProperty("autoImportVBNamespace", DefaultValue = true)]
	public bool AutoImportVBNamespace
	{
		get => (bool)base[_propAutoImportVBNamespace];
		set => base[_propAutoImportVBNamespace] = value;
	}

	public NamespaceInfo this[int index]
	{
		get => (NamespaceInfo)BaseGet(index);
		set
		{
			if (BaseGet(index) != null)
				BaseRemoveAt(index);

			BaseAdd(index, value);
			NamespaceEntries = null;
		}
	}
	public void Add(NamespaceInfo namespaceInformation)
	{
		BaseAdd(namespaceInformation);
		NamespaceEntries = null;
	}
	public void Remove(string s)
	{
		BaseRemove(s);
		NamespaceEntries = null;
	}

	public void RemoveAt(int index)
	{
		BaseRemoveAt(index);
		NamespaceEntries = null;
	}

	protected override ConfigurationElement CreateNewElement() => new NamespaceInfo();
	protected override object GetElementKey(ConfigurationElement element)
		=> ((NamespaceInfo)element).Namespace;

	public void Clear()
	{
		BaseClear();
		NamespaceEntries = null;
	}

	internal Hashtable NamespaceEntries
	{
		get
		{
			if (field is null)
				InitNamespaceEntries();

			return field;

			void InitNamespaceEntries()
			{
				lock (this)
				{
					if (field is null)
					{
						field = new(StringComparer.OrdinalIgnoreCase);

						foreach (NamespaceInfo ni in this)
						{
							var namespaceEntry = new NamespaceEntry
							{
								Namespace = ni.Namespace,
								// Remember the config file location info, in case an error
								// occurs later when we use this data
								Line = ni.ElementInformation.Properties["namespace"].LineNumber,
								VirtualPath = ni.ElementInformation.Properties["namespace"].Source
							};

							// If the namespace was given Programactically it needs to still have a
							// valid line number of the compiler chokes (1 based).
							if (namespaceEntry.Line == 0)
								namespaceEntry.Line = 1;

							field[ni.Namespace] = namespaceEntry;
						}
					}
				}
			}
		}
		private set;
	}

	IReadOnlyNamespaceInfo IReadOnlyNamespaceCollection.this[int index] => this[index];

	IReadOnlyNamespaceInfo INamespaceCollection.this[int index]
	{
		get => ((IReadOnlyNamespaceCollection)this)[index];
		set => this[index] = (NamespaceInfo)value;
	}

	void INamespaceCollection.Add(IReadOnlyNamespaceInfo namespaceInformation) => this.Add((NamespaceInfo)namespaceInformation);

	IReadOnlyNamespaceInfo INamespaceCollection.Add(string namespaceInformationName) => ((INamespaceCollection<NamespaceInfo>)this).Add(namespaceInformationName);

	NamespaceInfo INamespaceCollection<NamespaceInfo>.Add(string namespaceInformationName)
	{
		var instance = new NamespaceInfo(namespaceInformationName);
		this.Add(instance);
		return instance;
	}
}
