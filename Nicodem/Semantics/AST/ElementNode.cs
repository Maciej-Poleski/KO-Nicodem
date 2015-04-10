using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ElementNode : ExpressionNode
	{
		public ExpressionNode Array { get; set; }
		public ExpressionNode Index { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

