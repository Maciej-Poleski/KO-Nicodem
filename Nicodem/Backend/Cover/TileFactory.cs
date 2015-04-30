using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public static class TileFactory
	{
		#region Basic Tiles

		static Func<RegisterNode, Node, IEnumerable<Instruction>> noInstructions() {
			return (arg1, arg2) => new Instruction[0];
		}

		static IEnumerable<Tile> noChildren() {
			return new Tile[0];
		}

		static IEnumerable<RegisterNode> use(params RegisterNode[] nodes) {
			return nodes;
		}

		static IEnumerable<RegisterNode> define(params RegisterNode[] nodes) {
			return nodes;
		}

		static Instruction copyToTemporary( RegisterNode temporary, RegisterNode source ) {
			return new Instruction (
				map => string.Format ("MOV {0} {1}", map[temporary], map[source]),
				use (temporary, source), 
				define (temporary), 
				true
			);
		}

		static Tile LeafTile<T>() {
			return new Tile (typeof(T), noChildren (), noInstructions ()); 
		}

		#endregion

		#region MOV

		// AssignmentNode -> { 
		//	RegisterNode, 
		//	RegisterNode 
		// }
		public static Tile MOV_Reg_Reg() {
			return new Tile (typeof(AssignmentNode),
				new[] { 
					LeafTile<RegisterNode>(),
					LeafTile<RegisterNode>()
				},
				(regNode, node) => {
					var root = node as AssignmentNode;
					var target = root.Target as RegisterNode;
					var source = root.Source as RegisterNode;

					return new [] {
						// mov target source  (isCopy instruction)
						new Instruction (
							map => string.Format("MOV {0} {1}", map[target], map[source]),
							use(target, source),
							define(target),
							true),

						copyToTemporary( regNode, target )
					};
				}
			);
		}

		#endregion

		#region Arithmetic operations

		// AddOperatorNode -> { 
		//	RegisterNode, 
		//	RegisterNode 
		// }
		public static Tile ADD_Reg_Reg() {
			return new Tile (typeof(AddOperatorNode),
				new[] { 
					LeafTile<RegisterNode>(),
					LeafTile<RegisterNode>()
				},
				(regNode, node) => {
					var root = node as AddOperatorNode;
					var dst = root.LeftOperand as RegisterNode;
					var src = root.RightOperand as RegisterNode;

					return new [] {
						// add dst src
						new Instruction (
							map => string.Format("ADD {0} {1}", map[dst], map[src]),
							use(dst, src), define(dst)),

						copyToTemporary( regNode, dst )
					};
				}
			);
		}

		#endregion
	}
}

