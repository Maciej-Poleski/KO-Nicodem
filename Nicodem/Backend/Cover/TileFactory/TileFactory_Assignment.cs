using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public static partial class TileFactory
	{
		public static class Mov
		{
			// mov reg64 reg64
			public static Tile RegReg() {
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
							new Instruction (
								map => string.Format("mov {0}, {1}", map[target], map[source]),
								use(target, source), define(target), true),
							copyToTemporary( regNode, target )
						};
					}
				);
			}

			// mov reg64 imm64
			// xor reg64 reg64 (for val = 0)
			public static Tile RegConst() {
				return new Tile (typeof(AssignmentNode),
					new[] { 
						makeTile<RegisterNode>(),
						makeTile<ConstantNode<long>>()
					},
					(regNode, node) => {
						var root = node as AssignmentNode;
						var target = root.Target as RegisterNode;
						var valNode = root.Source as ConstantNode<long>;

						if (valNode.Value == 0L) {
							return new [] {
								new Instruction (
									map => string.Format("xor {0}, {0}", map[target]),
									use(target), define(target)),
								copyToTemporary( regNode, target )
							};
						} else {
							return new [] {
								new Instruction (
									map => string.Format("mov {0}, {1}", map[target], valNode.Value),
									use(target), define(target)),
								copyToTemporary( regNode, target )
							};
						}
					}
				);
			}
		}
	}
}

