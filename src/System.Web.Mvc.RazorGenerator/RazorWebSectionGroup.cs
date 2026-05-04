//using System.Collections.Immutable;
//using System.Diagnostics.CodeAnalysis;
//using System.Text;
//using System.Web.Configuration;
//using System.Web.WebPages.Razor.Configuration;
//using System.Xml.Linq;

//namespace System.Web.Mvc;

//public record struct RazorWebSectionGroup : IReadOnlyRazorWebSectionGroup<HostSection, RazorPagesSection>
//{
//	public RazorWebSectionGroup Default => default;

//	public RazorWebSectionGroup ExtendWith(XDocument document)
//	{

//	}

//	public required HostSection Host { readonly get; set; }
//	public required RazorPagesSection Pages { readonly get; set; }

//	readonly IReadOnlyHostSection IReadOnlyRazorWebSectionGroup.Host => this.Host;
//	readonly IReadOnlyRazorPagesSection IReadOnlyRazorWebSectionGroup.Pages => this.Pages;
//}

//public record struct HostSection : IHostSection
//{
//	public string FactoryType { readonly get; set; }

//	public RazorWebSectionGroup ExtendWith(XDocument document)
//	{

//	}

//	internal RazorWebSectionGroup ExtendWithCore(XElement group)
//	{
//		var hostElement = group.Element("host");
//	}
//}

//public record struct RazorPagesSection : IReadOnlyRazorPagesSection<NamespaceCollection>
//{
//	public required string PageBaseType { readonly get; set; }
//	public required NamespaceCollection Namespaces { readonly get; set; }

//	IReadOnlyNamespaceCollection IReadOnlyRazorPagesSection.Namespaces => this.Namespaces;

//	public RazorWebSectionGroup ExtendWith(XDocument document)
//	{

//	}

//	internal RazorWebSectionGroup ExtendWithCore(XElement group)
//	{
//		var hostElement = group.Element("pages");
//	}
//}

//public record struct NamespaceCollection : IReadOnlyNamespaceCollection
//{
//	readonly int IReadOnlyNamespaceCollection.Count => this.Namespaces.Count;
//	readonly string IReadOnlyNamespaceCollection.this[int index] => this.Namespaces[index];

//	public required bool AutoImportVBNamespace { readonly get; set; }
//	public NamespaceNamesCollection Namespaces { readonly get; set; }

//	public readonly bool Equals(NamespaceCollection other) => this.AutoImportVBNamespace == other.AutoImportVBNamespace && Equals(this.Namespaces, other.Namespaces);
//	public override readonly int GetHashCode()
//	{
//		var hashCode = this.Namespaces.GetHashCode();

//		if (this.AutoImportVBNamespace)
//			hashCode = ~hashCode;

//		return hashCode;
//	}

//	internal readonly NamespaceCollection ExtendWithCore(XElement group)
//	{
//		var hostElement = group.Element("namespaces");

		
//	}


//	[SuppressMessage("Intellisense", "IDE0051: Remove unused private members", Justification = "False-positive.")]
//	private readonly bool PrintMembers(StringBuilder sb)
//	{
//		sb
//			.Append(nameof(AutoImportVBNamespace)).Append(" = ").Append(this.AutoImportVBNamespace)
//			.Append(", ")
//			.Append(nameof(Namespaces)).Append(" = ");

//		this.Namespaces.AppendTo(sb);

//		return true;
//	}

//	private static bool Equals(ImmutableArray<string> x, ImmutableArray<string> y)
//	{
//		if (x == y)
//			return true;

//		if (x.IsDefault || y.IsDefault)
//			return false;

//		return x.AsSpan().SequenceEqual(y.AsSpan());
//	}

//	private static int GetHashCode(ImmutableArray<string> obj)
//	{
//		if (obj.IsDefault)
//			return 0;

//		var hashCode = new HashCode();
//		var i = 0;

//		do
//		{
//			var ns = obj[i];
//			hashCode.Add(string.IsNullOrEmpty(ns) ? 0 : ns.GetHashCode());
//		} while (unchecked((uint)++i < (uint)obj.Length));

//		return hashCode.ToHashCode();
//	}

//	private static void PrintValues(StringBuilder sb, ImmutableArray<string> namespaces)
//	{
//		if (namespaces.IsDefault)
//		{
//			sb.Append("null");
//		}
//		else
//		{
//			sb.Append('[');

//			for (var i = 0; unchecked((uint)i < (uint)namespaces.Length); i++)
//			{
//				if (i != 0)
//					sb.Append(',');

//				sb.Append(' ');
//				sb.Append(namespaces[i]);
//			}

//			sb.Append(' ').Append(']');
//		}
//	}
//}
