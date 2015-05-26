using System;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Semantics.AST;
using B = Nicodem.Backend;
using Brep = Nicodem.Backend.Representation;
using AST = Nicodem.Semantics.AST;

namespace Nicodem.Semantics
{
	class NodeBuilder
	{

		public static Brep.Node BuildNode(ExpressionNode expNode, AST.FunctionDefinitionNode funDef)
		{
			return (new RecursiveBuilder(funDef)).Build(expNode as dynamic);
		}

		// --- private classes -----------
		private class RecursiveBuilder
		{
			private readonly AST.FunctionDefinitionNode funDef;

			public RecursiveBuilder(AST.FunctionDefinitionNode funDef)
			{
				this.funDef = funDef;
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
				var type = constNode.VariableType as NamedTypeNode;
				if(type == null) {
					throw new InvalidOperationException("Constant can only be of a named type");
				}

				switch(type.Name) {
				case "char":
					return new Brep.ConstantNode<char>(constNode.Value[0]);
				case "byte":
					return new Brep.ConstantNode<byte>(Convert.ToByte(constNode.Value[0]));
				case "int":
					long res;
					Int64.TryParse(constNode.Value, out res);
					return new Brep.ConstantNode<long>(res);
				case "bool":
					if(constNode.Value.Equals("true")) {
						return new Brep.ConstantNode<bool>(true);
					} else if(constNode.Value.Equals("false")) {
						return new Brep.ConstantNode<bool>(false);
					} else {
						throw new InvalidOperationException("Bool can be either true or false but not: " + constNode.Value);
					}
				case "void":
					throw new InvalidOperationException("Constant cannot be void");

				default:
					throw new InvalidOperationException("No constant for a type with name: " + type.Name);
				}

			}

			// the only situation to call method is when we want a value of some function argument
			// all args in B.Function are assumed to be already set
			public Brep.Node Build(VariableDeclNode argNode)
			{
				int argNum = GetArgNumber(argNode, funDef);
				if(argNum < 0) {
					throw new InvalidOperationException("Trying to use undeclared function argument");
				}
				return funDef.BackendFunction.GetArgLocationNode(argNum);
			}

			public Brep.Node Build(VariableDefNode defNode)
			{
				var expr = Build(defNode.Value as dynamic);
				var locNode = funDef.BackendFunction.AccessLocal(defNode.VariableLocation);
				return new Brep.AssignmentNode(locNode, expr);
			}

			public Brep.Node Build(VariableUseNode useNode)
			{
				var definition = useNode.Declaration as VariableDefNode;
				if(definition != null) {
					return funDef.BackendFunction.AccessLocal(definition.VariableLocation);
				} else {
					// must be a function argument
					return Build(useNode.Declaration);
				}
			}

