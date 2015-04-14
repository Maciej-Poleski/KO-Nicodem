using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class NamedTypeNode : TypeNode
	{
		public string Name { get; set; }

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

