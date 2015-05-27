using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	class WhileNode : ExpressionNode
	{
        public ExpressionNode Condition { get; set; } // set during AST construction
        public ExpressionNode Body { get; set; } // set during AST construction
        public ExpressionNode Else { get; set; } // set during AST construction

		public bool HasElse { get { return !ReferenceEquals (Else, null); } }
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // WhileExpression -> "while" Expression Expression ("else" Expression)?
            var childs = ASTBuilder.ChildrenArray(parseTree);
            Debug.Assert(childs.Length >= 3);
            Condition = ExpressionNode.GetExpressionNode(childs[1]);
            Body = ExpressionNode.GetExpressionNode(childs[2]);
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

        public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
	}
}

