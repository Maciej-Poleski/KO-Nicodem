using System.Collections.Generic;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	class OperatorNode : OperationNode
	{
		public OperatorType Operator { get; set; }
        
        #region implemented abstract members of Node

        public static OperatorNode BuildBinaryOperator(string op, ExpressionNode left, ExpressionNode right){
            var opNode = new OperatorNode();
            var args = new LinkedList<ExpressionNode>();
            args.AddLast(left);
            args.AddLast(right);
            opNode.Arguments = args;
            opNode.Operator = GetOperatorType(op);
            return opNode;
        }

        public static OperatorType GetOperatorType(string text){
            //TODO: implement getting OperatorType from textual representation
            return OperatorType.OT_PLUS;
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
	}
}

