using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ParameterNode : Node
	{
		public string Name { get; set; }
		public TypeNode Type { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

