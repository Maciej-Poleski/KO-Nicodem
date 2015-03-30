using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ParameterNode : Node
	{
		public string Name { get; set; }
		public TypeNode Type { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

