using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using System.Diagnostics;
using System.Linq;

namespace Nicodem.Semantics.AST
{
	class ParameterNode : Node
	{
		public string Name { get; set; }
		public TypeNode Type { get; set; }
        
        #region implemented abstract members of Node

        // ObjectDeclaration.SetProduction(TypeSpecifier * ObjectName);
        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            var node = (ParseBranch<TSymbol>)parseTree;
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
	}
}

