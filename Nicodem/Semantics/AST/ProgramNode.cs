using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	class ProgramNode : Node
	{
		public IEnumerable<FunctionNode> Functions { get; set; }
	}
}

