// https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/Configuration/NamespaceCollection.cs

using System.Collections;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Web.UI;

namespace System.Web.Configuration;

/// <summary>Contains a collection of namespace objects. This class cannot be inherited.</summary>
/// <remarks>The <see cref="NamespaceCollection"/> contains <see cref="NamespaceInfo"/> objects. Each <see cref="NamespaceInfo"/> object is the same as an <c>Import</c> (<c>&lt;%@ Import %&gt;</c>) directive that applies to all pages and controls in the scope of the configuration file. The <c>Import</c> directive allows you to import a namespace into your ASP.NET page, making all its classes available for use on your page.</remarks>
[ConfigurationCollection(typeof(NamespaceInfo))]
public sealed class NamespaceCollection : ConfigurationElementCollection
{
	internal const string AutoImportVBNamespaceAttributeName = "autoImportVBNamespace";

	internal static readonly ConfigurationProperty _propAutoImportVBNamespace = new(
		name: AutoImportVBNamespaceAttributeName,
		type: typeof(bool),
		defaultValue: true,
		options: ConfigurationPropertyOptions.None
	);

	private static readonly ConfigurationPropertyCollection _properties = [_propAutoImportVBNamespace];

	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private Hashtable? _namespaceEntries;

	protected override ConfigurationPropertyCollection Properties => _properties;

	/// <summary>Gets or sets a value that determines whether the Visual Basic namespace is imported without having to specify it.</summary>
	/// <value><see langword="true"/> if the Visual Basic namespace is imported automatically; otherwise, <see langword="false"/>. The default is <see langword="true"/>.</value>
	/// <remarks>For ASP.NET pages written using Visual Basic the <a href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualbasic?view=netframework-4.8.1">Microsoft.VisualBasic</a> namespace is automatically imported unless the <see cref="AutoImportVBNamespace"/> property is <see langword="false"/>.</remarks>
	[ConfigurationProperty(AutoImportVBNamespaceAttributeName, DefaultValue = true)]
	public bool AutoImportVBNamespace
	{
		get => (bool)base[_propAutoImportVBNamespace];
		set => base[_propAutoImportVBNamespace] = value;
	}

	/// <summary>Gets or sets the <see cref="NamespaceInfo"/> object at the specified index in the collection.</summary>
	/// <param name="index">The index of a <see cref="NamespaceInfo"/> object in the collection.</param>
	/// <value><see cref="NamespaceInfo"/> object at the specified index, or <see langword="null"/> if there is no object at that index.</value>
	/// <remarks>This property overwrites the <see cref="NamespaceInfo"/> object if it already exists at the specified index; otherwise, a new object is created and added to the collection.</remarks>
	public NamespaceInfo this[int index]
	{
		get => (NamespaceInfo)BaseGet(index);
		set
		{
			if (BaseGet(index) != null)
				BaseRemoveAt(index);

			BaseAdd(index, value);
			_namespaceEntries = null;
		}
	}

	/// <summary>Adds a <see cref="NamespaceInfo"/> object to the collection.</summary>
	/// <param name="namespaceInformation">A <see cref="NamespaceInfo"/> object to add to the collection.</param>
	/// <exception cref="ConfigurationException">The <see cref="NamespaceInfo"/> object to add already exists in the collection or the collection is read-only.</exception>
	/// <remarks>The collection must not already contain a <see cref="NamespaceInfo"/> object with the same <see cref="NamespaceInfo.Namespace"/> property value.</remarks>
	public void Add(NamespaceInfo namespaceInformation)
	{
		BaseAdd(namespaceInformation);
		_namespaceEntries = null;
	}

	/// <summary>Removes the <see cref="NamespaceInfo"/> object with the specified key from the collection.</summary>
	/// <param name="s">The namespace of a <see cref="NamespaceInfo"/> object to remove from the collection.</param>
	/// <exception cref="ConfigurationException">There is no <see cref="NamespaceInfo"/> object with the specified key in the collection. -or- The element has already been removed. -or- The collection is read-only.</exception>
	/// <remarks>If the specified element is defined in a higher-level configuration file, this method inserts a <c>remove</c> element into the appropriate section of the configuration file at the level of the current application. If the element is defined in the current configuration file, its entry is removed from the configuration file. The object to remove must exist in the collection; if it does not, a <see cref="ConfigurationException"/> is thrown.</remarks>
	public void Remove(string s)
	{
		BaseRemove(s);
		_namespaceEntries = null;
	}

	/// <summary>Removes a <see cref="NamespaceInfo"/> object from the specified index in the collection.</summary>
	/// <param name="index">The index of a <see cref="NamespaceInfo"/> object to remove from the collection.</param>
	/// <exception cref="ConfigurationException">There is no <see cref="NamespaceInfo"/> object at the specified index in the collection. -or- The element has already been removed. -or- The collection is read-only.</exception>
	/// <remarks>If the specified element is defined in a higher-level configuration file, this method inserts a <c>remove</c> element into the appropriate section of the configuration file at the level of the current application. If the element is defined in the current configuration file, its entry is removed from the configuration file. The object to remove must exist in the collection; if it does not, a <see cref="ConfigurationException"/> is thrown.</remarks>
	public void RemoveAt(int index)
	{
		BaseRemoveAt(index);
		_namespaceEntries = null;
	}

	protected override ConfigurationElement CreateNewElement() => new NamespaceInfo();
	protected override object GetElementKey(ConfigurationElement element)
		=> ((NamespaceInfo)element).Namespace;

	/// <summary>Removes all <see cref="NamespaceInfo"/> objects from the collection.</summary>
	/// <remarks>This method empties the collection and inserts a <c>clear</c> element into the appropriate section of the configuration file to remove all references to elements defined in higher-level configuration files and in the current configuration file.</remarks>
	public void Clear()
	{
		BaseClear();
		_namespaceEntries = null;
	}

	[AllowNull]
	internal Hashtable NamespaceEntries
	{
		get
		{
			return _namespaceEntries ?? InitNamespaceEntries();

			Hashtable InitNamespaceEntries()
			{
				Hashtable? value;

				lock (this)
				{
					value = _namespaceEntries;

					if (value is null)
					{
						value = new(StringComparer.OrdinalIgnoreCase);

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

							value[ni.Namespace] = namespaceEntry;
						}

						_namespaceEntries = value;
					}
				}

				return value;
			}
		}
	}
}
