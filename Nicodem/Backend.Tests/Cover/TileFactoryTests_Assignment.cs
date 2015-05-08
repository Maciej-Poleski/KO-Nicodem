using NUnit.Framework;
using Nicodem.Backend.Representation;
using System;
using Nicodem.Backend.Cover;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests_Assignment
	{
		static Func<RegisterNode,RegisterNode,RegisterNode> same() {
			return (a, b) => a;
		}

		static Func<RegisterNode, RegisterNode,RegisterNode> diff() {
			return (a,b) => b;
		}

		static void generalTest(
			Func<RegisterNode,RegisterNode,RegisterNode> target,
			Func<RegisterNode,ConstantNode<long>,BinaryOperatorNode> condMaker,
			Tile tile, long value, string expected ) 
		{
			var dst = new TemporaryNode ();
			var src = new TemporaryNode ();
			var cons = new ConstantNode<long> (value);
			var oper = condMaker (src, cons);
			var assign = new AssignmentNode (target(src, dst), oper);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (dst, TileFactoryTestUtils.RAX);
			map.Add (src, TileFactoryTestUtils.RBX);

			var instructions = tile.Cover (assign);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);

			Assert.AreEqual (expected, got);
		}

		static void sameTest(
			Func<RegisterNode,ConstantNode<long>,BinaryOperatorNode> condMaker,
			Tile tile, long value, string expected )
		{
			generalTest (same(), condMaker, tile, value, expected);
		}

		static void diffTest(
			Func<RegisterNode,ConstantNode<long>,BinaryOperatorNode> condMaker,
			Tile tile, long value, string expected )
		{
			generalTest (diff(), condMaker, tile, value, expected);
		}

		[Test]
		public void Test_Inc() {
			var expected = 
				"inc " + TileFactoryTestUtils.RBX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RBX + "\n";
			sameTest ((reg, con) => new AddOperatorNode (reg, con), TileFactory.Assign.Reg_AddConst (), 1L, expected);
		}

		[Test]
		public void Test_Dec() {
			var expected = 
				"dec " + TileFactoryTestUtils.RBX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RBX + "\n";
			sameTest ((reg, con) => new SubOperatorNode (reg, con), TileFactory.Assign.Reg_SubConst (), 1L, expected);
		}

		[Test]
		public void Test_AddSameReg() {
			var expected = 
				"mov " + TileFactoryTestUtils.RBX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"add " + TileFactoryTestUtils.RBX + ", 2\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RBX + "\n";
			sameTest ((reg, con) => new AddOperatorNode (reg, con), TileFactory.Assign.Reg_AddConst (), 2L, expected);
		}

		[Test]
		public void Test_SubSameReg() {
			var expected = 
				"mov " + TileFactoryTestUtils.RBX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"sub " + TileFactoryTestUtils.RBX + ", 2\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RBX + "\n";
			sameTest ((reg, con) => new SubOperatorNode (reg, con), TileFactory.Assign.Reg_SubConst (), 2L, expected);
		}

		[Test]
		public void Test_AddDiffReg() {
			var expected = 
				"mov " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"add " + TileFactoryTestUtils.RAX + ", 2\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";
			diffTest ((reg, con) => new AddOperatorNode (reg, con), TileFactory.Assign.Reg_AddConst (), 2L, expected);
		}

		[Test]
		public void Test_SubDiffReg() {
			var expected = 
				"mov " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"sub " + TileFactoryTestUtils.RAX + ", 2\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";
			diffTest ((reg, con) => new SubOperatorNode (reg, con), TileFactory.Assign.Reg_SubConst (), 2L, expected);
		}

		[Test]
		public void Test_Add1DiffReg() {
			var expected = 
				"mov " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"add " + TileFactoryTestUtils.RAX + ", 1\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";
			diffTest ((reg, con) => new AddOperatorNode (reg, con), TileFactory.Assign.Reg_AddConst (), 1L, expected);
		}

		[Test]
		public void Test_Sub1DiffReg() {
			var expected = 
				"mov " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"sub " + TileFactoryTestUtils.RAX + ", 1\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";
			diffTest ((reg, con) => new SubOperatorNode (reg, con), TileFactory.Assign.Reg_SubConst (), 1L, expected);
		}

		[Test]
		public void Test_RegReg() {
			var dst = new TemporaryNode ();
			var src = new TemporaryNode ();
			var assign = new AssignmentNode (dst, src);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (dst, TileFactoryTestUtils.RAX);
			map.Add (src, TileFactoryTestUtils.RBX);

			var instructions = TileFactory.Assign.Reg_Reg().Cover (assign);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		static void testRegConst( long value, string expected ) {
			var dst = new TemporaryNode ();
			var src = new ConstantNode<long> (value);
			var assign = new AssignmentNode (dst, src);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (dst, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Assign.Reg_Const().Cover (assign);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_RegConstNonZero() {
			var expected = 
				"mov " + TileFactoryTestUtils.RAX + ", 2\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";
			testRegConst (2L, expected);
		}

		[Test]
		public void Test_RegConstZero() {
			var expected = 
				"xor " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RAX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RAX + "\n";
			testRegConst (0L, expected);
		}

		[Test]
		public void Test_MemReg_Reg() {
			var dst = new TemporaryNode ();
			var src = new TemporaryNode ();
			var assign = new AssignmentNode (new MemoryNode (dst), src);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (dst, TileFactoryTestUtils.RAX);
			map.Add (src, TileFactoryTestUtils.RBX);

			var instructions = TileFactory.Assign.MemReg_Reg().Cover (assign);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov [" + TileFactoryTestUtils.RAX + "], " + TileFactoryTestUtils.RBX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RBX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_MemReg_Const() {
			var dst = new TemporaryNode ();
			var src = new ConstantNode<long> (123L);
			var assign = new AssignmentNode (new MemoryNode (dst), src);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (dst, TileFactoryTestUtils.RAX);

			var instructions = TileFactory.Assign.MemReg_Const().Cover (assign);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov [" + TileFactoryTestUtils.RAX + "], 123\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", 123\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_MemConst_Reg() {
			var dst = new ConstantNode<long> (1111L);
			var src = new TemporaryNode ();
			var assign = new AssignmentNode (new MemoryNode (dst), src);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (src, TileFactoryTestUtils.RBX);

			var instructions = TileFactory.Assign.MemConst_Reg().Cover (assign);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov [1111], " + TileFactoryTestUtils.RBX + "\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", " + TileFactoryTestUtils.RBX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_MemConst_Const() {
			var dst = new ConstantNode<long> (1111L);
			var src = new ConstantNode<long> (123L);
			var assign = new AssignmentNode (new MemoryNode (dst), src);

			var map = TileFactoryTestUtils.createMapping ();

			var instructions = TileFactory.Assign.MemConst_Const().Cover (assign);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"mov [1111], 123\n" +
				"mov " + TileFactoryTestUtils.SPECIAL + ", 123\n";

			Assert.AreEqual (expected, got);
		}
	}
}

