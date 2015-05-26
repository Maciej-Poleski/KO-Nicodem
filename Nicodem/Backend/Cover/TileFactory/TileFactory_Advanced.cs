using Nicodem.Backend.Representation;
using System.Collections.Generic;
using System;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		public static class Advanced
		{
			// reg = T(L, R)
			static Tile makeRegisterAssignmentBinopTile<T,L,R>( Func<RegisterNode, L, R, IEnumerable<Instruction> > tileMaker )
				where T : BinaryOperatorNode
				where L : Node
				where R : Node
			{
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode> (),
						makeTile<T> (
							makeTile<L> (),
							makeTile<R> ()
						)
					},
					(regNode, node) => {
						var assignment = node as AssignmentNode;
						var dst = assignment.Target as RegisterNode;
						var binop = assignment.Source as T;
						var l = binop.LeftOperand as L;
						var r = binop.RightOperand as R;

						var insn = new LinkedList<Instruction> (tileMaker (dst, l, r));
						insn.AddLast (InstructionFactory.Move (regNode, dst));
						return insn;
					}
				);
			}

			// reg = T1(L, T2(M, R))
			static Tile makeRegisterAssignmentTriop1Tile<T1,L,T2,M,R>(
				Func<RegisterNode, L, M, R, IEnumerable<Instruction> > tileMaker
			)
				where T1 : BinaryOperatorNode
				where T2 : BinaryOperatorNode
				where L : Node
				where M : Node
				where R : Node
			{
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode> (),
						makeTile<T1> (
							makeTile<L> (),
							makeTile<T2> (
								makeTile<M> (),
								makeTile<R> ()
							)
						)
					},
					(regNode, node) => {
						var assignment = node as AssignmentNode;
						var dst = assignment.Target as RegisterNode;
						var t1 = assignment.Source as T1;
						var l = t1.LeftOperand as L;
						var t2 = t1.RightOperand as T2;
						var m = t2.LeftOperand as M;
						var r = t2.RightOperand as R;

						var insn = new LinkedList<Instruction> (tileMaker (dst, l, m, r));
						insn.AddLast (InstructionFactory.Move (regNode, dst));
						return insn;
					}
				);
			}

			// reg = T1(T2(L, M), R)
			static Tile makeRegisterAssignmentTriop2Tile<T1,T2,L,M,R>(
				Func<RegisterNode, L, M, R, IEnumerable<Instruction> > tileMaker
			)
				where T1 : BinaryOperatorNode
				where T2 : BinaryOperatorNode
				where L : Node
				where M : Node
				where R : Node
			{
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode> (),
						makeTile<T1> (
							makeTile<T2> (
								makeTile<L>(),
								makeTile<M>()
							),
							makeTile<R>()
						)
					},
					(regNode, node) => {
						var assignment = node as AssignmentNode;
						var dst = assignment.Target as RegisterNode;
						var t1 = assignment.Source as T1;
						var t2 = t1.LeftOperand as T2;
						var l = t2.LeftOperand as L;
						var m = t2.RightOperand as M;
						var r = t1.RightOperand as R;

						var insn = new LinkedList<Instruction> (tileMaker (dst, l, m, r));
						insn.AddLast (InstructionFactory.Move (regNode, dst));
						return insn;
					}
				);
			}

			#region multiplication

			static IEnumerable<Instruction> EffectiveMultiplication( RegisterNode dst, RegisterNode mul_reg, ConstantNode<long> mul_val)
			{
				switch (mul_val.Value) {
				case 2L:
				case 4L:
				case 8L:
					return new [] {
						InstructionFactory.Lea_Mul (dst, mul_reg, mul_val)
					};
				case 3L:
				case 5L:
				case 9L:
					var tmp_val = new ConstantNode<long> (mul_val.Value - 1L);
					return new [] {
						InstructionFactory.Lea_MulAdd (dst, mul_reg, tmp_val, mul_reg)
					};
				case 1L:
					return new [] {
						InstructionFactory.Move (dst, mul_reg)
					};
				case 0L:
					return new [] {
						InstructionFactory.Xor (dst, dst)
					};
				default:
					// dst = mul_reg * mul_val
					return new [] {
						InstructionFactory.Move (Target.RAX, mul_val), // RAX = mul_val
						InstructionFactory.Mul (mul_reg),              // RDX:RAX = mul_reg * mul_val
						InstructionFactory.Move (dst, Target.RAX)      // dst = RAX
					};
				}
			}

			// lea dst, [reg1 + val1 * reg2 + val2]
			// x = y * c,  better for c = 0,1,2,3,4,5,8,9
			public static Tile EffectiveMultiplication_RegConst() {
				return makeRegisterAssignmentBinopTile<MulOperatorNode, RegisterNode, ConstantNode<long>> (
					EffectiveMultiplication
				);
			}

			// lea dst, [reg1 + val1 * reg2 + val2]
			// x = c * y,  better for c = 0,1,2,3,4,5,8,9
			public static Tile EffectiveMultiplication_ConstReg() {
				return makeRegisterAssignmentBinopTile<MulOperatorNode, ConstantNode<long>, RegisterNode> (
					(dst, l, r) => EffectiveMultiplication (dst, r, l)
				);
			}

			#endregion

			#region addition

			// x = y + z
			public static Tile EffectiveAddition_RegReg() {
				return makeRegisterAssignmentBinopTile<AddOperatorNode, RegisterNode, RegisterNode> (
					(dst, l, r) => new [] {
						InstructionFactory.Lea_Add (dst, l, r)
					}
				);
			}

			// x = y + c
			public static Tile EffectiveAddition_RegConst<T>() {
				return makeRegisterAssignmentBinopTile<AddOperatorNode, RegisterNode, ConstantNode<T>> (
					(dst, l, r) => new [] {
						InstructionFactory.Lea_Add<T> (dst, l, r)
					}
				);
			}

			// x = c + y
			public static Tile EffectiveAddition_ConstReg<T>() {
				return makeRegisterAssignmentBinopTile<AddOperatorNode, ConstantNode<T>, RegisterNode> (
					(dst, l, r) => new [] {
						InstructionFactory.Lea_Add<T> (dst, r, l)
					}
				);
			}

			// x = y + (z + c)
			public static Tile EffectiveAddition_RegRegConst1<T>() {
				return makeRegisterAssignmentTriop1Tile<AddOperatorNode, RegisterNode, AddOperatorNode, RegisterNode, ConstantNode<T>> (
					(dst, l, m, r) => new [] {
						InstructionFactory.Lea_Add (dst, l, m, r)
					}
				);
			}

			// x = (y + z) + c
			public static Tile EffectiveAddition_RegRegConst2<T>() {
				return makeRegisterAssignmentTriop2Tile<AddOperatorNode, AddOperatorNode, RegisterNode, RegisterNode, ConstantNode<T>> (
					(dst, l, m, r) => new [] {
						InstructionFactory.Lea_Add (dst, l, m, r)
					}
				);
			}

			// x = y + (c + z)
			public static Tile EffectiveAddition_RegConstReg1<T>() {
				return makeRegisterAssignmentTriop1Tile<AddOperatorNode, RegisterNode, AddOperatorNode, ConstantNode<T>, RegisterNode> (
					(dst, l, m, r) => new [] {
						InstructionFactory.Lea_Add (dst, l, r, m)
					}
				);
			}

			// x = (y + c) + z
			public static Tile EffectiveAddition_RegConstReg2<T>() {
				return makeRegisterAssignmentTriop2Tile<AddOperatorNode, AddOperatorNode, RegisterNode, ConstantNode<T>, RegisterNode> (
					(dst, l, m, r) => new [] {
						InstructionFactory.Lea_Add (dst, l, r, m)
					}
				);
			}

			// x = c + (y + z)
			public static Tile EffectiveAddition_ConstRegReg1<T>() {
				return makeRegisterAssignmentTriop1Tile<AddOperatorNode, ConstantNode<T>, AddOperatorNode, RegisterNode, RegisterNode> (
					(dst, l, m, r) => new [] {
						InstructionFactory.Lea_Add (dst, m, r, l)
					}
				);
			}

			// x = (c + y) + z
			public static Tile EffectiveAddition_ConstRegReg2<T>() {
				return makeRegisterAssignmentTriop2Tile<AddOperatorNode, AddOperatorNode, ConstantNode<T>, RegisterNode, RegisterNode> (
					(dst, l, m, r) => new [] {
						InstructionFactory.Lea_Add (dst, m, r, l)
					}
				);
			}

			#endregion

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
		}
	}
}

