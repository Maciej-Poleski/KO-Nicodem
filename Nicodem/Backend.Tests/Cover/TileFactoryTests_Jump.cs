﻿using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System.Collections.Generic;
using System;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests_Jump
	{
		[Test]
		public void TestUnconditionalJump() {
			var lbl = new LabelNode ("target");
			var jmp = new UnconditionalJumpToLabelNode (lbl);

			var tile = TileFactory.Jump.Unconditional ();
			var map = TileFactoryTestUtils.createMapping ();

			var instructions = tile.Cover (jmp);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = "jmp " + lbl.Label;

			Assert.AreEqual (expected, got);
		}

		static void testCondRegRegJump( 
			Func<RegisterNode,RegisterNode,BinaryOperatorNode> condMaker, Tile tile, string cond_type )
		{
			var l = new TemporaryNode ();
			var r = new TemporaryNode ();
			var lbl = new LabelNode ("target");
			var jmp = new ConditionalJumpToLabelNode (condMaker (l, r), lbl);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);
			map.Add (r, TileFactoryTestUtils.RBX);

			var instructions = tile.Cover (jmp);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = "cmp " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"j" + cond_type + lbl.Label;

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestConditionalJump_RegReg() {
			testCondRegRegJump ((l, r) => new EqOperatorNode (l, r), TileFactory.Jump.Cond_RegReg_Eq (), "e");
			testCondRegRegJump ((l, r) => new NeqOperatorNode (l, r), TileFactory.Jump.Cond_RegReg_Neq (), "ne");
			testCondRegRegJump ((l, r) => new LtOperatorNode (l, r), TileFactory.Jump.Cond_RegReg_Lt (), "l");
			testCondRegRegJump ((l, r) => new LteOperatorNode (l, r), TileFactory.Jump.Cond_RegReg_Le (), "le");
			testCondRegRegJump ((l, r) => new GtOperatorNode (l, r), TileFactory.Jump.Cond_RegReg_Gt (), "g");
			testCondRegRegJump ((l, r) => new GteOperatorNode (l, r), TileFactory.Jump.Cond_RegReg_Ge (), "ge");
		}

		static void testCondRegConstJump( 
			Func<RegisterNode,ConstantNode<long>,BinaryOperatorNode> condMaker, Tile tile, string cond_type )
		{
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (15L);
			var lbl = new LabelNode ("target");
			var jmp = new ConditionalJumpToLabelNode (condMaker (l, r), lbl);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = tile.Cover (jmp);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = "cmp " + TileFactoryTestUtils.RAX + ", 10\n" +
			               "j" + cond_type + lbl.Label;

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestConditionalJump_RegConst() {
			testCondRegConstJump ((l, r) => new EqOperatorNode (l, r), TileFactory.Jump.Cond_RegConst_Eq<long> (), "e");
			testCondRegConstJump ((l, r) => new NeqOperatorNode (l, r), TileFactory.Jump.Cond_RegConst_Neq<long> (), "ne");
			testCondRegConstJump ((l, r) => new LtOperatorNode (l, r), TileFactory.Jump.Cond_RegConst_Lt<long> (), "l");
			testCondRegConstJump ((l, r) => new LteOperatorNode (l, r), TileFactory.Jump.Cond_RegConst_Le<long> (), "le");
			testCondRegConstJump ((l, r) => new GtOperatorNode (l, r), TileFactory.Jump.Cond_RegConst_Gt<long> (), "g");
			testCondRegConstJump ((l, r) => new GteOperatorNode (l, r), TileFactory.Jump.Cond_RegConst_Ge<long> (), "ge");
		}
	}
}

