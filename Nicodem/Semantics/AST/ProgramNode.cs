using System.Collections.Generic;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ProgramNode : Node
	{
		public IEnumerable<FunctionNode> Functions { get; set; }

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

