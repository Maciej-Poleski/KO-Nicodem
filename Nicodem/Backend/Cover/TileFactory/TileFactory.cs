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

		static Tile makeTile<T>(params Tile[] children) {
			return new Tile (typeof(T), children, noInstructions ());
		}

		public static IEnumerable<Tile> GetTiles() {
			return new[] {
				LabelTile(),
				ConstTile<long>(),
				MemAccessTile(),
				MemAccessTile<long>(),

				Add.RegReg(),
				Add.RegConst<long>(),
				Add.ConstReg<long>(),

				Sub.RegReg(),
				Sub.RegConst<long>(),
				Sub.ConstReg<long>(),

				Mul.RegReg(),
				Mul.RegConst<long>(),
				Mul.ConstReg<long>(),

				Div.RegReg(),
				Div.RegConst<long>(),
				Div.ConstReg<long>(),

				Mod.RegReg(),
				Mod.RegConst<long>(),
				Mod.ConstReg<long>(),

				Shl.RegConst<long>(),
				Shr.RegConst<long>(),

				BitXor.RegReg(),
				BitXor.RegConst<long>(),
				BitXor.ConstReg<long>(),
				BitAnd.RegReg(),
				BitAnd.RegConst<long>(),
				BitAnd.ConstReg<long>(),
				BitOr.RegReg(),
				BitOr.RegConst<long>(),
				BitOr.ConstReg<long>(),

				LogAnd.RegReg(),
				LogAnd.RegConst(),
				LogAnd.ConstReg(),
				LogOr.RegReg(),
				LogOr.RegConst<long>(),
				LogOr.ConstReg<long>(),

				Unop.Inc_Reg(),
				Unop.Dec_Reg(),
				Unop.Plus_Reg(),
				Unop.Minus_Reg(),
				Unop.Neg_Reg(),
				Unop.BinNot_Reg(),

				Compare.RegReg_Eq(),
				Compare.RegReg_Neq(),
				Compare.RegReg_Lt(),
				Compare.RegReg_Le(),
				Compare.RegReg_Gt(),
				Compare.RegReg_Ge(),
				Compare.RegConst_Eq<long>(),
				Compare.RegConst_Neq<long>(),
				Compare.RegConst_Lt<long>(),
				Compare.RegConst_Le<long>(),
				Compare.RegConst_Gt<long>(),
				Compare.RegConst_Ge<long>(),
				Compare.ConstReg_Eq<long>(),
				Compare.ConstReg_Neq<long>(),
				Compare.ConstReg_Lt<long>(),
				Compare.ConstReg_Le<long>(),
				Compare.ConstReg_Gt<long>(),
				Compare.ConstReg_Ge<long>(),

				Assign.Reg_Reg(),
				Assign.Reg_Const(),
				Assign.Reg_AddConst(),
				Assign.Reg_SubConst(),
				Assign.MemReg_Reg(),
				Assign.MemReg_Const(),
				Assign.MemConst_Reg(),
				Assign.MemConst_Const(),

				Jump.Unconditional(),
				Jump.Cond_RegReg_Eq(),
				Jump.Cond_RegReg_Neq(),
				Jump.Cond_RegReg_Lt(),
				Jump.Cond_RegReg_Le(),
				Jump.Cond_RegReg_Gt(),
				Jump.Cond_RegReg_Ge(),
				Jump.Cond_RegConst_Eq<long>(),
				Jump.Cond_RegConst_Neq<long>(),
				Jump.Cond_RegConst_Lt<long>(),
				Jump.Cond_RegConst_Le<long>(),
				Jump.Cond_RegConst_Gt<long>(),
				Jump.Cond_RegConst_Ge<long>(),
				Jump.Cond_ConstReg_Eq<long>(),
				Jump.Cond_ConstReg_Neq<long>(),
				Jump.Cond_ConstReg_Lt<long>(),
				Jump.Cond_ConstReg_Le<long>(),
				Jump.Cond_ConstReg_Gt<long>(),
				Jump.Cond_ConstReg_Ge<long>()
			};
		}

		public static Tile LabelTile() {
			return new Tile (typeof(LabelNode),
				new Tile[] { },
				(regNode, node) => new[] {
					InstructionFactory.Label (node as LabelNode)
				}
			);
		}

		public static Tile ConstTile<T>() {
			return new Tile (typeof(ConstantNode<T>),
				new Tile[] { },
				(regNode, node) => new[] {
					InstructionFactory.Move (regNode, node as ConstantNode<T>)
				}
			);
		}

		public static Tile MemAccessTile() {
			return new Tile (typeof(MemoryNode),
				new [] {
					makeTile<RegisterNode> ()
				},
				(regNode, node) => {
					var root = node as MemoryNode;
					var reg = root.Address as RegisterNode;
					return new[] {
						InstructionFactory.MoveFromMemory (regNode, reg)
					};
				}
			);
		}

		public static Tile MemAccessTile<T>() {
			return new Tile (typeof(MemoryNode),
				new [] {
					makeTile<ConstantNode<T>> ()
				},
				(regNode, node) => {
					var root = node as MemoryNode;
					var add = root.Address as ConstantNode<T>;
					return new[] {
						InstructionFactory.MoveFromMemory (regNode, add)
					};
				}
			);
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
								InstructionFactory.Move( regNode, dst )
							};
						case 3L:
						case 5L:
						case 9L:
							return new [] {
								new Instruction (
									map => string.Format("lea {0}, [{1}+{1}*{2}]", map[dst], map[mul_reg], mul_val.Value - 1L),
									use(dst, mul_reg), define(dst)),
								InstructionFactory.Move( regNode, dst )
							};
						case 1L:
							return new [] {
								new Instruction (
									map => string.Format("mov {0}, {1}", map[dst], map[mul_reg]),
									use(dst, mul_reg), define(dst), true),
								InstructionFactory.Move( regNode, dst )
							};
						case 0L:
							return new [] {
								new Instruction (
									map => string.Format("xor {0}, {0}", map[dst]),
									use(dst), define(dst)),
								InstructionFactory.Move( regNode, dst )
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
							InstructionFactory.Move (regNode, dst)
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

							InstructionFactory.Move( regNode, dst )
						};
					}
				);
			}
		}
	}
}