			public Brep.Node Build(FunctionCallNode funCallNode)
			{
			    var args = new Brep.Node[funCallNode.Arguments.Count];
			    for (var i = 0; i < args.Length; ++i)
			    {
			        args[i] = Build(funCallNode.Arguments[i] as dynamic);
			    }
                Action<Brep.Node> setter;
				return funCallNode.Definition.BackendFunction.FunctionCall(funDef.BackendFunction, args, out setter);
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
					return BuildAssignment(leftArg as Brep.LocationNode, rightArg);
				case OperatorType.PLUS_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.AddOperatorNode(leftArg, rightArg));
				case OperatorType.MINUS_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.SubOperatorNode(leftArg, rightArg));
				case OperatorType.MUL_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.MulOperatorNode(leftArg, rightArg));
				case OperatorType.DIV_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.DivOperatorNode(leftArg, rightArg));
				case OperatorType.MOD_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.ModOperatorNode(leftArg, rightArg));
				case OperatorType.BIT_SHIFT_UP_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.ShlOperatorNode(leftArg, rightArg));
				case OperatorType.BIT_SHIFT_DOWN_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.ShrOperatorNode(leftArg, rightArg));
				case OperatorType.BIT_AND_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.BitAndOperatorNode(leftArg, rightArg));
				case OperatorType.BIT_XOR_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.BitXorOperatorNode(leftArg, rightArg));
				case OperatorType.BIT_OR_ASSIGN:
					return BuildAssignment(leftArg as Brep.LocationNode,
						new Brep.BitOrOperatorNode(leftArg, rightArg));
		        // "||"
				case OperatorType.OR:
					return new Brep.LogOrOperatorNode(leftArg, rightArg);
		        // "&&"
				case OperatorType.AND:
					return new Brep.LogAndOperatorNode(leftArg, rightArg);
		        // "|"
				case OperatorType.BIT_OR:
					return new Brep.BitOrOperatorNode(leftArg, rightArg);
		        // "^"
				case OperatorType.BIT_XOR:
					return new Brep.BitXorOperatorNode(leftArg, rightArg);
		        // "&"
				case OperatorType.BIT_AND:
					return new Brep.BitAndOperatorNode(leftArg, rightArg);
		        // "==", "!="
				case OperatorType.EQUAL:
					return new Brep.EqOperatorNode(leftArg, rightArg);
				case OperatorType.NOT_EQUAL:
					return new Brep.NeqOperatorNode(leftArg, rightArg);
		        // "< <= > >="
				case OperatorType.LESS:
					return new Brep.LtOperatorNode(leftArg, rightArg);
				case OperatorType.LESS_EQUAL:
					return new Brep.LteOperatorNode(leftArg, rightArg);
				case OperatorType.GREATER:
					return new Brep.GtOperatorNode(leftArg, rightArg);
				case OperatorType.GREATER_EQUAL:
					return new Brep.GteOperatorNode(leftArg, rightArg);
		        // "<< >>"
				case OperatorType.BIT_SHIFT_UP:
					return new Brep.ShlOperatorNode(leftArg, rightArg);
				case OperatorType.BIT_SHIFT_DOWN:
					return new Brep.ShrOperatorNode(leftArg, rightArg);
		        // "+ -"
				case OperatorType.PLUS: 
					return new Brep.AddOperatorNode(leftArg, rightArg);
				case OperatorType.MINUS:
					return new Brep.SubOperatorNode(leftArg, rightArg);
		        // "* / %"
				case OperatorType.MUL:
					return new Brep.MulOperatorNode(leftArg, rightArg);
				case OperatorType.DIV:
					return new Brep.DivOperatorNode(leftArg, rightArg);
				case OperatorType.MOD:
					return new Brep.ModOperatorNode(leftArg, rightArg);

				default:
					throw new InvalidOperationException("Wrong operator!");
				}
			}

			private Brep.Node BuildUnaryOperator(OperatorNode opNode) 
			{
				var arg = Build(opNode.Arguments.ElementAt(0) as dynamic);

				switch(opNode.Operator) {
		        // PRE "++ -- + - ! ~"
				case OperatorType.PRE_INCREMENT:
					return new Brep.IncOperatorNode(arg);
				case OperatorType.PRE_DECREMENT:
					return new Brep.DecOperatorNode(arg);
				case OperatorType.UNARY_PLUS:
					return new Brep.UnaryPlusOperatorNode(arg);
				case OperatorType.UNARY_MINUS:
					return new Brep.UnaryMinusOperatorNode(arg);
				case OperatorType.NOT:
					return new Brep.LogNotOperatorNode(arg);
				case OperatorType.BIT_NOT:
					return new Brep.BinNotOperatorNode(arg);
		        // POST "++", "--"
				case OperatorType.POST_INCREMENT:
					{
						var result = new Brep.TemporaryNode();
						var actions = new List<Brep.Node> {
							new Brep.AssignmentNode(result, arg),
							new Brep.IncOperatorNode(arg)
						};
						return new Brep.SequenceNode(actions, null, result);
					}

				case OperatorType.POST_DECREMENT:
					{
						var result = new Brep.TemporaryNode();
						var actions = new List<Brep.Node> {
							new Brep.AssignmentNode(result, arg),
							new Brep.DecOperatorNode(arg)
						};
						return new Brep.SequenceNode(actions, null, result);
					}

				default:
					throw new InvalidOperationException("Wrong operator!");
				}
			}

			private static Brep.Node BuildAssignment(Brep.LocationNode location, Brep.Node expr)
			{
				if(location == null) {
					throw new InvalidOperationException("Cannot assign something to a null location");
				}
				return new Brep.AssignmentNode(location, expr);
			}

			private static int GetArgNumber(AST.VariableDeclNode arg, AST.FunctionDefinitionNode funDef)
			{
				for(int i = 0; i < funDef.Parameters.Count(); i++) {
					if(Object.ReferenceEquals(arg, funDef.Parameters.ElementAt(i))) {
						return i;
					}
				}

				return -1;
			}
		}
	}
}

