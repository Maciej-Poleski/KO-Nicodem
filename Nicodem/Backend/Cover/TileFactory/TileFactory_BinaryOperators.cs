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
		}

		public static class BitOr
		{
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

		public static class LogAnd
		{
			public static Tile RegReg() {
				return makeBinopRegRegTile<LogAndOperatorNode> (InstructionFactory.And);
			}

			public static Tile RegConst<T>() {
				return makeBinopRegConstTile<LogAndOperatorNode,T> (InstructionFactory.And);
			}

			public static Tile ConstReg<T>() {
				return makeBinopConstRegTile<LogAndOperatorNode,T> (InstructionFactory.And);
			}
		}

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

