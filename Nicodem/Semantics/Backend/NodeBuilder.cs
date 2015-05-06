using System;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Semantics.AST;
using B = Nicodem.Backend;
using Brep = Nicodem.Backend.Representation;

namespace Nicodem.Semantics
{
	class NodeBuilder
	{

		public static Brep.Node BuildNode(ExpressionNode expNode, B.Function function)
		{
			return (new RecursiveBuilder(function)).Build(expNode as dynamic);
		}

		// --- private classes -----------
		private class RecursiveBuilder
		{
			private readonly B.Function function;

			public RecursiveBuilder(B.Function function)
			{
				this.function = function;
			}

			public Brep.Node Build(Node node) {
				// This type of node is not supported
				throw new InvalidOperationException("Cannot build backend for this node");
			}

			public Brep.Node Build(BlockExpressionNode blockNode)
			{
				var seq = new List<Brep.Node>();
				foreach(var exp in blockNode.Elements) {
					seq.Add(Build(exp as dynamic));
				}
				return new Brep.SequenceNode(seq, null); // TODO maybe set that jump later?
			}

			public Brep.Node Build(ConstNode constNode)
			{
				throw new NotImplementedException();
			}

			public Brep.Node Build(FunctionCallNode funCallNode)
			{
				List<Brep.Node> args = new List<Brep.Node>();
				foreach(var arg in funCallNode.Arguments) {
					args.Add(Build(arg as dynamic));
				}
				return new Brep.FunctionCallNode(funCallNode.Definition.BackendFunction, args);
			}

			public Brep.Node Build(OperatorNode opNode)
			{
				if(opNode.Operator.CompareTo(OperatorType.PRE_INCREMENT) < 0) {
					return BuildBinaryOperator(opNode);
				} else {
					return BuildUnaryOperator(opNode);
				}
			}

			private Brep.Node BuildBinaryOperator(OperatorNode opNode) 
			{
				var leftArg = Build(opNode.Arguments.ElementAt(0) as dynamic);
				var rightArg = Build(opNode.Arguments.ElementAt(1) as dynamic);

				switch(opNode.Operator) {

				case OperatorType.ASSIGN:
				case OperatorType.PLUS_ASSIGN:
				case OperatorType.MINUS_ASSIGN:
				case OperatorType.MUL_ASSIGN:
				case OperatorType.DIV_ASSIGN:
				case OperatorType.MOD_ASSIGN:
				case OperatorType.BIT_SHIFT_UP_ASSIGN:
				case OperatorType.BIT_SHIFT_DOWN_ASSIGN:
				case OperatorType.BIT_AND_ASSIGN:
				case OperatorType.BIT_XOR_ASSIGN:
				case OperatorType.BIT_OR_ASSIGN:
		        // "||"
				case OperatorType.OR:
		        // "&&"
				case OperatorType.AND:
		        // "|"
				case OperatorType.BIT_OR:
		        // "^"
				case OperatorType.BIT_XOR:
		        // "&"
				case OperatorType.BIT_AND:
		        // "==", "!="
				case OperatorType.EQUAL:
				case OperatorType.NOT_EQUAL:
		        // "< <= > >="
				case OperatorType.LESS:
				case OperatorType.LESS_EQUAL:
				case OperatorType.GREATER:
				case OperatorType.GREATER_EQUAL:
		        // "<< >>"
				case OperatorType.BIT_SHIFT_UP:
				case OperatorType.BIT_SHIFT_DOWN:
		        // "+ -"
				case OperatorType.PLUS: 
				case OperatorType.MINUS:
		        // "* / %"
				case OperatorType.MUL:
				case OperatorType.DIV:
				case OperatorType.MOD:
					break;

				default:
					throw new InvalidOperationException("Wrong operator!");
				}
				throw new NotImplementedException();
			}

			private Brep.Node BuildUnaryOperator(OperatorNode opNode) 
			{
				var arg = Build(opNode.Arguments.ElementAt(0) as dynamic);
				switch(opNode.Operator) {
		        // PRE "++ -- + - ! ~"
				case OperatorType.PRE_INCREMENT:
				case OperatorType.PRE_DECREMENT:
				case OperatorType.UNARY_PLUS:
				case OperatorType.UNARY_MINUS:
				case OperatorType.NOT:
				case OperatorType.BIT_NOT:
		        // POST "++", "--"
				case OperatorType.POST_INCREMENT:
				case OperatorType.POST_DECREMENT:
					break;

				default:
					throw new InvalidOperationException("Wrong operator!");
				}
				throw new NotImplementedException();
			}
		}
	}
}

