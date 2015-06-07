using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
	enum LoopControlMode
	{
		LCM_BREAK,
		LCM_CONTINUE
	}

	class LoopControlNode : ExpressionNode
	{
        public LoopControlMode Mode { get; set; } // set during AST construction
        public int Depth { get; set; } // set during AST construction
        public ExpressionNode Value { get; set; } // set during AST construction
        
        #region implemented abstract members of Node

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            // LoopControl -> ("break" | "continue") (Expression DecimalNumberLiteral?)?
            var childs = ASTBuilder.ChildrenArray(parseTree);
            Debug.Assert(childs.Length >= 1);
            switch (childs[0].Fragment.GetOriginText())
            {
                case "break":
                    Mode = LoopControlMode.LCM_BREAK;
                    break;
                case "continue":
                    Mode = LoopControlMode.LCM_CONTINUE;
                    break;
            }
            if(childs.Length >= 2)
            {
                Value = ExpressionNode.GetExpressionNode(childs[1]);
            }
            if (childs.Length == 3)
            {
                Depth = int.Parse(childs[2].Fragment.GetOriginText());
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
            var rhs = (LoopControlNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(Mode, rhs.Mode) &&
                object.Equals(Depth, rhs.Depth) &&
                object.Equals(Value, rhs.Value);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix)
                + PrintVar(prefix, "Mode", Mode)
                + PrintVar(prefix, "Depth", Depth)
                + PrintVar(prefix, "Value", Value);
        }
	}
}

