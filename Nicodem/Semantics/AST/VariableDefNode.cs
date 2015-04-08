using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class VariableDefNode : ExpressionNode
	{
		public string Name { get; set; }
		public TypeNode VariableType { get; set; }
		public ExpressionNode Value { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

