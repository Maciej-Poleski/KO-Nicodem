using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nicodem.Semantics.Visitors;
using Nicodem.Parser;
using System;

namespace Nicodem.Semantics.AST
{
	abstract class ExpressionNode : Node
	{
		public TypeNode ExpressionType { get; set; }
        
        #region implemented abstract members of Node

        const int OP_BOTTOM_LEVEL = 4;

        /// <summary>
        /// Parse one line expression with unary prefix operators. Operators must be given in reversed order. Enumerator must be
        /// positioned before first operator.
        /// </summary>
        /// <returns>Expression precedes with given operators.</returns>
        public static ExpressionNode AddPrefixOperators<TSymbol>(int opLevel, ExpressionNode expr, IEnumerator<IParseTree<TSymbol>> operators) 
            where TSymbol : ISymbol<TSymbol>
        {
            ExpressionNode lastNode = expr;
            while (operators.MoveNext())
            {
                string opText = operators.Current.Fragment.GetOriginText();
                lastNode = OperatorNode.BuildUnaryOperator(opLevel, opText, lastNode);
            }
            return lastNode;
        }

        /// <summary>
        /// Parse one line expression with opLevel-expressions and binary operators (from one level above) between them. 
        /// If you want to parse leftToRight operator - provide enumerator with reversed order of arguments.
        /// </summary>
        public static ExpressionNode ParseBinOperator<TSymbol>(IEnumerator<IParseTree<TSymbol>> opExpr, int opLevel, bool leftToRight) 
            where TSymbol:ISymbol<TSymbol>
        {
            var leftArg = ResolveOperatorExpression(opExpr.Current, opLevel);
            if (opExpr.MoveNext()) { // this is not the last symbol
                // build recursively binary operator for this arg and the rest
                // get an operator
                string opText = opExpr.Current.Fragment.GetOriginText();
                Debug.Assert(opExpr.MoveNext());
                // get the right hand side of this operator
                var rightArg = ParseBinOperator(opExpr, opLevel, leftToRight);
                // build and return binary operator
                return leftToRight ?
                    OperatorNode.BuildBinaryOperator(opLevel+1, opText, rightArg, leftArg) : // if left-to-right, reversed arguments
                    OperatorNode.BuildBinaryOperator(opLevel+1, opText, leftArg, rightArg); // right-to-left, normal order
            } else { // last arg - just return it
                return leftArg;
            }
        }

        /// <summary>
        /// Parse one line expression with ++ -- (postfix), function call, array subscript, slice subscript operators.
        /// Enumerator must be given with reversed ordering of arguments. This corresponds to Operator2Expression from Grammar.
        /// </summary>
        public static ExpressionNode ParseOperator2<TSymbol>(IEnumerator<IParseTree<TSymbol>> opExpr) where TSymbol : ISymbol<TSymbol>
        {
            // O2 -> O1 (     "++" | "--"
            //           |    "(" (E ",")* E? ")"
            //           |    "[" E "]"
            //           |    "[" E? ".." E? "]"
            //          )*
            if (opExpr.Current.Symbol.IsTerminal)
            {
                string curText = ASTBuilder.EatTerminal(opExpr);
                switch (curText)
                {
                    case "++":
                    case "--":
                        return OperatorNode.BuildUnaryOperator(2, curText, ParseOperator2(opExpr));
                    case ")":
                        var args = new LinkedList<ExpressionNode>();
                        while (!ASTBuilder.EatSymbol("(", opExpr))
                        {
                            ASTBuilder.EatSymbol(",", opExpr); // eat ',' before arg - f(x,y,z,) is also possible
                            args.AddFirst(GetExpressionNode(opExpr.Current));
                            opExpr.MoveNext(); // move to next param with ','
                        }
                        var res = new FunctionCallNode();
                        res.Arguments = new List<ExpressionNode>(args);
                        // TODO: can something return a 'pointer' to function which we can call to execute?
                        res.Name = opExpr.Current.Fragment.GetOriginText();
                        return res;
                    case "]":
                        var toExpr = ASTBuilder.EatNonTerminal(opExpr); // slice to or index
                        bool isSlice = ASTBuilder.EatSymbol("..", opExpr);
                        var fromExpr = ASTBuilder.EatNonTerminal(opExpr);
                        Debug.Assert(ASTBuilder.EatSymbol("[", opExpr));
                        if (isSlice) // slice subscript
                        {
                            return SliceNode.CreateSliceNode(ParseOperator2(opExpr),
                                GetExpressionOrNull(fromExpr), GetExpressionOrNull(toExpr));
                        }
                        else // array subscript
                        {
                            return ElementNode.Create(ParseOperator2(opExpr), GetExpressionNode(toExpr));
                        }
                    default:
                        Debug.Assert(false);
                        break;
                }
            }
            else // non-terminal -> Operator1Expression
            {
                var op1 = opExpr.Current;
                Debug.Assert(!opExpr.MoveNext()); // it must be final symbol in this enumeration
                var node = ASTBuilder.FirstChild(op1); // Operator1Expression -> Operator0Expression
                // Operator0Expression -> AtomicExpression | "(" Expression ")";
                var childs = ASTBuilder.ChildrenEnumerator(node);
                Debug.Assert(childs.MoveNext());
                if (ASTBuilder.EatSymbol("(", childs))
                {
                    return GetExpressionNode(childs.Current); // Expression
                }
                else // proceed with AtomicExpression
                {
                    return ResolveAtomicExpression(childs.Current);
                }
            }
            return null;
        }

