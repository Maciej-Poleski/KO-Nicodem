using System.Collections.Generic;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class BlockExpressionNode : ExpressionNode
	{
		public IEnumerable<ExpressionNode> Elements { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

