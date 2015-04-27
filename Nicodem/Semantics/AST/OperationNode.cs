using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;


namespace Nicodem.Semantics.AST
{
    // TODO is this class needed?
	class OperationNode : ExpressionNode
	{
		public IEnumerable<ExpressionNode> Arguments { get; set; }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // nothing to do here
            throw new System.NotImplementedException();
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

