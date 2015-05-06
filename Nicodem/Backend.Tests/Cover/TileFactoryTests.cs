using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests
	{/*
		static readonly HardwareRegisterNode RSpecial = new HardwareRegisterNode("xxx");

		static readonly HardwareRegisterNode RAX = new HardwareRegisterNode("rax");
		static readonly HardwareRegisterNode RBX = new HardwareRegisterNode("rbx");
		static readonly HardwareRegisterNode RCX = new HardwareRegisterNode("rcx");

		static void updateMapping( IEnumerable<Instruction> instructions,
			IDictionary<RegisterNode, HardwareRegisterNode> mapping)
		{
			foreach (var ins in instructions)
				foreach (var reg in ins.RegistersUsed)
					if (!mapping.Keys.Contains(reg))
						mapping.Add (reg, RSpecial);
		}

		static string cover( IEnumerable<Instruction> instructions, 
			IReadOnlyDictionary<RegisterNode, HardwareRegisterNode> mapping)
		{
			var coverBuilder = new StringBuilder ();
			foreach (var instruction in instructions)
				coverBuilder.Append (instruction.ToString (mapping));
			return coverBuilder.ToString ();
		}

		static void testBinop_RegReg<T>( Func<TemporaryNode, TemporaryNode, T> fun, Tile tile, string mnemonik )
			where T : BinaryOperatorNode
		{
			var left = new TemporaryNode ();
			var right = new TemporaryNode ();
			var node = fun (left, right);

			var instructions = tile.Cover (node).ToArray ();

			Assert.AreEqual (2, instructions.Length);

			Assert.IsFalse (instructions [0].IsCopyInstruction);
			Assert.AreEqual (new[] { left, right }, instructions [0].RegistersUsed);
			Assert.AreEqual (new[] { left }, instructions [0].RegistersDefined);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			dict.Add (right, RBX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			var expected = 
				mnemonik + " " + RAX + ", " + RBX + "\n" +
				"mov " + RSpecial + ", " + RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Binops_RegReg() {
			testBinop_RegReg ((l, r) => new AddOperatorNode (l, r), TileFactory.Add.RegReg (), TileFactory.Add.Mnemonik);
			testBinop_RegReg ((l, r) => new SubOperatorNode (l, r), TileFactory.Sub.RegReg (), TileFactory.Sub.Mnemonik);
			testBinop_RegReg ((l, r) => new BitXorOperatorNode (l, r), TileFactory.BitXor.RegReg (), TileFactory.BitXor.Mnemonik);
			testBinop_RegReg ((l, r) => new LogAndOperatorNode (l, r), TileFactory.LogAnd.RegReg (), TileFactory.LogAnd.Mnemonik);
			testBinop_RegReg ((l, r) => new LogOrOperatorNode (l, r), TileFactory.LogOr.RegReg (), TileFactory.LogOr.Mnemonik);
		}

		static void testBinop_RegConst<T>( Func<TemporaryNode, ConstantNode<long>, T> fun, Tile tile, string mnemonik )
			where T : BinaryOperatorNode
		{
			var left = new TemporaryNode ();
			var right = new ConstantNode<long> (15L);
			var node = fun (left, right);

			var instructions = tile.Cover (node).ToArray ();

			Assert.AreEqual (2, instructions.Length);

			Assert.IsFalse (instructions [0].IsCopyInstruction);
			Assert.AreEqual (new[] { left }, instructions [0].RegistersUsed);
			Assert.AreEqual (new[] { left }, instructions [0].RegistersDefined);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			var expected = 
				mnemonik + " " + RAX + ", 15\n" +
				"mov " + RSpecial + ", " + RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Binops_RegConst() {
			testBinop_RegConst ((l, r) => new AddOperatorNode (l, r), TileFactory.Add.RegConst (), TileFactory.Add.Mnemonik);
			testBinop_RegConst ((l, r) => new SubOperatorNode (l, r), TileFactory.Sub.RegConst (), TileFactory.Sub.Mnemonik);
			testBinop_RegConst ((l, r) => new BitXorOperatorNode (l, r), TileFactory.BitXor.RegConst (), TileFactory.BitXor.Mnemonik);
			testBinop_RegConst ((l, r) => new LogAndOperatorNode (l, r), TileFactory.LogAnd.RegConst (), TileFactory.LogAnd.Mnemonik);
			testBinop_RegConst ((l, r) => new LogOrOperatorNode (l, r), TileFactory.LogOr.RegConst (), TileFactory.LogOr.Mnemonik);
		}

		static void testCmp_RegReg<T>( Func<TemporaryNode, TemporaryNode, T> fun, Tile tile, string mnemonik )
			where T : BinaryOperatorNode
		{
			var left = new TemporaryNode ();
			var right = new TemporaryNode ();
			var node = fun (left, right);

			var instructions = tile.Cover (node).ToArray ();

			Assert.AreEqual (3, instructions.Length);

			Assert.IsFalse (instructions [0].IsCopyInstruction);
			Assert.AreEqual (1, instructions [0].RegistersUsed.Count ());
			Assert.AreEqual (1, instructions [0].RegistersDefined.Count ());

			Assert.IsFalse (instructions [1].IsCopyInstruction);
			Assert.AreEqual (new[]{ left, right }, instructions [1].RegistersUsed);
			Assert.AreEqual (new Node[]{ }, instructions [1].RegistersDefined);

			Assert.IsFalse (instructions [2].IsCopyInstruction);
			Assert.AreEqual (1, instructions [2].RegistersUsed.Count ());
			Assert.AreEqual (1, instructions [2].RegistersDefined.Count ());

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			dict.Add (right, RBX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			var expected = 
				"xor " + RSpecial + ", " + RSpecial + "\n" +
				"cmp " + RAX + ", " + RBX + "\n" +
				mnemonik + " " + RSpecial + ", 1\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Cmp_RegReg() {
			testCmp_RegReg ((l, r) => new LtOperatorNode (l, r), TileFactory.Lt.RegReg (), TileFactory.Lt.Mnemonik);
			testCmp_RegReg ((l, r) => new LteOperatorNode (l, r), TileFactory.Le.RegReg (), TileFactory.Le.Mnemonik);
			testCmp_RegReg ((l, r) => new GtOperatorNode (l, r), TileFactory.Gt.RegReg (), TileFactory.Gt.Mnemonik);
			testCmp_RegReg ((l, r) => new GteOperatorNode (l, r), TileFactory.Ge.RegReg (), TileFactory.Ge.Mnemonik);
			testCmp_RegReg ((l, r) => new EqOperatorNode (l, r), TileFactory.Eq.RegReg (), TileFactory.Eq.Mnemonik);
			testCmp_RegReg ((l, r) => new NeqOperatorNode (l, r), TileFactory.Neq.RegReg (), TileFactory.Neq.Mnemonik);
		}

		static void testCmp_RegConst<T>( Func<TemporaryNode, ConstantNode<long>, T> fun, Tile tile, string mnemonik )
			where T : BinaryOperatorNode
		{
			var left = new TemporaryNode ();
			var right = new ConstantNode<long> (15L);
			var node = fun (left, right);

			var instructions = tile.Cover (node).ToArray ();

			Assert.AreEqual (3, instructions.Length);

			Assert.IsFalse (instructions [0].IsCopyInstruction);
			Assert.AreEqual (1, instructions [0].RegistersUsed.Count ());
			Assert.AreEqual (1, instructions [0].RegistersDefined.Count ());

			Assert.IsFalse (instructions [1].IsCopyInstruction);
			Assert.AreEqual (new[]{ left }, instructions [1].RegistersUsed);
			Assert.AreEqual (new Node[]{ }, instructions [1].RegistersDefined);

			Assert.IsFalse (instructions [2].IsCopyInstruction);
			Assert.AreEqual (1, instructions [2].RegistersUsed.Count ());
			Assert.AreEqual (1, instructions [2].RegistersDefined.Count ());

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			var expected = 
				"xor " + RSpecial + ", " + RSpecial + "\n" +
				"cmp " + RAX + ", 15\n" +
				mnemonik + " " + RSpecial + ", 1\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_Cmp_RegConst() {
			testCmp_RegConst ((l, r) => new LtOperatorNode (l, r), TileFactory.Lt.RegConst (), TileFactory.Lt.Mnemonik);
			testCmp_RegConst ((l, r) => new LteOperatorNode (l, r), TileFactory.Le.RegConst (), TileFactory.Le.Mnemonik);
			testCmp_RegConst ((l, r) => new GtOperatorNode (l, r), TileFactory.Gt.RegConst (), TileFactory.Gt.Mnemonik);
			testCmp_RegConst ((l, r) => new GteOperatorNode (l, r), TileFactory.Ge.RegConst (), TileFactory.Ge.Mnemonik);
			testCmp_RegConst ((l, r) => new EqOperatorNode (l, r), TileFactory.Eq.RegConst (), TileFactory.Eq.Mnemonik);
			testCmp_RegConst ((l, r) => new NeqOperatorNode (l, r), TileFactory.Neq.RegConst (), TileFactory.Neq.Mnemonik);
		}

		[Test]
		public void Test_MOV_Reg_Reg() {
			var left = new TemporaryNode ();
			var right = new TemporaryNode ();
			var node = new AssignmentNode (left, right);

			var tile = TileFactory.Mov.RegReg ();
			var instructions = tile.Cover (node);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			dict.Add (right, RBX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			var expected = 
				"mov " + RAX + ", " + RBX + "\n" +
				"mov " + RSpecial + ", " + RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_SUB_Reg_Reg() {
			var left = new TemporaryNode ();
			var right = new TemporaryNode ();
			var node = new SubOperatorNode (left, right);

			var tile = TileFactory.Sub.RegReg ();
			var instructions = tile.Cover (node);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			dict.Add (right, RBX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			var expected = 
				"sub " + RAX + ", " + RBX + "\n" +
				"mov " + RSpecial + ", " + RAX + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_LEA_Reg_Reg_Reg() {
			var dst = new TemporaryNode ();
			var src1 = new TemporaryNode ();
			var src2 = new TemporaryNode ();
			var plus = new AddOperatorNode (src1, src2);
			var node = new AssignmentNode (dst, new MemoryNode (plus));

			var tile = TileFactory.Lea.Reg_RegReg ();
			var instructions = tile.Cover (node);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (dst, RAX);
			dict.Add (src1, RBX);
			dict.Add (src2, RCX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			var expected = 
				"lea " + RAX + ", [" + RBX + " + " + RCX + "]\n" +
				"mov " + RSpecial + ", " + RAX + "\n";

			Assert.AreEqual (expected, got);
		}*/
	}
}

