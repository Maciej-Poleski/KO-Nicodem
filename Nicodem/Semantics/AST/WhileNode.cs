using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class WhileNode : ExpressionNode
	{
		public ExpressionNode Condition { get; set; }
		public ExpressionNode Body { get; set; }
		public ExpressionNode Else { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

