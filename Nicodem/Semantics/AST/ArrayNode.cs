using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	class ArrayNode : ConstNode
	{
		public IEnumerable<ExpressionNode> Elements { get; set; }

		public ArrayNode( TypeNode type ) : base( type )
		{
		}
	}
}

