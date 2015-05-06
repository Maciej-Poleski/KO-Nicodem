using Nicodem.Backend.Representation;
using System.Collections.Generic;
using System;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		public static class Compare
		{
			static Tile compare<T,L,R>( Func<RegisterNode,T,L,R,IEnumerable<Instruction>> tileBuilder )
				where T : BinaryOperatorNode
				where L : Node
				where R : Node
			{
				return new Tile (typeof(T),
					new [] {
						makeTile<L>(),
						makeTile<R>()
					},
					(regNode, node) => {
						var root = node as T;
						var cond_l = root.LeftOperand as L;
						var cond_r = root.RightOperand as R;

						return tileBuilder(regNode, root, cond_l, cond_r);
					}
				);
			}

			#region reg - reg

			static Tile compareRegReg<T>( string cond_type ) 
				where T : BinaryOperatorNode
			{
				return compare<T,RegisterNode,RegisterNode> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Xor (regNode, regNode),
						InstructionFactory.Cmp (left, right),
						InstructionFactory.Cmov (cond_type, regNode, 1L)
					}
				);
			}

			public static Tile RegReg_Lt() {
				return compareRegReg<LtOperatorNode> ("l");
			}

			public static Tile RegReg_Le() {
				return compareRegReg<LteOperatorNode> ("le");
			}

			public static Tile RegReg_Gt() {
				return compareRegReg<GtOperatorNode> ("g");
			}

			public static Tile RegReg_Ge() {
				return compareRegReg<GteOperatorNode> ("ge");
			}

			public static Tile RegReg_Eq() {
				return compareRegReg<EqOperatorNode> ("e");
			}

			public static Tile RegReg_Neq() {
				return compareRegReg<NeqOperatorNode> ("ne");
			}

			#endregion

			#region reg - const

			static Tile compareRegConst<T, C>( string cond_type ) 
				where T : BinaryOperatorNode
			{
				return compare<T,RegisterNode,ConstantNode<C>> (
					(regNode, root, left, right) => new[] {
						InstructionFactory.Xor (regNode, regNode),
						InstructionFactory.Cmp (left, right),
						InstructionFactory.Cmov (cond_type, regNode, 1L)
					}
				);
			}

			public static Tile RegConst_Lt<C>() {
				return compareRegConst<LtOperatorNode, C> ("l");
			}

			public static Tile RegConst_Le<C>() {
				return compareRegConst<LteOperatorNode, C> ("le");
			}

			public static Tile RegConst_Gt<C>() {
				return compareRegConst<GtOperatorNode, C> ("g");
			}

			public static Tile RegConst_Ge<C>() {
				return compareRegConst<GteOperatorNode, C> ("ge");
			}

			public static Tile RegConst_Eq<C>() {
				return compareRegConst<EqOperatorNode, C> ("e");
			}

			public static Tile RegConst_Neq<C>() {
				return compareRegConst<NeqOperatorNode, C> ("ne");
			}

			#endregion
		}
	}
}

