using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	class OperationNode : ExpressionNode
	{
		public IEnumerable<ExpressionNode> Arguments { get; set; }
	}
}

