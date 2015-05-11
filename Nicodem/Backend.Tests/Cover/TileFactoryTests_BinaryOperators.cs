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
			testBinopRegReg ((l, r) => new BitAndOperatorNode (l, r), TileFactory.BitAnd.RegReg (), "and");
			testBinopRegReg ((l, r) => new BitOrOperatorNode (l, r), TileFactory.BitOr.RegReg (), "or");
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
			testBinopRegConst ((l, r) => new BitAndOperatorNode (l, r), TileFactory.BitAnd.RegConst<long> (), "and");
			testBinopRegConst ((l, r) => new BitOrOperatorNode (l, r), TileFactory.BitOr.RegConst<long> (), "or");
			testBinopRegConst ((l, r) => new LogOrOperatorNode (l, r), TileFactory.LogOr.RegConst<long> (), "or");
		}

		static void testBinopConstReg( 
			Func<ConstantNode<long>,RegisterNode,BinaryOperatorNode> condMaker, Tile tile, string mnemonik )
		{
			var l = new ConstantNode<long> (4L);
			var r = new TemporaryNode ();
			var binop = condMaker (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (r, TileFactoryTestUtils.RAX);

			var instructions = tile.Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", 4\n" +
				mnemonik + " " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestBinop_ConstReg() {
			testBinopConstReg ((l, r) => new AddOperatorNode (l, r), TileFactory.Add.ConstReg<long> (), "add");
			testBinopConstReg ((l, r) => new SubOperatorNode (l, r), TileFactory.Sub.ConstReg<long> (), "sub");
			//testBinopConstReg ((l, r) => new ShlOperatorNode (l, r), TileFactory.Shl.ConstReg<long> (), "shl");
			//testBinopConstReg ((l, r) => new ShrOperatorNode (l, r), TileFactory.Shr.ConstReg<long> (), "shr");
			testBinopConstReg ((l, r) => new BitXorOperatorNode (l, r), TileFactory.BitXor.ConstReg<long> (), "xor");
			testBinopConstReg ((l, r) => new BitAndOperatorNode (l, r), TileFactory.BitAnd.ConstReg<long> (), "and");
			testBinopConstReg ((l, r) => new BitOrOperatorNode (l, r), TileFactory.BitOr.ConstReg<long> (), "or");
			testBinopConstReg ((l, r) => new LogOrOperatorNode (l, r), TileFactory.LogOr.ConstReg<long> (), "or");
		}

		[Test]
		public void Test_Mul_RegReg() {
			var l = new TemporaryNode ();
			var r = new TemporaryNode ();
			var binop = new MulOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);
			map.Add (r, TileFactoryTestUtils.R10);

			var instructions = TileFactory.Mul.RegReg().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + Target.RAX + ", " + TileFactoryTestUtils.R9 + "\n" +
				"mul " + TileFactoryTestUtils.R10 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Mul_RegConst() {
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (13L);
			var binop = new MulOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);

			var instructions = TileFactory.Mul.RegConst<long>().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + Target.RAX + ", 13\n" +
				"mul " + TileFactoryTestUtils.R9 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Mul_ConstReg() {
			var l = new ConstantNode<long> (13L);
			var r = new TemporaryNode ();
			var binop = new MulOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (r, TileFactoryTestUtils.R10);

			var instructions = TileFactory.Mul.ConstReg<long>().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + Target.RAX + ", 13\n" +
				"mul " + TileFactoryTestUtils.R10 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Div_RegReg() {
			var l = new TemporaryNode ();
			var r = new TemporaryNode ();
			var binop = new DivOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);
			map.Add (r, TileFactoryTestUtils.R10);

			var instructions = TileFactory.Div.RegReg().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + Target.RDX + ", " + Target.RDX + "\n" +
				"mov " + Target.RAX + ", " + TileFactoryTestUtils.R9 + "\n" +
				"div " + TileFactoryTestUtils.R10 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Div_RegConst() {
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (13L);
			var binop = new DivOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);

			var instructions = TileFactory.Div.RegConst<long> ().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + Target.RDX + ", " + Target.RDX + "\n" +
				"mov " + Target.RAX + ", " + TileFactoryTestUtils.R9 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", 13\n" +
				"div " + TileFactoryTestUtils.SPECIAL + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Div_ConstReg() {
			var l = new ConstantNode<long> (13L);
			var r = new TemporaryNode ();
			var binop = new DivOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (r, TileFactoryTestUtils.R10);

			var instructions = TileFactory.Div.ConstReg<long> ().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + Target.RDX + ", " + Target.RDX + "\n" +
				"mov " + Target.RAX + ", 13\n" +
				"div " + TileFactoryTestUtils.R10 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Mod_RegReg() {
			var l = new TemporaryNode ();
			var r = new TemporaryNode ();
			var binop = new ModOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);
			map.Add (r, TileFactoryTestUtils.R10);

			var instructions = TileFactory.Mod.RegReg().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + Target.RDX + ", " + Target.RDX + "\n" +
				"mov " + Target.RAX + ", " + TileFactoryTestUtils.R9 + "\n" +
				"div " + TileFactoryTestUtils.R10 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RDX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Mod_RegConst() {
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (13L);
			var binop = new ModOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);

			var instructions = TileFactory.Mod.RegConst<long> ().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + Target.RDX + ", " + Target.RDX + "\n" +
				"mov " + Target.RAX + ", " + TileFactoryTestUtils.R9 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", 13\n" +
				"div " + TileFactoryTestUtils.SPECIAL + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RDX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Mod_ConstReg() {
			var l = new ConstantNode<long> (13L);
			var r = new TemporaryNode ();
			var binop = new ModOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (r, TileFactoryTestUtils.R10);

			var instructions = TileFactory.Mod.ConstReg<long> ().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + Target.RDX + ", " + Target.RDX + "\n" +
				"mov " + Target.RAX + ", 13\n" +
				"div " + TileFactoryTestUtils.R10 + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + Target.RDX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_LogAnd_RegReg() {
			var l = new TemporaryNode ();
			var r = new TemporaryNode ();
			var binop = new LogAndOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);
			map.Add (r, TileFactoryTestUtils.R10);

			var instructions = TileFactory.LogAnd.RegReg().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.R9 + "\n" +
				"xor " + TileFactoryTestUtils.R9 + ", " + TileFactoryTestUtils.R9 + "\n" +
				"cmp " + TileFactoryTestUtils.R10 + ", " + TileFactoryTestUtils.R9 + "\n" +
				"cmove " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.R9 + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_LogAnd_RegConst_True() {
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (1L);
			var binop = new LogAndOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);

			var instructions = TileFactory.LogAnd.RegConst().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.R9 + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_LogAnd_RegConst_False() {
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (0L);
			var binop = new LogAndOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.R9);

			var instructions = TileFactory.LogAnd.RegConst().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_LogAnd_ConstReg_True() {
			var l = new ConstantNode<long> (1L);
			var r = new TemporaryNode ();
			var binop = new LogAndOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (r, TileFactoryTestUtils.R9);

			var instructions = TileFactory.LogAnd.ConstReg().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.R9 + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_LogAnd_ConstReg_False() {
			var l = new ConstantNode<long> (0L);
			var r = new TemporaryNode ();
			var binop = new LogAndOperatorNode (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (r, TileFactoryTestUtils.R9);

			var instructions = TileFactory.LogAnd.ConstReg().Cover (binop);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"xor " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}
	}
}

