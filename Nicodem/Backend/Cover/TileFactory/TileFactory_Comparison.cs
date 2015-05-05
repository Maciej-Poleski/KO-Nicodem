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
					InstructionFactory.Xor (regNode, regNode),
					InstructionFactory.Cmp (left, right),
					InstructionFactory.Cmov (mnemonik, regNode, 1L)
				}
			);
		}

		static Tile makeCmpRegConstTile<T>( string mnemonik ) 
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, RegisterNode, ConstantNode<long>> (
				(regNode, root, left, right) => new[] {
					InstructionFactory.Xor (regNode, regNode),
					InstructionFactory.Cmp (left, right),
					InstructionFactory.Cmov (mnemonik, regNode, 1L)
				}
			);
		}

		static Tile makeCmpConstConstTile<T>( string mnemonik ) 
			where T : BinaryOperatorNode
		{
			return makeBinopTile<T, ConstantNode<long>, ConstantNode<long>> (
				(regNode, root, left, right) => new[] {
					InstructionFactory.Xor (regNode, regNode),
					new Instruction (
						map => string.Format ("cmp {0}, {1}", left.Value, right.Value),
						use (), define ()),
					InstructionFactory.Cmov (mnemonik, regNode, 1L)
				}
			);
		}

		public static class Lt
		{
			public static readonly string Mnemonik = "l";

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
			public static readonly string Mnemonik = "le";

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
			public static readonly string Mnemonik = "g";

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
			public static readonly string Mnemonik = "ge";

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
			public static readonly string Mnemonik = "e";

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
			public static readonly string Mnemonik = "ne";

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

