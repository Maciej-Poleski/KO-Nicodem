using Nicodem.Semantics.Visitors;
using System.Collections.Generic;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class FunctionCallNode : ExpressionNode
	{
		public string Name { get; set; }
		public IReadOnlyList<ExpressionNode> Arguments { get; set; }
        public FunctionDefinitionNode Definition { get; set; }

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

        public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
	}
}

