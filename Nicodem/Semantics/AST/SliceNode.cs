using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class SliceNode : ExpressionNode
	{
        public ExpressionNode Array { get; set; } // set during AST construction
        public ExpressionNode Left { get; set; } // set during AST construction
        public ExpressionNode Right { get; set; } // set during AST construction

		public bool HasLeft { get { return !ReferenceEquals (Left, null); } }
		public bool HasRight { get { return !ReferenceEquals (Right, null); } }
        
        #region implemented abstract members of Node

        public static SliceNode CreateSliceNode(ExpressionNode array, ExpressionNode from, ExpressionNode to)
        {
            var res = new SliceNode();
            res.Array = array;
            res.Left = from;
            res.Right = to;
            return res;
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

        public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override bool Compare(object rhs_)
        {
            var rhs = (SliceNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Array, rhs.Array) &&
                object.Equals(Left, rhs.Left) &&
                object.Equals(Right, rhs.Right);
        }
	}
}

