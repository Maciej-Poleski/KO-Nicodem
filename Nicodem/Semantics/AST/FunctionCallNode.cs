using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class FunctionCallNode : ExpressionNode
	{
		public string Name { get; set; }
		public ExpressionNode Arguments { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

