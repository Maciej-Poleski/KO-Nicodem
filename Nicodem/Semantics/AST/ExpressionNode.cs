using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	abstract class ExpressionNode : Node
	{
		public TypeNode ExpressionType { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

