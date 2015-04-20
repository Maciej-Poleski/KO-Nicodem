using System.Collections.Generic;
using System.Diagnostics;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
	abstract class ExpressionNode : Node
	{
		public TypeNode ExpressionType { get; set; }
        
        #region implemented abstract members of Node

        const int OP_BOTTOM_LEVEL = 4;

        /// <summary>
        /// Parse one line expression with opLevel-expressions and binary operators (between them).
        /// </summary>
        public static ExpressionNode ParseBinOperator<TSymbol>(IEnumerator<IParseTree<TSymbol>> opExpr, int opLevel) where TSymbol:ISymbol<TSymbol>
        {
            var leftArg = ResolveOperatorExpression(opExpr.Current, opLevel);
            if (opExpr.MoveNext()) { // this is not the last symbol
                // build recursively binary operator for this arg and the rest
                // get an operator
                string opText = opExpr.Current.Fragment.GetOriginText();
                Debug.Assert(opExpr.MoveNext());
                // get the right hand side of this operator
                var rightArg = ParseBinOperator(opExpr, opLevel);
                // build and return binary operator
                return OperatorNode.BuildBinaryOperator(opText, leftArg, rightArg);
            } else { // last arg - just return it
                return leftArg;
            }
        }

        public static ExpressionNode ResolveOperatorExpression<TSymbol>(IParseTree<TSymbol> parseTree, int opLevel) where TSymbol:ISymbol<TSymbol>
        {
            if (opLevel > OP_BOTTOM_LEVEL) {
                // these part proceeds op from level 15 to 5
                var childs = ASTBuilder.Children(parseTree).GetEnumerator(); // get childrens connected by operators (one level lower)
                Debug.Assert(childs.MoveNext()); // set enumerator to the first element (must be present)
                return ParseBinOperator(childs, opLevel - 1);
            } else {
                // continue parsing lower level expressions
                Debug.Assert(opLevel == OP_BOTTOM_LEVEL); // op level must be 4
                throw new System.NotImplementedException();
            }
        }

        public static ExpressionNode GetExpressionNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var node = ASTBuilder.FirstChild(parseTree); // Expression -> OperatorExpression
            node = ASTBuilder.FirstChild(node); // OperatorExpression -> Operator17Expression
            node = ASTBuilder.FirstChild(node); // Operator17Expression -> Operator16Expression
            node = ASTBuilder.FirstChild(node); // Operator16Expression -> Operator15Expression

            // TODO: what about operator "="? variable def?
            return ResolveOperatorExpression(node, 15); // node is operator 15
        }

        #endregion

		public override void Accept (AbstractVisitor visitor)
		{
			visitor.Visit (this);
		}
	}
}

