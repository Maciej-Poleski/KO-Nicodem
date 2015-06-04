using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class NamedTypeNode : TypeNode
	{
        public string Name { get; set; } // set during AST construction

        public static NamedTypeNode VoidType(){
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "void";
            return node;
        }

        public static NamedTypeNode BoolType()
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "bool";
            return node;
        }

        public static NamedTypeNode IntType()
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "int";
            return node;
        }

        public static NamedTypeNode CharType()
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "char";
            return node;
        }

        public static NamedTypeNode ByteType()
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "byte";
            return node;
        }

        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            Name = parseTree.Fragment.GetOriginText();
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

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is NamedTypeNode)
            {
                if (this.Name.Equals(((NamedTypeNode)obj).Name))
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
	}
}

