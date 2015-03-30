using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class VariableUseNode : ExpressionNode
	{
		public string Name { get; set; }
		public VariableDefNode Definition { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

