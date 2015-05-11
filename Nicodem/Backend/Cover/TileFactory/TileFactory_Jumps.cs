using Nicodem.Backend.Representation;
using System;
using System.Collections.Generic;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		public static class Jump
		{
			#region unconditional

			public static Tile Unconditional() {
				return new Tile (typeof(UnconditionalJumpToLabelNode),
					new [] {
						makeTile<LabelNode> ()
					},
					(regNode, node) => {
						var root = node as UnconditionalJumpToLabelNode;
						return new[] {
							InstructionFactory.Jmp (root.NextNode)
						};
					}
				);
			}

			#endregion

			#region conditional

			static Tile conditionalWithCmp<T,L,R>( Func<T,L,R,LabelNode,IEnumerable<Instruction>> tileBuilder )
				where T : BinaryOperatorNode
				where L : Node
				where R : Node
			{
				return new Tile (typeof(ConditionalJumpToLabelNode),
					new [] {
						makeTile<T>(
							makeTile<L>(),
							makeTile<R>()
						),
						makeTile<LabelNode> ()
					},
					(regNode, node) => {
						var root = node as ConditionalJumpToLabelNode;
						var condition = root.Condition as T;
						var cond_l = condition.LeftOperand as L;
						var cond_r = condition.RightOperand as R;
						var lbl = root.NextNode;

						return tileBuilder(condition, cond_l, cond_r, lbl);
					}
				);
			}

			#region reg - reg

			static Tile conditionalWithCmpRegReg<T>( string cond_type ) 
				where T : BinaryOperatorNode
			{
				return conditionalWithCmp<T, RegisterNode, RegisterNode> (
					(root, reg1, reg2, lbl) => new[] {
						InstructionFactory.Cmp (reg1, reg2),
						InstructionFactory.Jump (cond_type, lbl)
					}
				);
			}

			public static Tile Cond_RegReg_Lt() {
				return conditionalWithCmpRegReg<LtOperatorNode> ("l");
			}

			public static Tile Cond_RegReg_Le() {
				return conditionalWithCmpRegReg<LteOperatorNode> ("le");
			}

			public static Tile Cond_RegReg_Gt() {
				return conditionalWithCmpRegReg<GtOperatorNode> ("g");
			}

			public static Tile Cond_RegReg_Ge() {
				return conditionalWithCmpRegReg<GteOperatorNode> ("ge");
			}

			public static Tile Cond_RegReg_Eq() {
				return conditionalWithCmpRegReg<EqOperatorNode> ("e");
			}

			public static Tile Cond_RegReg_Neq() {
				return conditionalWithCmpRegReg<NeqOperatorNode> ("ne");
			}

			#endregion

			#region reg - const

			static Tile conditionalWithCmpRegConst<T,C>( string cond_type ) 
				where T : BinaryOperatorNode
			{
				return conditionalWithCmp<T, RegisterNode, ConstantNode<C>> (
					(root, reg1, con, lbl) => new[] {
						InstructionFactory.Cmp (reg1, con),
						InstructionFactory.Jump (cond_type, lbl)
					}
				);
			}

			public static Tile Cond_RegConst_Lt<C>() {
				return conditionalWithCmpRegConst<LtOperatorNode, C> ("l");
			}

			public static Tile Cond_RegConst_Le<C>() {
				return conditionalWithCmpRegConst<LteOperatorNode, C> ("le");
			}

			public static Tile Cond_RegConst_Gt<C>() {
				return conditionalWithCmpRegConst<GtOperatorNode, C> ("g");
			}

			public static Tile Cond_RegConst_Ge<C>() {
				return conditionalWithCmpRegConst<GteOperatorNode, C> ("ge");
			}

			public static Tile Cond_RegConst_Eq<C>() {
				return conditionalWithCmpRegConst<EqOperatorNode, C> ("e");
			}

			public static Tile Cond_RegConst_Neq<C>() {
				return conditionalWithCmpRegConst<NeqOperatorNode, C> ("ne");
			}

			#endregion

			#region const - reg

			static Tile conditionalWithCmpConstReg<T,C>( string cond_type ) 
				where T : BinaryOperatorNode
			{
				return conditionalWithCmp<T, ConstantNode<C>, RegisterNode> (
					(root, reg1, con, lbl) => new[] {
						InstructionFactory.Cmp (reg1, con),
						InstructionFactory.Jump (cond_type, lbl)
					}
				);
			}

			public static Tile Cond_ConstReg_Lt<C>() {
				return conditionalWithCmpConstReg<LtOperatorNode, C> ("l");
			}

			public static Tile Cond_ConstReg_Le<C>() {
				return conditionalWithCmpConstReg<LteOperatorNode, C> ("le");
			}

			public static Tile Cond_ConstReg_Gt<C>() {
				return conditionalWithCmpConstReg<GtOperatorNode, C> ("g");
			}

			public static Tile Cond_ConstReg_Ge<C>() {
				return conditionalWithCmpConstReg<GteOperatorNode, C> ("ge");
			}

			public static Tile Cond_ConstReg_Eq<C>() {
				return conditionalWithCmpConstReg<EqOperatorNode, C> ("e");
			}

			public static Tile Cond_ConstReg_Neq<C>() {
				return conditionalWithCmpConstReg<NeqOperatorNode, C> ("ne");
			}

			#endregion

			#endregion
		}
	}
}

