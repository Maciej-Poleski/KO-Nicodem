﻿using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System.Collections.Generic;
using System;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests_Comparison
	{
		static void testCompareRegReg( 
			Func<RegisterNode,RegisterNode,BinaryOperatorNode> condMaker, Tile tile, string cond_type )
		{
			var l = new TemporaryNode ();
			var r = new TemporaryNode ();
			var jmp = condMaker (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);
			map.Add (r, TileFactoryTestUtils.RBX);

			var instructions = tile.Cover (jmp);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"cmp " + TileFactoryTestUtils.RAX + ", " + TileFactoryTestUtils.RBX + "\n" +
				"set" + cond_type + " " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestComparison_RegReg() {
			testCompareRegReg ((l, r) => new EqOperatorNode (l, r), TileFactory.Compare.RegReg_Eq (), "e");
			testCompareRegReg ((l, r) => new NeqOperatorNode (l, r), TileFactory.Compare.RegReg_Neq (), "ne");
			testCompareRegReg ((l, r) => new LtOperatorNode (l, r), TileFactory.Compare.RegReg_Lt (), "l");
			testCompareRegReg ((l, r) => new LteOperatorNode (l, r), TileFactory.Compare.RegReg_Le (), "le");
			testCompareRegReg ((l, r) => new GtOperatorNode (l, r), TileFactory.Compare.RegReg_Gt (), "g");
			testCompareRegReg ((l, r) => new GteOperatorNode (l, r), TileFactory.Compare.RegReg_Ge (), "ge");
		}

		static void testCompareRegConst( 
			Func<RegisterNode,ConstantNode<long>,BinaryOperatorNode> condMaker, Tile tile, string cond_type )
		{
			var l = new TemporaryNode ();
			var r = new ConstantNode<long> (15L);
			var jmp = condMaker (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (l, TileFactoryTestUtils.RAX);

			var instructions = tile.Cover (jmp);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"cmp " + TileFactoryTestUtils.RAX + ", 15\n" +
				"set" + cond_type + " " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestComparison_RegConst() {
			testCompareRegConst ((l, r) => new EqOperatorNode (l, r), TileFactory.Compare.RegConst_Eq<long> (), "e");
			testCompareRegConst ((l, r) => new NeqOperatorNode (l, r), TileFactory.Compare.RegConst_Neq<long> (), "ne");
			testCompareRegConst ((l, r) => new LtOperatorNode (l, r), TileFactory.Compare.RegConst_Lt<long> (), "l");
			testCompareRegConst ((l, r) => new LteOperatorNode (l, r), TileFactory.Compare.RegConst_Le<long> (), "le");
			testCompareRegConst ((l, r) => new GtOperatorNode (l, r), TileFactory.Compare.RegConst_Gt<long> (), "g");
			testCompareRegConst ((l, r) => new GteOperatorNode (l, r), TileFactory.Compare.RegConst_Ge<long> (), "ge");
		}

		static void testCompareConstReg( 
			Func<ConstantNode<long>,RegisterNode,BinaryOperatorNode> condMaker, Tile tile, string cond_type )
		{
			var l = new ConstantNode<long> (15L);
			var r = new TemporaryNode ();
			var jmp = condMaker (l, r);

			var map = TileFactoryTestUtils.createMapping ();
			map.Add (r, TileFactoryTestUtils.RAX);

			var instructions = tile.Cover (jmp);
			TileFactoryTestUtils.updateMapping (instructions, map);

			var got = TileFactoryTestUtils.getASM (instructions, map);
			var expected = 
				"cmp 15, " + TileFactoryTestUtils.RAX + "\n" +
				"set" + cond_type + " " + TileFactoryTestUtils.SPECIAL + "\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void TestComparison_ConstReg() {
			testCompareConstReg ((l, r) => new EqOperatorNode (l, r), TileFactory.Compare.ConstReg_Eq<long> (), "e");
			testCompareConstReg ((l, r) => new NeqOperatorNode (l, r), TileFactory.Compare.ConstReg_Neq<long> (), "ne");
			testCompareConstReg ((l, r) => new LtOperatorNode (l, r), TileFactory.Compare.ConstReg_Lt<long> (), "l");
			testCompareConstReg ((l, r) => new LteOperatorNode (l, r), TileFactory.Compare.ConstReg_Le<long> (), "le");
			testCompareConstReg ((l, r) => new GtOperatorNode (l, r), TileFactory.Compare.ConstReg_Gt<long> (), "g");
			testCompareConstReg ((l, r) => new GteOperatorNode (l, r), TileFactory.Compare.ConstReg_Ge<long> (), "ge");
		}
	}
}

