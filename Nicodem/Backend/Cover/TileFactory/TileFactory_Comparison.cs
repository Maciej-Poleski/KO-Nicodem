using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		static Tile makeCmpRegRegTile<T>( string mnemonik ) 
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, RegisterNode, RegisterNode> (
				(regNode, root, left, right) => new[] {
					new Instruction (
						map => string.Format ("xor {0}, {0}", map [regNode]),
						use (regNode), define (regNode)),
					new Instruction (
						map => string.Format ("cmp {0}, {1}", map [left], map [right]),
						use (left, right), define ()),
					new Instruction (
						map => string.Format ("{0} {1}, 1", mnemonik, map [regNode]),
						use (regNode), define (regNode))
				}
			);
		}

		static Tile makeCmpRegConstTile<T>( string mnemonik ) 
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, RegisterNode, ConstantNode<long>> (
				(regNode, root, left, right) => new[] {
					new Instruction (
						map => string.Format ("xor {0}, {0}", map [regNode]),
						use (regNode), define (regNode)),
					new Instruction (
						map => string.Format ("cmp {0}, {1}", map [left], right.Value),
						use (left), define ()),
					new Instruction (
						map => string.Format ("{0} {1}, 1", mnemonik, map [regNode]),
						use (regNode), define (regNode))
				}
			);
		}

		static Tile makeCmpConstConstTile<T>( string mnemonik ) 
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, ConstantNode<long>, ConstantNode<long>> (
				(regNode, root, left, right) => new[] {
					new Instruction (
						map => string.Format ("xor {0}, {0}", map [regNode]),
						use (regNode), define (regNode)),
					new Instruction (
						map => string.Format ("cmp {0}, {1}", left.Value, right.Value),
						use (), define ()),
					new Instruction (
						map => string.Format ("{0} {1}, 1", mnemonik, map [regNode]),
						use (regNode), define (regNode))
				}
			);
		}

		public static class Lt
		{
			public static readonly string Mnemonik = "cmovl";

			public static Tile RegReg() {
				return makeCmpRegRegTile<LtOperatorNode> (Lt.Mnemonik);
			}

			public static Tile RegConst() {
				return makeCmpRegConstTile<LtOperatorNode> (Lt.Mnemonik);
			}

			public static Tile ConstConst() {
				return makeCmpConstConstTile<LtOperatorNode> (Lt.Mnemonik);
			}
		}

		public static class Le
		{
			public static readonly string Mnemonik = "cmovle";

			public static Tile RegReg() {
				return makeCmpRegRegTile<LteOperatorNode> (Le.Mnemonik);
			}

			public static Tile RegConst() {
				return makeCmpRegConstTile<LteOperatorNode> (Le.Mnemonik);
			}

			public static Tile ConstConst() {
				return makeCmpConstConstTile<LteOperatorNode> (Le.Mnemonik);
			}
		}

		public static class Gt
		{
			public static readonly string Mnemonik = "cmovg";

			public static Tile RegReg() {
				return makeCmpRegRegTile<GtOperatorNode> (Gt.Mnemonik);
			}

			public static Tile RegConst() {
				return makeCmpRegConstTile<GtOperatorNode> (Gt.Mnemonik);
			}

			public static Tile ConstConst() {
				return makeCmpConstConstTile<GtOperatorNode> (Gt.Mnemonik);
			}
		}

		public static class Ge
		{
			public static readonly string Mnemonik = "cmovge";

			public static Tile RegReg() {
				return makeCmpRegRegTile<GteOperatorNode> (Ge.Mnemonik);
			}

			public static Tile RegConst() {
				return makeCmpRegConstTile<GteOperatorNode> (Ge.Mnemonik);
			}

			public static Tile ConstConst() {
				return makeCmpConstConstTile<GteOperatorNode> (Ge.Mnemonik);
			}
		}

		public static class Eq
		{
			public static readonly string Mnemonik = "cmove";

			public static Tile RegReg() {
				return makeCmpRegRegTile<EqOperatorNode> (Eq.Mnemonik);
			}

			public static Tile RegConst() {
				return makeCmpRegConstTile<EqOperatorNode> (Eq.Mnemonik);
			}

			public static Tile ConstConst() {
				return makeCmpConstConstTile<EqOperatorNode> (Eq.Mnemonik);
			}
		}

		public static class Neq
		{
			public static readonly string Mnemonik = "cmovne";

			public static Tile RegReg() {
				return makeCmpRegRegTile<NeqOperatorNode> (Neq.Mnemonik);
			}

			public static Tile RegConst() {
				return makeCmpRegConstTile<NeqOperatorNode> (Neq.Mnemonik);
			}

			public static Tile ConstConst() {
				return makeCmpConstConstTile<NeqOperatorNode> (Neq.Mnemonik);
			}
		}
	}
}

