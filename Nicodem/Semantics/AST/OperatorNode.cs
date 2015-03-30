using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class OperatorNode : OperationNode
	{
		public OperatorType Operator { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

