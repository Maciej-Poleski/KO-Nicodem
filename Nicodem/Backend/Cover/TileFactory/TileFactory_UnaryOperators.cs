using Nicodem.Backend.Representation;
using System;
using System.Collections.Generic;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		public static class Unop 
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

			public static Tile LogNot_Reg() {
				return makeUnopTile<LogNotOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Cmp (left, new ConstantNode<long> (0L)),  // cmp(l, 0)
						InstructionFactory.Sete (regNode)                            // if l = 0 then reg = 1 else reg = 0
					}
				);
			}

			public static Tile BinNot_Reg() {
				return makeUnopTile<BinNotOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Move (regNode, left),
						InstructionFactory.Not (regNode)
					}
				);
			}

			public static Tile Neg_Reg() {
				return makeUnopTile<NegOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Move (regNode, left),
						InstructionFactory.Neg (regNode)
					}
				);
			}

			public static Tile Plus_Reg() {
				return makeUnopTile<UnaryPlusOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Move (regNode, left)
					}
				);
			}

			// -x = 0-x
			public static Tile Minus_Reg() {
				return makeUnopTile<UnaryMinusOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Xor (regNode, regNode),
						InstructionFactory.Sub (regNode, left),
					}
				);
			}

			public static Tile Inc_Reg() {
				return makeUnopTile<IncOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Inc (left),
						InstructionFactory.Move (regNode, left)
					}
				);
			}

			public static Tile Dec_Reg() {
				return makeUnopTile<DecOperatorNode, RegisterNode> (
					(regNode, root, left) => new [] {
						InstructionFactory.Dec (left),
						InstructionFactory.Move (regNode, left)
					}
				);
			}
		}
	}
}

