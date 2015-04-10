using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ArrayTypeNode : TypeNode
	{
		public TypeNode ElementType { get; set; }

		public bool IsFixedSize { get; set; }
		public int Length { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

