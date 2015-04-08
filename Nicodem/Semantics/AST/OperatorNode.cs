using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class OperatorNode : OperationNode
	{
		public OperatorType Operator { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

