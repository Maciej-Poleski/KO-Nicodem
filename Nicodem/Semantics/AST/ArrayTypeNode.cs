using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class ArrayTypeNode : TypeNode
	{
		public TypeNode ElementType { get; set; } // type of elements of this arrays

		public bool IsFixedSize { get; set; } // TODO: currently all arrays are initialized as dynamic (not fixed sized)
		//public int Length { get; set; } // TODO: currently lengths of arrays are not defined
        public ExpressionNode LengthExpression { get; set; }

        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // no work to do here
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

