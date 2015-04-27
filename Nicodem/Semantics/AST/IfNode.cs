using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class IfNode : ExpressionNode
	{
        public ExpressionNode Condition { get; set; } // set during AST construction
        public ExpressionNode Then { get; set; } // set during AST construction
        public ExpressionNode Else { get; set; } // set during AST construction

		public bool HasElse { get { return !ReferenceEquals (Else, null); } }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // IfExpression -> "if" Expression Expression ("else" Expression)?
            var childs = ASTBuilder.ChildrenArray(parseTree);
            Debug.Assert(childs.Length >= 3);
            Condition = ExpressionNode.GetExpressionNode(childs[1]);
            Then = ExpressionNode.GetExpressionNode(childs[2]);
            if (childs.Length == 5)
            {
                Else = ExpressionNode.GetExpressionNode(childs[4]);
            }
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

