using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		public static class Assign
		{
			#region Destination - Reg

			// reg1 = reg2 + const
			public static Tile Reg_AddConst() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<RegisterNode> (),
						makeTile<AddOperatorNode> (
							makeTile<RegisterNode> (),
							makeTile<ConstantNode<long>> ()
						)
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var reg1 = root.Target as RegisterNode;
						var addop = root.Source as AddOperatorNode;
						var reg2 = addop.LeftOperand as RegisterNode;
						var val = addop.RightOperand as ConstantNode<long>;

						// reg = reg + 1 -> reg++
						if (reg1 == reg2 && val.Value == 1L) {
							return new[] {
								InstructionFactory.Inc (reg1),
								InstructionFactory.Move (regNode, reg1)
							};
						} else {
							return new[] {
								InstructionFactory.Move (reg1, reg2),  // r = left
								InstructionFactory.Add (reg1, val),    // r = left + right
								InstructionFactory.Move (regNode, reg1)
							};
						}
					}
				);
			}

			// reg1 = reg2 - const
			public static Tile Reg_SubConst() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<RegisterNode> (),
						makeTile<SubOperatorNode> (
							makeTile<RegisterNode> (),
							makeTile<ConstantNode<long>> ()
						)
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var reg1 = root.Target as RegisterNode;
						var subop = root.Source as SubOperatorNode;
						var reg2 = subop.LeftOperand as RegisterNode;
						var val = subop.RightOperand as ConstantNode<long>;

						// reg = reg - 1 -> reg--
						if (reg1 == reg2 && val.Value == 1L) {
							return new[] {
								InstructionFactory.Dec (reg1),
								InstructionFactory.Move (regNode, reg1)
							};
						} else {
							return new[] {
								InstructionFactory.Move (reg1, reg2),  // r = left
								InstructionFactory.Sub (reg1, val),    // r = left - right
								InstructionFactory.Move (regNode, reg1)
							};
						}
					}
				);
			}

			public static Tile Reg_Reg() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<RegisterNode> (),
						makeTile<RegisterNode> ()
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var target = root.Target as RegisterNode;
						var source = root.Source as RegisterNode;

						return new [] {
							InstructionFactory.Move (target, source),
							InstructionFactory.Move (regNode, target)
						};
					}
				);
			}

			public static Tile Reg_Const() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<RegisterNode> (),
						makeTile<ConstantNode<long>> ()
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var target = root.Target as RegisterNode;
						var valNode = root.Source as ConstantNode<long>;

						if (valNode.Value == 0L) {
							return new [] {
								InstructionFactory.Xor (target, target),
								InstructionFactory.Move (regNode, target)
							};
						} else {
							return new [] {
								InstructionFactory.Move (target, valNode),
								InstructionFactory.Move (regNode, target)
							};
						}
					}
				);
			}

			public static Tile Reg_MemReg() {
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode> (),
						makeTile<MemoryNode> (
							makeTile<RegisterNode> ()
						)
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var dst = root.Target as RegisterNode;
						var mem = root.Source as MemoryNode;
						var src = mem.Address as RegisterNode;

						return new[] {
							new Instruction (
								map => string.Format ("mov {0}, [{1}]", map [dst], map [src]),
								use (dst, src), define (dst)),
							InstructionFactory.Move (regNode, dst)
						};
					}
				);
			}

			public static Tile Reg_MemConst() {
				return new Tile (typeof(AssignmentNode),
					new[] {
						makeTile<RegisterNode> (),
						makeTile<MemoryNode> (
							makeTile<ConstantNode<long>> ()
						)
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var dst = root.Target as RegisterNode;
						var mem = root.Source as MemoryNode;
						var addr = mem.Address as ConstantNode<long>;

						return new[] {
							new Instruction (
								map => string.Format ("mov {0}, [{1}]", map [dst], addr.Value),
								use (dst), define (dst)),
							InstructionFactory.Move (regNode, dst)
						};
					}
				);
			}

			#endregion

			#region Destination - Memory Reg

			public static Tile MemReg_Reg() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<MemoryNode>(
							makeTile<RegisterNode>()
						),
						makeTile<RegisterNode>()
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var mem = root.Target as MemoryNode;
						var dst = mem.Address as RegisterNode;
						var src = root.Source as RegisterNode;

						return new [] {
							new Instruction (
								map => string.Format("mov [{0}], {1}", map[dst], map[src]),
								use(dst, src), define())
						};
					}
				);
			}

			public static Tile MemReg_Const() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<MemoryNode>(
							makeTile<RegisterNode>()
						),
						makeTile<ConstantNode<long>>()
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var mem = root.Target as MemoryNode;
						var dst = mem.Address as RegisterNode;
						var src = root.Source as ConstantNode<long>;

						return new [] {
							new Instruction (
								map => string.Format("mov [{0}], {1}", map[dst], src.Value),
								use(dst), define())
						};
					}
				);
			}

			#endregion

			#region Destination - Memory Const

			public static Tile MemConst_Reg() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<MemoryNode>(
							makeTile<ConstantNode<long>>()
						),
						makeTile<RegisterNode>()
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var mem = root.Target as MemoryNode;
						var dst = mem.Address as ConstantNode<long>;
						var src = root.Source as RegisterNode;

						return new [] {
							new Instruction (
								map => string.Format("mov [{0}], {1}", dst.Value, map[src]),
								use(src), define())
						};
					}
				);
			}

			public static Tile MemConst_Const() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<MemoryNode>(
							makeTile<ConstantNode<long>>()
						),
						makeTile<ConstantNode<long>>()
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var mem = root.Target as MemoryNode;
						var dst = mem.Address as ConstantNode<long>;
						var src = root.Source as ConstantNode<long>;

						return new [] {
							new Instruction (
								map => string.Format("mov [{0}], {1}", dst.Value, src.Value),
								use(), define())
						};
					}
				);
			}

			#endregion
		}
	}
}

