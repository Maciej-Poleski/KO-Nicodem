using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	class FunctionNode : Node
	{
		public string Name { get; set; }
		public IEnumerable<ParameterNode> Parameters { get; set; }
		public TypeNode Type { get; set; }
		public ExpressionNode Body { get; set; }
	}
}

