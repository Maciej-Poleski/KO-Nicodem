using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		static Func<RegisterNode, Node, IEnumerable<Instruction>> noInstructions() {
			return (arg1, arg2) => new Instruction[0];
		}

		static IEnumerable<RegisterNode> use(params RegisterNode[] nodes) {
			return nodes;
		}

		static IEnumerable<RegisterNode> define(params RegisterNode[] nodes) {
			return nodes;
		}

		static Instruction copyToTemporary( RegisterNode temporary, RegisterNode source ) {
			return new Instruction (
				map => string.Format ("mov {0}, {1}", map[temporary], map[source]),
				use (temporary, source), 
				define (temporary), 
				true
			);
		}

		static Tile makeTile<T>(params Tile[] children) {
			return new Tile (typeof(T), children, noInstructions ());
		}

		public static class Lea
		{
			// lea dst, [reg1 + val1 * reg2 + val2]
			// x = y * c,  better for c = 0,1,2,3,4,5,8,9
			public static Tile EffectiveMultiplication() {
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode>(),
						makeTile<MulOperatorNode>(
							makeTile<RegisterNode>(),
							makeTile<ConstantNode<long>>()
						)
					},
					(regNode, node) => {
						var assignment = node as AssignmentNode;
						var dst = assignment.Target as RegisterNode;
						var mul = assignment.Source as MulOperatorNode;
						var mul_reg = mul.LeftOperand as RegisterNode;
						var mul_val = mul.RightOperand as ConstantNode<long>;

						switch (mul_val.Value) {
						case 2L:
						case 4L:
						case 8L:
							return new [] {
								new Instruction (
									map => string.Format("lea {0}, [{1}*{2}]", map[dst], map[mul_reg], mul_val.Value),
									use(dst, mul_reg), define(dst)),
								copyToTemporary( regNode, dst )
							};
						case 3L:
						case 5L:
						case 9L:
							return new [] {
								new Instruction (
									map => string.Format("lea {0}, [{1}+{1}*{2}]", map[dst], map[mul_reg], mul_val.Value - 1L),
									use(dst, mul_reg), define(dst)),
								copyToTemporary( regNode, dst )
							};
						case 1L:
							return new [] {
								new Instruction (
									map => string.Format("mov {0}, {1}", map[dst], map[mul_reg]),
									use(dst, mul_reg), define(dst), true),
								copyToTemporary( regNode, dst )
							};
						case 0L:
							return new [] {
								new Instruction (
									map => string.Format("xor {0}, {0}", map[dst]),
									use(dst), define(dst)),
								copyToTemporary( regNode, dst )
							};
						default:
							return null; // TODO copy from multiply
						}
					}
				);
			}

			public static Tile Reg_RegRegConstConst() {
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode> (),
						makeTile<AddOperatorNode>(
							makeTile<AddOperatorNode>(
								makeTile<RegisterNode>(),
								makeTile<MulOperatorNode> (
									makeTile<RegisterNode> (),
									makeTile<ConstantNode<long>> ()
								)
							),
							makeTile<ConstantNode<long>>()
						)
					},
					(regNode, node) => {
						var assignment = node as AssignmentNode;
						var dst = assignment.Target as RegisterNode;
						var add1 = assignment.Source as AddOperatorNode;
						var add2 = add1.LeftOperand as AddOperatorNode;

						var reg1 = add2.LeftOperand as RegisterNode;
						var mul = add2.RightOperand as MulOperatorNode;
						var mul_reg = mul.LeftOperand as RegisterNode;
						var mul_val = mul.RightOperand as ConstantNode<long>;
						var val2 = add1.RightOperand as ConstantNode<long>;

						return new [] {
							new Instruction (
								map => string.Format ("lea {0}, [{1}+{2}*{3}+{4}]", map [dst], map [reg1], map[mul_reg], mul_val.Value, val2.Value),
								use (dst, mul_reg), define (dst)),
							copyToTemporary (regNode, dst)
						};
					}
				);
			}

			public static Tile Reg_RegReg() {
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode>(),
						makeTile<MemoryNode>(
							makeTile<AddOperatorNode>(
								makeTile<RegisterNode>(),
								makeTile<RegisterNode>()
							)
						)
					},
					(regNode, node) => {
						var assignment = node as AssignmentNode;
						var dst = assignment.Target as RegisterNode;
						var mem = assignment.Source as MemoryNode;
						var mem_add = mem.Address as AddOperatorNode;
						var mem_add_1 = mem_add.LeftOperand as RegisterNode;
						var mem_add_2 = mem_add.RightOperand as RegisterNode;

						return new [] {
							// lea dst, [reg1 + reg2]
							new Instruction (
								map => string.Format("lea {0}, [{1} + {2}]", map[dst], map[mem_add_1], map[mem_add_2]),
								use(dst, mem_add_1, mem_add_2), define(dst)),

							copyToTemporary( regNode, dst )
						};
					}
				);
			}
		}
	}
}

