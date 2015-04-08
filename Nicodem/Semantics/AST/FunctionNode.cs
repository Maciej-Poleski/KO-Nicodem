using System.Collections.Generic;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class FunctionNode : Node
	{
		public string Name { get; set; }
		public IEnumerable<ParameterNode> Parameters { get; set; }
		public TypeNode Type { get; set; }
		public ExpressionNode Body { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

