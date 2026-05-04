// https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/UI/TemplateParser.cs

#nullable disable

using System.Diagnostics.CodeAnalysis;

namespace System.Web.UI;

/// <summary>Base class for classes that contain source file & line information for error reporting</summary>
internal abstract class SourceLineInfo // https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/UI/TemplateParser.cs#L3013-L3032
{
	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private string _virtualPath;

	/// <summary>Source file where the information appears</summary>
	public string VirtualPath
	{
		get => _virtualPath;
		set => _virtualPath = value;
	}

	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private int _line;
	/// <summary>Line number in the source file where the information appears</summary>
	public int Line
	{
		get => _line;
		set => _line = value;
	}
}

/// <summary>
/// Entry representing an import directive. e.g. <c>&lt;%@ import namespace="System.Web.UI" %&gt;</c>
/// </summary>
internal sealed class NamespaceEntry : SourceLineInfo // https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/UI/TemplateParser.cs#L3063-L3079
{
	[SuppressMessage("Style", "IDE0032:Use auto property", Justification = "Binary compatibility.")]
	private string _namespace;

	internal NamespaceEntry() { }

	public string Namespace
	{
		get => _namespace;
		set => _namespace = value;
	}
}