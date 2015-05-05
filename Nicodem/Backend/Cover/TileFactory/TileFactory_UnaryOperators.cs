using Nicodem.Backend.Representation;
using System;
using System.Collections.Generic;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		static Tile makeUnopTile<T,L>( Func<RegisterNode, T, L, IEnumerable<Instruction> > tileMaker )
			where T : UnaryOperatorNode
			where L : Node
		{
			return new Tile (typeof(T),
				new[] { 
					makeTile<L>()
				},
				(regNode, node) => {
					var root = node as T;
					var left = root.Operand as L;
					return tileMaker(regNode, root, left);
				}
			);
		}

		public static class LogNot
		{
		}

		public static class BinNot
		{
			public static Tile Reg() {
				return makeUnopTile<BinNotOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Move (regNode, left),
						InstructionFactory.Not (regNode)
					}
				);
			}
		}

		public static class Neg
		{
			public static Tile Reg() {
				return makeUnopTile<NegOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Move (regNode, left),
						InstructionFactory.Neg (regNode)
					}
				);
			}
		}
	}
}

