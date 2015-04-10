using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	abstract class TypeNode : Node
	{
		public bool IsConstant { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

