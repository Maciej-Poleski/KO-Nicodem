using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class NamedTypeNode : TypeNode
	{
		public string Name { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

