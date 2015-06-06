using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class ArrayTypeNode : TypeNode
	{
        // type of elements of this arrays
        public TypeNode ElementType { get; set; } // set during AST construction

        // TODO: arrays implementation - currently all arrays are initialized as dynamic (not fixed sized)
        public bool IsFixedSize { get; set; } // set during AST construction
        //public int Length { get; set; } // TODO: arrays implementation - currently lengths of arrays are not defined
        // there is only expression which can represent length
        public ExpressionNode LengthExpression { get; set; } // set during AST construction

        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // no work to do here
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
            var rhs = (ArrayTypeNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(ElementType, rhs.ElementType) &&
                object.Equals(IsFixedSize, rhs.IsFixedSize) &&
                object.Equals(LengthExpression, rhs.LengthExpression);
        }
	}
}

