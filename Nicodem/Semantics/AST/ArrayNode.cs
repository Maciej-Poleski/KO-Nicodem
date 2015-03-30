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

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

