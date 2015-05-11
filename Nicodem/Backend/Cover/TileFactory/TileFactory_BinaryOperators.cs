using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		static Tile makeBinopTile<T,L,R>( Func<RegisterNode, L, R, IEnumerable<Instruction> > tileMaker )
			where T : BinaryOperatorNode
			where L : Node
			where R : Node
		{
			return new Tile (typeof(T),
				new[] { 
					makeTile<L>(),
					makeTile<R>()
				},
				(regNode, node) => {
					var root = node as T;
					var left = root.LeftOperand as L;
					var right = root.RightOperand as R;
					return tileMaker(regNode, left, right);
				}
			);
		}

		static Tile makeBinopRegRegTile<T>( Func<RegisterNode,RegisterNode,Instruction> specificInsn )
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, RegisterNode, RegisterNode> (
				(regNode, left, right) => new[] {
					InstructionFactory.Move (regNode, left),
					specificInsn (regNode, right)
				}
			);
		}

		static Tile makeBinopRegConstTile<T,C>( Func<RegisterNode,ConstantNode<C>,Instruction> specificInsn )
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, RegisterNode, ConstantNode<C>> (
				(regNode, left, right) => new[] {
					InstructionFactory.Move (regNode, left),
					specificInsn (regNode, right)
				}
			);
		}

		static Tile makeBinopConstRegTile<T,C>( Func<RegisterNode,RegisterNode,Instruction> specificInsn )
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, ConstantNode<C>, RegisterNode> (
				(regNode, left, right) => new[] {
					InstructionFactory.Move (regNode, left),
					specificInsn (regNode, right)
				}
			);
		}

		public static class Add 
		{
			public static Tile RegReg() {
				return makeBinopRegRegTile<AddOperatorNode> (InstructionFactory.Add);
			}

			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<AddOperatorNode,T> (InstructionFactory.Add);
			}

			public static Tile ConstReg<T>() {
				return makeBinopConstRegTile<AddOperatorNode,T> (InstructionFactory.Add);
			}
		}

		public static class Sub
		{
			public static Tile RegReg() {
				return makeBinopRegRegTile<SubOperatorNode> (InstructionFactory.Sub);
			}
				
			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<SubOperatorNode,T> (InstructionFactory.Sub);
			}

			public static Tile ConstReg<T>() {
				return makeBinopConstRegTile<SubOperatorNode,T> (InstructionFactory.Sub);
			}
		}

		public static class Mul
		{
			public static Tile RegReg() {
				return makeBinopTile<MulOperatorNode, RegisterNode, RegisterNode> (
					(regNode, left, right) => new[] {
						InstructionFactory.Move (Target.RAX, left),   // RAX = left
						InstructionFactory.Mul (right),               // RDX:RAX = left * right
						InstructionFactory.Move (regNode, Target.RAX) // result = RAX
					}
				);
			}

			public static Tile RegConst<T>() {
				return makeBinopTile<MulOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, left, right) => new[] {
						InstructionFactory.Move (Target.RAX, right),  // RAX = right
						InstructionFactory.Mul (left),                // RDX:RAX = left * right
						InstructionFactory.Move (regNode, Target.RAX) // result = RAX
					}
				);
			}

			public static Tile ConstReg<T>() {
				return makeBinopTile<MulOperatorNode, ConstantNode<T>, RegisterNode> (
					(regNode, left, right) => new[] {
						InstructionFactory.Move (Target.RAX, left),   // RAX = left
						InstructionFactory.Mul (right),               // RDX:RAX = left * right
						InstructionFactory.Move (regNode, Target.RAX) // result = RAX
					}
				);
			}
		}

		public static class Div
		{
			public static Tile RegReg() {
				return makeBinopTile<DivOperatorNode, RegisterNode, RegisterNode> (
					(regNode, left, right) => new[] {
						InstructionFactory.Xor (Target.RDX, Target.RDX), // RDX = 0
						InstructionFactory.Move (Target.RAX, left),      // RDX:RAX = left
						InstructionFactory.Div (right),                  // RAX = left / right
						InstructionFactory.Move (regNode, Target.RAX)    // result = RAX
					}
				);
			}

			public static Tile RegConst<T>() {
				return makeBinopTile<DivOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, left, right) => new[] {
						InstructionFactory.Xor (Target.RDX, Target.RDX), // RDX = 0
						InstructionFactory.Move (Target.RAX, left),      // RDX:RAX = left
						InstructionFactory.Move (regNode, right),        // reg = rigth
						InstructionFactory.Div (regNode),                // RAX = left / reg = left / right
						InstructionFactory.Move (regNode, Target.RAX)    // result = RAX
					}
				);
			}

			public static Tile ConstReg<T>() {
				return makeBinopTile<DivOperatorNode, ConstantNode<T>, RegisterNode> (
					(regNode, left, right) => new[] {
						InstructionFactory.Xor (Target.RDX, Target.RDX), // RDX = 0
						InstructionFactory.Move (Target.RAX, left),      // RDX:RAX = left
						InstructionFactory.Div (right),                  // RAX = left / right
						InstructionFactory.Move (regNode, Target.RAX)    // result = RAX
					}
				);
			}
		}

		public static class Mod
		{
			public static Tile RegReg() {
				return makeBinopTile<ModOperatorNode, RegisterNode, RegisterNode> (
					(regNode, left, right) => new[] {
						InstructionFactory.Xor (Target.RDX, Target.RDX), // RDX = 0
						InstructionFactory.Move (Target.RAX, left),      // RDX:RAX = left
						InstructionFactory.Div (right),                  // RDX = left % right
						InstructionFactory.Move (regNode, Target.RDX)    // result = RDX
					}
				);
			}

			public static Tile RegConst<T>() {
				return makeBinopTile<ModOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, left, right) => new[] {
						InstructionFactory.Xor (Target.RDX, Target.RDX), // RDX = 0
						InstructionFactory.Move (Target.RAX, left),      // RDX:RAX = left
						InstructionFactory.Move (regNode, right),        // reg = rigth
						InstructionFactory.Div (regNode),                // RDX = left % reg = left % right
						InstructionFactory.Move (regNode, Target.RDX)    // result = RDX
					}
				);
			}

			public static Tile ConstReg<T>() {
				return makeBinopTile<ModOperatorNode, ConstantNode<T>, RegisterNode> (
					(regNode, left, right) => new[] {
						InstructionFactory.Xor (Target.RDX, Target.RDX), // RDX = 0
						InstructionFactory.Move (Target.RAX, left),      // RDX:RAX = left
						InstructionFactory.Div (right),                  // RDX = left % right
						InstructionFactory.Move (regNode, Target.RDX)    // result = RDX
					}
				);
			}
		}

		public static class Shl
		{
			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<ShlOperatorNode,T> (InstructionFactory.Shl);
			}
		}

		public static class Shr
		{
			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<ShrOperatorNode,T> (InstructionFactory.Shr);
			}
		}

		public static class BitAnd
		{
			public static Tile RegReg() {
				return makeBinopRegRegTile<BitAndOperatorNode> (InstructionFactory.And);
			}

			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<BitAndOperatorNode,T> (InstructionFactory.And);
			}

			public static Tile ConstReg<T>() {
				return makeBinopConstRegTile<BitAndOperatorNode,T> (InstructionFactory.And);
			}
		}

		public static class BitOr
		{
			public static Tile RegReg() {
				return makeBinopRegRegTile<BitOrOperatorNode> (InstructionFactory.Or);
			}

			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<BitOrOperatorNode,T> (InstructionFactory.Or);
			}

			public static Tile ConstReg<T>() {
				return makeBinopConstRegTile<BitOrOperatorNode,T> (InstructionFactory.Or);
			}
		}

		public static class BitXor
		{
			public static Tile RegReg() {
				return makeBinopRegRegTile<BitXorOperatorNode> (InstructionFactory.Xor);
			}

			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<BitXorOperatorNode,T> (InstructionFactory.Xor);
			}

			public static Tile ConstReg<T>() {
				return makeBinopConstRegTile<BitXorOperatorNode,T> (InstructionFactory.Xor);
			}
		}

		// x > 0 if l > 0 and r > 0
		// 0 otherwise
		public static class LogAnd
		{
			public static Tile RegReg() {
				// l = 0            ->  first instruction sets reg to 0
				// r = 0 but l > 0  ->  fourth instruction sets reg to 0
				// l > 0 and r > 0  ->  first instruction sets reg = l > 0 and nothing changes
				return makeBinopTile<LogAndOperatorNode, RegisterNode, RegisterNode> (
					(regNode, left, right) => new[] {
						InstructionFactory.Move (regNode, left),   // reg = left
						InstructionFactory.Xor (left, left),       // left = 0
						InstructionFactory.Cmp (right, left),      // if right == 0
						InstructionFactory.Cmove (regNode, left)   // then reg = 0
					}
				);
			}

			public static Tile RegConst() {
				return makeBinopTile<LogAndOperatorNode, RegisterNode, ConstantNode<long>> (
					(regNode, left, right) => right.Value == 0L
					? new[] { InstructionFactory.Xor (regNode, regNode) } // r == 0 -> all = 0
					: new[] { InstructionFactory.Move (regNode, left) }   // r != 0 -> all = l
				);
			}

			public static Tile ConstReg() {
				return makeBinopTile<LogAndOperatorNode, ConstantNode<long>, RegisterNode> (
					(regNode, left, right) => left.Value == 0L
					? new[] { InstructionFactory.Xor (regNode, regNode) }  // l == 0 -> all = 0
					: new[] { InstructionFactory.Move (regNode, right) }   // l != 0 -> all = r
				);
			}
		}

		// x > 0 if l > 0 or r > 0
		// 0 otherwise
		// is equal to BitOr operator !!!
		public static class LogOr
		{
			public static Tile RegReg() {
				return makeBinopRegRegTile<LogOrOperatorNode> (InstructionFactory.Or);
			}

			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<LogOrOperatorNode,T> (InstructionFactory.Or);
			}

			public static Tile ConstReg<T>() {
				return makeBinopConstRegTile<LogOrOperatorNode,T> (InstructionFactory.Or);
			}
		}
	}
}

