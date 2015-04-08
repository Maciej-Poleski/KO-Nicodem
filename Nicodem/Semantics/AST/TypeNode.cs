using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	abstract class TypeNode : Node
	{
		public bool IsConstant { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

