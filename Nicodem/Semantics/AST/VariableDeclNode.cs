using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using System.Diagnostics;
using System.Linq;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class VariableDeclNode : ExpressionNode
	{
        public string Name { get; set; } // set during AST construction
        public TypeNode Type { get; set; } // set during AST construction

        public bool NestedUse { get; set; }
        
        #region implemented abstract members of Node

        // ObjectDeclaration.SetProduction(TypeSpecifier * ObjectName);
        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var node = ASTBuilder.AsBranch(parseTree);
            IParseTree<TSymbol>[] childs = node.Children.ToArray();
            // type name
            Debug.Assert(childs.Length == 2); 
            Type = TypeNode.GetTypeNode(childs[0]); // type - arg 0
            Name = childs[1].Fragment.GetOriginText(); // name - arg 1
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
            var rhs = (VariableDeclNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Name, rhs.Name) &&
                object.Equals(Type, rhs.Type) &&
                object.Equals(NestedUse, rhs.NestedUse);
        }
	}
}

