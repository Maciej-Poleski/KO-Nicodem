using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	abstract class TypeNode : Node
	{
		public bool IsConstant { get; set; }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            throw new System.NotImplementedException();
        }

        public static TypeNode GetTypeNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            // TODO: how to get TYPE node?
            throw new System.NotImplementedException();
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

