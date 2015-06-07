using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

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

        public override T Accept<T>(ReturnedAbstractVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        protected override bool Compare(object rhs_)
        {
            var rhs = (IfNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Condition, rhs.Condition) &&
                object.Equals(Then, rhs.Then) &&
                object.Equals(Else, rhs.Else);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix)
                + PrintVar(prefix, "Condition", Condition)
                + PrintVar(prefix, "Then", Then)
                + PrintVar(prefix, "Else", Else);
        }
	}
}

