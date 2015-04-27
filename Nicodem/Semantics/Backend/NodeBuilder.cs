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

			public Brep.Node Build(ConstNode blockNode)
			{
				throw new NotImplementedException();
			}

			public Brep.Node Build(FunctionCallNode funCallNode)
			{
				List<Brep.Node> args = new List<Brep.Node>();
				foreach(var arg in funCallNode.Arguments) {
					args.Add(Build(arg as dynamic));
				}
				// TODO conflict - in ast a class, in backand a generic parameter
				throw new NotImplementedException();
			}

			public Brep.Node Build(OperatorNode opNode)
			{
				// TODO not enaugh more operator types
				var leftArg = Build(opNode.Arguments.ElementAt(0) as dynamic);
				var rightArg = Build(opNode.Arguments.ElementAt(1) as dynamic);

				switch(opNode.Operator) {

				case OperatorType.OT_ASSIGNMENT:
					throw new NotImplementedException();
				case OperatorType.OT_PLUS:
					return new Brep.BinaryOperatorNode(Brep.BinaryOperatorType.Add, leftArg, rightArg);
				}
				throw new NotImplementedException();
			}
		}
	}
}

