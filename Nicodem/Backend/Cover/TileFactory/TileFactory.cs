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

				Stack.Call(),
				Stack.Ret(),
				Stack.Push(),
				Stack.Pop(),

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
				Unop.LogNot_Reg(),

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

				Jump.Unconditional_Label(),
				Jump.Unconditional_Reg(),
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

		public static class Stack
		{
			public static Tile Call() {
				return new Tile (typeof(FunctionCallNode),
					new Tile[] { },
					(regNode, node) => new[] {
						InstructionFactory.Call (node as FunctionCallNode)
					}
				);
			}

			public static Tile Ret() {
				return new Tile (typeof(RetNode),
					new Tile[] { },
					(regNode, node) => new[] {
						InstructionFactory.Ret ()
					}
				);
			}

			public static Tile Push() {
				return new Tile (typeof(PushNode),
					new [] {
						makeTile<RegisterNode> ()
					},
					(regNode, node) => {
						var root = node as PushNode;
						return new[] {
							InstructionFactory.Push (root.Register)
						};
					}
				);
			}

			public static Tile Pop() {
				return new Tile (typeof(PopNode),
					new [] {
						makeTile<RegisterNode> ()
					},
					(regNode, node) => {
						var root = node as PopNode;
						return new[] {
							InstructionFactory.Pop (root.Register)
						};
					}
				);
			}
		}
	}
}

