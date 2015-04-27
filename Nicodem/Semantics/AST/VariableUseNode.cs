using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class VariableUseNode : ExpressionNode
	{
        public string Name { get; set; } // set during AST construction
		public VariableDeclNode Declaration { get; set; }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // ObjectUseExpression -> ObjectName
            Name = parseTree.Fragment.GetOriginText();
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

