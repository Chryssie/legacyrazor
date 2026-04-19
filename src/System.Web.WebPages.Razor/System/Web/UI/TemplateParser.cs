//------------------------------------------------------------------------------
// <copyright file="TemplateParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

/*
 * Implements the ASP.NET template parser
 *
 * Copyright (c) 1998 Microsoft Corporation
 */
namespace System.Web.UI;

/*
 * Base class for classes that contain source file & line information for error reporting
 */
internal abstract class SourceLineInfo // https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/UI/TemplateParser.cs#L3013-L3032
{

	// Source file where the information appears
	internal string VirtualPath { get; set; }

	// Line number in the source file where the information appears
	internal int Line { get; set; }
}

// https://github.com/microsoft/referencesource/blob/ec9fa9ae770d522a5b5f0607898044b7478574a3/System.Web/UI/TemplateParser.cs#L3063-L3079
/*
 * Entry representing an import directive.
 * e.g. <%@ import namespace="System.Web.UI" %>
 */
internal sealed class NamespaceEntry : SourceLineInfo 
{
	internal string Namespace { get; set; }
}