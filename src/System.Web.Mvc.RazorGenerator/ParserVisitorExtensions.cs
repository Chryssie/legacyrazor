using System.Web.Razor.Parser;
using System.Web.Razor.Parser.SyntaxTree;

namespace System.Web.Mvc;

internal static class ParserVisitorExtensions
{
	public static void Visit(this ParserVisitor visitor, SyntaxTreeNode node)
	{
		ArgumentNullException.ThrowIfNull(visitor);
		ArgumentNullException.ThrowIfNull(node);

		node.Accept(visitor);
		visitor.OnComplete();
	}
}
