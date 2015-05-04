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
				map => string.Format ("mov {0}, {1}", map[temporary], map[source]),
				use (temporary, source), 
				define (temporary), 
				true
			);
		}

		static Tile makeTile<T>(params Tile[] children) {
			return new Tile (typeof(T), children, noInstructions ());
		}

		#endregion

		#region MOV

		// AssignmentNode -> { 
		//   RegisterNode, 
		//   RegisterNode 
		// }
		public static Tile MOV_Reg_Reg() {
			return new Tile (typeof(AssignmentNode),
				new[] { 
					makeTile<RegisterNode>(),
					makeTile<RegisterNode>()
				},
				(regNode, node) => {
					var root = node as AssignmentNode;
					var target = root.Target as RegisterNode;
					var source = root.Source as RegisterNode;

					return new [] {
						// mov target, source  (isCopy instruction)
						new Instruction (
							map => string.Format("mov {0}, {1}", map[target], map[source]),
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
		//   RegisterNode, 
		//   RegisterNode 
		// }
		public static Tile ADD_Reg_Reg() {
			return new Tile (typeof(AddOperatorNode),
				new[] { 
					makeTile<RegisterNode>(),
					makeTile<RegisterNode>()
				},
				(regNode, node) => {
					var root = node as AddOperatorNode;
					var dst = root.LeftOperand as RegisterNode;
					var src = root.RightOperand as RegisterNode;

					return new [] {
						// add dst, src
						new Instruction (
							map => string.Format("add {0}, {1}", map[dst], map[src]),
							use(dst, src), define(dst)),

						copyToTemporary( regNode, dst )
					};
				}
			);
		}

		#endregion

		#region Memory addressing

		// AssignmentNode -> {
		//   MemoryNode -> {
		//     RegisterNode
		//     AddOperatorNode -> {
		//       RegisterNode
		//       RegisterNode
		//     }
		//   }
		// }
		public static Tile LEA_Reg_Reg_Reg() {
			return new Tile (typeof(AssignmentNode),
				new[] {
					makeTile<RegisterNode>(),
					makeTile<MemoryNode>(
						makeTile<AddOperatorNode>(
							makeTile<RegisterNode>(),
							makeTile<RegisterNode>()
						)
					)
				},
				(regNode, node) => {
					var assignment = node as AssignmentNode;
					var dst = assignment.Target as RegisterNode;
					var mem = assignment.Source as MemoryNode;
					var mem_add = mem.Address as AddOperatorNode;
					var mem_add_1 = mem_add.LeftOperand as RegisterNode;
					var mem_add_2 = mem_add.RightOperand as RegisterNode;

					return new [] {
						// lea dst, [reg1 + reg2]
						new Instruction (
							map => string.Format("lea {0}, [{1} + {2}]", map[dst], map[mem_add_1], map[mem_add_2]),
							use(dst, mem_add_1, mem_add_2), define(dst)),
							
						copyToTemporary( regNode, dst )
					};
				}
			);
		}

		#endregion
	}
}

