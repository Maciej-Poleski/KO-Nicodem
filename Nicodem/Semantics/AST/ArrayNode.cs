using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
    // TODO: how to use this?
	class ArrayNode : ConstNode
	{
		public IEnumerable<ExpressionNode> Elements { get; set; }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        public ArrayNode(TypeNode type) : base(type) { }

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}

        public override SubExpressionGraph Accept(ReturnedAbstractVisitor<SubExpressionGraph> visitor)
        {
            return visitor.Visit(this);
        }
	}
}

