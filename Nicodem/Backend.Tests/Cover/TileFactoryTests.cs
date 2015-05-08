using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests
	{
		[Test]
		public void Test_Const() {
			var node = new ConstantNode<long> (123L);

			var map = TileFactoryTestUtils.createMapping ();
			var instructions = TileFactory.ConstTile<long>().Cover (node);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", 123\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_MemAccess() {
			var reg = new TemporaryNode ();
			var mem = new MemoryNode (reg);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (reg, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.MemAccessTile().Cover (mem);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", [" + TileFactoryTestUtils.RAX + "]\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_MemAccessConst() {
			var val = new ConstantNode<long> (123L);
			var mem = new MemoryNode (val);

			var map = TileFactoryTestUtils.createMapping ();

			var instructions = TileFactory.MemAccessTile<long>().Cover (mem);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", [123]\n";

			Assert.AreEqual (expected, got);
		}
	}
}