        public static ExpressionNode ResolveAtomicExpression<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol : ISymbol<TSymbol>
        {
            // AtomicExpression -> BlockExpression | ObjectDefinitionExpression | ArrayLiteralExpression | ObjectUseExpression |
            //                     IfExpression | WhileExpression | LoopControlExpression
            var node = ASTBuilder.FirstChild(parseTree); // AtomicExpression -> one of available symbols
            ExpressionNode atomic;
            switch (ASTBuilder.GetName(node.Symbol))
            {
                case "BlockExpression":
                    atomic = new BlockExpressionNode();
                    break;
                case "ArrayLiteralExpression":
                    throw new System.NotImplementedException(); // TODO: arrays implementation
                case "ObjectUseExpression":
                    // ObjectUseExpression -> ObjectName | Literals
                    node = ASTBuilder.FirstChild(node);
                    if (node.Symbol.IsTerminal) { // ObjectName
                        atomic = new VariableUseNode();
                    } else { // Literals
                        node = ASTBuilder.FirstChild(node); // Literals -> one of available literals
                        atomic = ConstNode.GetConstNode(node);
                    }
                    break;
                case "LoopControlExpression":
                    atomic = new LoopControlNode();
                    break;
                default:
                    throw new System.ArgumentException();
            }
            atomic.BuildNode(node);
            return atomic;
        }

        public static ExpressionNode ResolveOperatorExpression<TSymbol>(IParseTree<TSymbol> parseTree, int opLevel) where TSymbol:ISymbol<TSymbol>
        {
            if (opLevel > OP_BOTTOM_LEVEL) {
                // these part proceeds op from level 15 to 5
                bool isLeft = ASTBuilder.IsLeftOperator(parseTree.Symbol);
                // get childrens connected by operators (one level lower)
                var childs = isLeft ?
                    ASTBuilder.Children(parseTree).Reverse().GetEnumerator() : // if left-to-right then reverse arguments
                    ASTBuilder.Children(parseTree).GetEnumerator();
                Debug.Assert(childs.MoveNext()); // set enumerator to the first element (must be present)
                return ParseBinOperator(childs, opLevel - 1, isLeft);
            } else {
                // continue parsing lower level expressions
                Debug.Assert(opLevel == OP_BOTTOM_LEVEL); // op level must be 4
                var node = ASTBuilder.FirstChild(parseTree); // Operator4Expression -> Operator3Expression
                // Operator3Expression -> (prefix_operator)* Operator2Expression
                var childs = ASTBuilder.Children(node).Reverse().GetEnumerator();
                Debug.Assert(childs.MoveNext());
                var op2 = childs.Current; // last child -> Operator2Expression
                var op2Childs = ASTBuilder.RevChildrenEnumerator(op2);
                Debug.Assert(op2Childs.MoveNext());
                return AddPrefixOperators(3, ParseOperator2(op2Childs), childs);
            }
        }

        /// <summary>
        /// Resolve Expression from ParseTree. Main method.
        /// </summary>
        public static ExpressionNode GetExpressionNode<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
		{
			var node = ASTBuilder.FirstChild(parseTree); // Expression -> RecordVariableDefinitionExpression | RecordFieldAccessExpression | OperatorExpression

			if (ASTBuilder.GetName (node.Symbol) == "RecordVariableDefinitionExpression") {
				var nod = new RecordVariableDefNode ();
				nod.BuildNode (node);
				return nod;
			}

			if (ASTBuilder.GetName (node.Symbol) == "RecordFieldAccessExpression") {
				var nod = new RecordVariableFieldUseNode ();
				nod.BuildNode (node);
				return nod;
			}

            node = ASTBuilder.FirstChild(node); // OperatorExpression -> Operator19Expression
            node = ASTBuilder.FirstChild(node); // Operator19Expression -> ObjectDefinitionExpression | Operator18Expression
            if (ASTBuilder.GetName(node.Symbol) == "ObjectDefinitionExpression") { // ObjectDefinition
                var defNode = new VariableDefNode();
                defNode.BuildNode(node);
                return defNode;
            } 
            // Operator18Expression
            node = ASTBuilder.FirstChild(node); // IfExpression | WhileExpression | Operator17Expression
            switch (ASTBuilder.GetName(node.Symbol))
            {
                case "IfExpression":
                    var ifNode = new IfNode();
                    ifNode.BuildNode(node);
                    return ifNode;
                case "WhileExpression":
                    var whileNode = new WhileNode();
                    whileNode.BuildNode(node);
                    return whileNode;
                default: // default - Operator17Expression
                    break;
            }
            node = ASTBuilder.FirstChild(node); // Operator17Expression -> Operator16Expression
            node = ASTBuilder.FirstChild(node); // Operator16Expression -> Operator15Expression
            return ResolveOperatorExpression(node, 15); // node is operator 15
        }

        public static ExpressionNode GetExpressionOrNull<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol : ISymbol<TSymbol>
        {
            if (parseTree == null)
                return null;
            return GetExpressionNode(parseTree);
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
            var rhs = (ExpressionNode)rhs_;
            return base.Compare(rhs) &&
                object.Equals(ExpressionType, rhs.ExpressionType);
        }

        protected override string PrintElements(string prefix)
        {
            return base.PrintElements(prefix)
                + PrintVar(prefix, "ExpressionType", ExpressionType);
        }
	}
}
