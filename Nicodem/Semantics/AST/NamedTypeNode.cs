using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class NamedTypeNode : TypeNode
	{
        public string Name { get; set; } // set during AST construction

        public static NamedTypeNode VoidType(bool isConstant = false)
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "void";
            node.IsConstant = isConstant;
            return node;
        }

        public static NamedTypeNode BoolType(bool isConstant=false)
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "bool";
            node.IsConstant = isConstant;
            return node;
        }

        public static NamedTypeNode IntType(bool isConstant=false)
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "int";
            node.IsConstant = isConstant;
            return node;
        }

        public static NamedTypeNode CharType(bool isConstant = false)
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "char";
            node.IsConstant = isConstant;
            return node;
        }

        public static NamedTypeNode ByteType(bool isConstant = false)
        {
            NamedTypeNode node = new NamedTypeNode();
            node.Name = "byte";
            node.IsConstant = isConstant;
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
        
        protected override bool Compare(object rhs_)
        {
            var rhs = (NamedTypeNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Name, rhs.Name);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix) +
                PrintVar(prefix, "Name", Name);
        }
	}
}

