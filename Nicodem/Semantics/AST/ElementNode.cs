using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ElementNode : ExpressionNode
	{
		public ExpressionNode Array { get; set; }
		public ExpressionNode Index { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

