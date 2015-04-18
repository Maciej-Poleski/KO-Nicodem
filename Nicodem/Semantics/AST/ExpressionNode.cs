using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	abstract class ExpressionNode : Node
	{
		public TypeNode ExpressionType { get; set; }
        
        #region implemented abstract members of Node

        public static ExpressionNode GetExpressionNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            // TODO: how to get EXPRESSION node?
            throw new System.NotImplementedException();
        }

        public static int EvalExpression<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            //TODO: implement compile time constant expression evaluation!
            return 0;
        }

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

