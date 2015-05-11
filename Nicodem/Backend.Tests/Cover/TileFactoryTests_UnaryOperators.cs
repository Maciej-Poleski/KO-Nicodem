using NUnit.Framework;
using Nicodem.Backend.Representation;
using System;
using Nicodem.Backend.Cover;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests_UnaryOperators
	{
		[Test]
		public void Test_Inc() {
			var l = new TemporaryNode ();
			var unary = new IncOperatorNode (l);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Unop.Inc_Reg().Cover (unary);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"inc " + TileFactoryTestUtils.RAX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Dec() {
			var l = new TemporaryNode ();
			var unary = new DecOperatorNode (l);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Unop.Dec_Reg().Cover (unary);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"dec " + TileFactoryTestUtils.RAX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Minus() {
			var l = new TemporaryNode ();
			var unary = new UnaryMinusOperatorNode (l);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Unop.Minus_Reg().Cover (unary);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.SPECIAL + "\n" +
				"sub " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Plus() {
			var l = new TemporaryNode ();
			var unary = new UnaryPlusOperatorNode (l);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Unop.Plus_Reg().Cover (unary);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Neg() {
			var l = new TemporaryNode ();
			var unary = new NegOperatorNode (l);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Unop.Neg_Reg().Cover (unary);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n" +
				"neg " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_BinNot() {
			var l = new TemporaryNode ();
			var unary = new BinNotOperatorNode (l);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Unop.BinNot_Reg().Cover (unary);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n" +
				"not " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_LogNot() {
			var l = new TemporaryNode ();
			var unary = new LogNotOperatorNode (l);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Unop.LogNot_Reg().Cover (unary);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"cmp " + TileFactoryTestUtils.RAX + ", 0\n" +
				"sete " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}
	}
}

