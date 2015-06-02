using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.ExpressionGraph;

namespace Nicodem.Semantics.AST
{
    class OperatorNode : ExpressionNode
	{
        public OperatorType Operator { get; set; } // set during AST construction
        public IEnumerable<ExpressionNode> Arguments { get; set; } // set during AST construction

        #region implemented abstract members of Node

        public static OperatorNode BuildUnaryOperator(int level, string op, ExpressionNode arg)
        {
            var opNode = new OperatorNode();
            var args = new LinkedList<ExpressionNode>();
            args.AddLast(arg);
            opNode.Arguments = args;
            opNode.Operator = OperatorTypeHelper.GetOperatorType(level, op);
            return opNode;
        }

        public static OperatorNode BuildBinaryOperator(int level, string op, ExpressionNode left, ExpressionNode right){
            var opNode = new OperatorNode();
            var args = new LinkedList<ExpressionNode>();
            args.AddLast(left);
            args.AddLast(right);
            opNode.Arguments = args;
            opNode.Operator = OperatorTypeHelper.GetOperatorType(level, op);
            return opNode;
        }

        public override void BuildNode<TSymbol>(IParseTree<TSymbol> parseTree)
        {
            throw new System.NotImplementedException();
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

