using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests_BinaryOperators
	{
		static void testBinopRegReg( 
			Func<RegisterNode,RegisterNode,BinaryOperatorNode> condMaker, Tile tile, string mnemonik )
		{
			var l = new TemporaryNode ();
			var r = new TemporaryNode ();
			var binop = condMaker (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);
			map.Add (r, TileFactoryTestUtils.RBX);

			var instructions = tile.Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n" +
				mnemonik + " " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RBX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestBinop_RegReg() {
			testBinopRegReg ((l, r) => new AddOperatorNode (l, r), TileFactory.Add.RegReg (), "add");
			testBinopRegReg ((l, r) => new SubOperatorNode (l, r), TileFactory.Sub.RegReg (), "sub");
			testBinopRegReg ((l, r) => new BitXorOperatorNode (l, r), TileFactory.BitXor.RegReg (), "xor");
			testBinopRegReg ((l, r) => new LogAndOperatorNode (l, r), TileFactory.LogAnd.RegReg (), "and");
			testBinopRegReg ((l, r) => new LogOrOperatorNode (l, r), TileFactory.LogOr.RegReg (), "or");
		}

		static void testBinopRegConst( 
			Func<RegisterNode,ConstantNode<long>,BinaryOperatorNode> condMaker, Tile tile, string mnemonik )
		{
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (4L);
			var binop = condMaker (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = tile.Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n" +
				mnemonik + " " + TileFactoryTestUtils.SPECIAL + ", 4\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestBinop_RegConst() {
			testBinopRegConst ((l, r) => new AddOperatorNode (l, r), TileFactory.Add.RegConst<long> (), "add");
			testBinopRegConst ((l, r) => new SubOperatorNode (l, r), TileFactory.Sub.RegConst<long> (), "sub");
			testBinopRegConst ((l, r) => new ShlOperatorNode (l, r), TileFactory.Shl.RegConst<long> (), "shl");
			testBinopRegConst ((l, r) => new ShrOperatorNode (l, r), TileFactory.Shr.RegConst<long> (), "shr");
			testBinopRegConst ((l, r) => new BitXorOperatorNode (l, r), TileFactory.BitXor.RegConst<long> (), "xor");
			testBinopRegConst ((l, r) => new LogAndOperatorNode (l, r), TileFactory.LogAnd.RegConst<long> (), "and");
			testBinopRegConst ((l, r) => new LogOrOperatorNode (l, r), TileFactory.LogOr.RegConst<long> (), "or");
		}
	}
}

