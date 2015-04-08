using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class NamedTypeNode : TypeNode
	{
		public string Name { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

