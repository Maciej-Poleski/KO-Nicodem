using Nicodem.Semantics.Visitors;
using System.Collections.Generic;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class FunctionCallNode : ExpressionNode
	{
		public string Name { get; set; }
		public IEnumerable<ExpressionNode> Arguments { get; set; }
        public FunctionNode Definition { get; set; }

        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            throw new System.NotImplementedException();
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

