using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Backend;
using Brep = Nicodem.Backend.Representation;

namespace Nicodem.Semantics.AST
{
	class VariableDefNode : VariableDeclNode
	{
		public ExpressionNode Value { get; set; } // set during AST construction
		public Brep.LocationNode VariableLocation { get; set; }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // ObjectDefinition -> TypeSpecifier ObjectName "=" Expression
            var childs = ASTBuilder.ChildrenArray(parseTree);
            Debug.Assert(childs.Length == 4);
            Type = TypeNode.GetTypeNode(childs[0]); // type - arg 0
            Name = childs[1].Fragment.GetOriginText(); // name - arg 1
            Value = ExpressionNode.GetExpressionNode(childs[3]); // value - arg 3
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

