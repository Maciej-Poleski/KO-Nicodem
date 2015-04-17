using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class WhileNode : ExpressionNode
	{
		public ExpressionNode Condition { get; set; }
		public ExpressionNode Body { get; set; }
		public ExpressionNode Else { get; set; }

		public bool HasElse { get { return !ReferenceEquals (Else, null); } }
        
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

