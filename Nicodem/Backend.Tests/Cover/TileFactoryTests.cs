using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System.Linq;
using System.Collections.Generic;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests
	{
		[Test]
		public void Test_Call() {
			var fun = new Function ("myFun", new[]{ false });
			var node = new FunctionCallNode (fun);

			var map = TileFactoryTestUtils.createMapping ();
			var instructions = TileFactory.Stack.Call ().Cover (node);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"call " + node.Function.Label + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Push() {
			var reg = new TemporaryNode ();
			var node = new PushNode (reg);

			var map = TileFactoryTestUtils.createMapping ();
			var instructions = TileFactory.Stack.Push ().Cover (node);
			map.Add (reg, TileFactoryTestUtils.RAX);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"push " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Pop() {
			var reg = new TemporaryNode ();
			var node = new PopNode (reg);

			var map = TileFactoryTestUtils.createMapping ();
			var instructions = TileFactory.Stack.Pop ().Cover (node);
			map.Add (reg, TileFactoryTestUtils.RAX);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"pop " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}


		[Test]
		public void Test_Ret() {
			var node = new RetNode ();

			var map = TileFactoryTestUtils.createMapping ();
			var instructions = TileFactory.Stack.Ret ().Cover (node);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			const string expected = "ret\n";

			Assert.AreEqual (expected, got);
		}

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
		public void Test_Label() {
			var node = new LabelNode ("myLabel");

			var map = TileFactoryTestUtils.createMapping ();
			var instructions = TileFactory.LabelTile ().Cover (node);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var insn = instructions.ToArray () [0];
			Assert.IsFalse (insn.IsCopyInstruction);
			Assert.IsFalse (insn.IsJumpInstruction);
			Assert.IsTrue (insn.IsLabel);
			Assert.AreEqual (node.Label, insn.Label);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = node.Label + ":\n";

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

