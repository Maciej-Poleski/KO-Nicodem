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


        //temporary only elementType differ

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is ArrayTypeNode)
            {
                if (this.ElementType.Equals(((ArrayTypeNode)obj).ElementType))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            //ELemenent type + isFixedSize + Length Expression
            return this.ElementType.GetHashCode();
        }


        public static ArrayTypeNode MakeType(TypeNode typeNode)
        {
            var _ret = new ArrayTypeNode();
            _ret.ElementType = typeNode;
            return _ret;
        }

        public static ArrayTypeNode BoolType()
        {
            return MakeType(NamedTypeNode.BoolType());
        }

        public static ArrayTypeNode ByteType()
        {
            return MakeType(NamedTypeNode.ByteType());
        }

        public static ArrayTypeNode CharType()
        {
            return MakeType(NamedTypeNode.CharType());
        }

        public static ArrayTypeNode IntType()
        {
            return MakeType(NamedTypeNode.IntType());
        }

        public static ArrayTypeNode VoidType()
        {
            return MakeType(NamedTypeNode.VoidType());
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

