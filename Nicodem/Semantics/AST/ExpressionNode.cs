using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	abstract class ExpressionNode : Node
	{
		public TypeNode ExpressionType { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

