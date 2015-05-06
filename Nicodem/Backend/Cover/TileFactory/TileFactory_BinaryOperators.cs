using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		static Tile makeBinopTile<T,L,R>( Func<RegisterNode, T, L, R, IEnumerable<Instruction> > tileMaker )
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
					return tileMaker(regNode, root, left, right);
				}
			);
		}

		public static class Add 
		{
			public static Tile RegReg() {
				return makeBinopTile<AddOperatorNode, RegisterNode, RegisterNode> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Add (regNode, right)   // res = left + right
					}
				);
			}

			public static Tile RegMem() {
				throw new NotImplementedException();
			}

			public static Tile RegConst<T>() {
				return makeBinopTile<AddOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Add (regNode, right)   // res = left + right
					}
				);
			}

			public static Tile MemReg() {
				throw new NotImplementedException();
			}

			public static Tile MemConst() {
				throw new NotImplementedException();
			}
		}

		public static class Sub
		{
			public static Tile RegReg() {
				return makeBinopTile<SubOperatorNode, RegisterNode, RegisterNode> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Sub (regNode, right)   // res = left - right
					}
				);
			}

			public static Tile RegMem() {
				throw new NotImplementedException();
			}
				
			public static Tile RegConst<T>() {
				return makeBinopTile<SubOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Sub (regNode, right)   // res = left - right
					}
				);
			}

			public static Tile MemReg() {
				throw new NotImplementedException();
			}

			public static Tile MemConst() {
				throw new NotImplementedException();
			}
		}

		public static class Mul
		{
			public static Tile RegReg() {
				return makeBinopTile<MulOperatorNode, RegisterNode, RegisterNode> (
					(regNode, root, left, right) => new[] {
						new Instruction ( //TODO define rax !!!
							map => string.Format ("mov rax, {0}", map [left]),
							use (left), define (), true),

						// wynik rdx:rax
						new Instruction ( //TODO define rax rdx
							map => string.Format ("imul {0}", map [right]),
							use (left, right), define (left)),

						new Instruction ( //TODO use rax !!!
							map => string.Format ("mov {0}, rax", map [regNode]),
							use (left, regNode), define (regNode), true)
					}
				);
			}
		}

		public static class Div
		{
		}

		public static class Mod
		{
		}

		public static class Shl
		{
			public static Tile RegConst<T>() {
				return makeBinopTile<ShlOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Shl (regNode, right)   // res = left << right
					}
				);
			}
		}

		public static class Shr
		{
			public static Tile RegConst<T>() {
				return makeBinopTile<ShrOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Shr (regNode, right)   // res = left >> right
					}
				);
			}
		}

		public static class BitAnd
		{
		}

		public static class BitOr
		{
		}

		public static class BitXor
		{
			public static Tile RegReg() {
				return makeBinopTile<BitXorOperatorNode, RegisterNode, RegisterNode> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Xor (regNode, right)   // res = xor(left, right)
					}
				);
			}

			public static Tile RegConst<T>() {
				return makeBinopTile<BitXorOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Xor (regNode, right)   // res = xor(left, right)
					}
				);
			}
		}

		public static class LogAnd
		{
			public static Tile RegReg() {
				return makeBinopTile<LogAndOperatorNode, RegisterNode, RegisterNode> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.And (regNode, right)   // res = left && right
					}
				);
			}

			public static Tile RegConst<T>() {
				return makeBinopTile<LogAndOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.And (regNode, right)   // res = left && right
					}
				);
			}
		}

		public static class LogOr
		{
			public static Tile RegReg() {
				return makeBinopTile<LogOrOperatorNode, RegisterNode, RegisterNode> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Or (regNode, right)    // res = left || right
					}
				);
			}

			public static Tile RegConst<T>() {
				return makeBinopTile<LogOrOperatorNode, RegisterNode, ConstantNode<T>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Move (regNode, left),  // res = left
						InstructionFactory.Or (regNode, right)   // res = left || right
					}
				);
			}
		}
	}
}

