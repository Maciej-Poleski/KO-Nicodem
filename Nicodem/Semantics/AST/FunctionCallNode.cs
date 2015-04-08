using Nicodem.Semantics.Visitors;
using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	class FunctionCallNode : ExpressionNode
	{
		public string Name { get; set; }
		public IEnumerable<ExpressionNode> Arguments { get; set; }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

