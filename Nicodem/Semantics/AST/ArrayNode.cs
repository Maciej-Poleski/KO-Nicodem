using System.Collections.Generic;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.AST
{
	class ArrayNode : ConstNode
	{
		public IEnumerable<ExpressionNode> Elements { get; set; }

		public ArrayNode( TypeNode type ) : base( type )
		{
		}

		public override TResult Accept<TResult> (AbstractVisitor<TResult> visitor)
		{
			return visitor.Visit (this);
		}
	}
}

