using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
    // FIXME: change name to more descriptive - ArrayElementAccess
	class ElementNode : ExpressionNode
	{
        public ExpressionNode Array { get; set; } // set during AST construction
        public ExpressionNode Index { get; set; } // set during AST construction
        
        #region implemented abstract members of Node

        public static ElementNode Create(ExpressionNode array, ExpressionNode index)
        {
            var res = new ElementNode();
            res.Array = array;
            res.Index = index;
            return res;
        }

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // not used during AST construction
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

        protected override bool Compare(object rhs_)
        {
            var rhs = (ElementNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Array, rhs.Array) &&
                object.Equals(Index, rhs.Index);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix)
                + PrintVar(prefix, "Array", Array)
                + PrintVar(prefix, "Index", Index);
        }
	}
}

