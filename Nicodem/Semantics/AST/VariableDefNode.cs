using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class VariableDefNode : ExpressionNode
	{
		public string Name { get; set; }
		public TypeNode VariableType { get; set; }
		public ExpressionNode Value { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

