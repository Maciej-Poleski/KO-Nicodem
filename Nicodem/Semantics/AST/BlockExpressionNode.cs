using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	class BlockExpressionNode : ExpressionNode
	{
		public IEnumerable<ExpressionNode> Elements { get; set; }
	}
}

