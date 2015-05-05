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

		static Tile makeBinopRegRegTile<T>( string mnemonik ) 
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, RegisterNode, RegisterNode> (
				(regNode, root, left, right) => new[] {
					new Instruction (
						map => string.Format ("{0} {1}, {2}", mnemonik, map [left], map [right]),
						use (left, right), define (left)),
					copyToTemporary (regNode, left)
				}
			);
		}

		static Tile makeBinopRegConstTile<T>( string mnemonik )
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, RegisterNode, ConstantNode<long>> (
				(regNode, root, left, right) => new[] {
					new Instruction (
						map => string.Format ("{0} {1}, {2}", mnemonik, map [left], right.Value),
						use (left), define (left)),
					copyToTemporary (regNode, left)
				}
			);
		}

		public static class Add 
		{
			public static readonly string Mnemonik = "add";

			// add reg64, reg64
			public static Tile RegReg() {
				return makeBinopRegRegTile<AddOperatorNode> (Add.Mnemonik);
			}

			public static Tile RegMem() {
				throw new NotImplementedException();
			}

			// add reg64, imm64
			public static Tile RegConst() {
				return makeBinopRegConstTile<AddOperatorNode> (Add.Mnemonik);
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
			public static readonly string Mnemonik = "sub";

			// sub reg64, reg64
			public static Tile RegReg() {
				return makeBinopRegRegTile<SubOperatorNode> (Sub.Mnemonik);
			}

			public static Tile RegMem() {
				throw new NotImplementedException();
			}

			// sub reg64, imm64
			public static Tile RegConst() {
				return makeBinopRegConstTile<SubOperatorNode> (Sub.Mnemonik);
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
			public static readonly string Mnemonik = "imul";

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
			public static readonly string Mnemonik = "idiv";
		}

		public static class Mod
		{
			public static readonly string Mnemonik = "idiv";
		}

		public static class Shl
		{
		}

		public static class Shr
		{
		}

		public static class BitAnd
		{
		}

		public static class BitOr
		{
		}

		public static class BitXor
		{
			public static readonly string Mnemonik = "xor";

			public static Tile RegReg() {
				return makeBinopRegRegTile<BitXorOperatorNode> (BitXor.Mnemonik);
			}

			public static Tile RegConst() {
				return makeBinopRegConstTile<BitXorOperatorNode> (BitXor.Mnemonik);
			}
		}

		public static class LogAnd
		{
			public static readonly string Mnemonik = "and";

			public static Tile RegReg() {
				return makeBinopRegRegTile<LogAndOperatorNode> (LogAnd.Mnemonik);
			}

			public static Tile RegConst() {
				return makeBinopRegConstTile<LogAndOperatorNode> (LogAnd.Mnemonik);
			}
		}

		public static class LogOr
		{
			public static readonly string Mnemonik = "or";

			public static Tile RegReg() {
				return makeBinopRegRegTile<LogOrOperatorNode> (LogOr.Mnemonik);
			}

			public static Tile RegConst() {
				return makeBinopRegConstTile<LogOrOperatorNode> (LogOr.Mnemonik);
			}
		}
	}
}

