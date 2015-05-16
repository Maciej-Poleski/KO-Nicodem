using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System;
using System.Collections.Generic;


namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class CompareNodeWithTileTests
	{

		[Test]
		public void SingleTileTests() {
			var results = new Dictionary<string, Tuple<bool, List<Node>>> ();

			// Add{Reg,Reg}
			var addRegReg = new AddOperatorNode (
				                new HardwareRegisterNode ("a"),
				                new HardwareRegisterNode ("b")
			                );
			results.Add ("addRegReg", addRegReg.Compare (TileFactory.Add.RegReg ()));

			// Add{Reg,Const}
			var addRegConst = new AddOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new ConstantNode<long> (2)
			                  );
			results.Add ("addRegConst", addRegConst.Compare (TileFactory.Add.RegConst<long> ()));

			// Add{Const,Reg}
			var addConstReg = new AddOperatorNode (
				                  new ConstantNode<long> (2),
				                  new HardwareRegisterNode ("a")
			                  );
			results.Add ("addConstReg", addConstReg.Compare (TileFactory.Add.ConstReg<long> ()));

			// Sub{Reg,Reg}
			var subRegReg = new SubOperatorNode (
				                new HardwareRegisterNode ("a"),
				                new HardwareRegisterNode ("b")
			                );
			results.Add ("subRegReg", subRegReg.Compare (TileFactory.Sub.RegReg ()));

			// Sub{Reg,Const}
			var subRegConst = new SubOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new ConstantNode<long> (2)
			                  );
			results.Add ("subRegConst", subRegConst.Compare (TileFactory.Sub.RegConst<long> ()));

			// Sub{Const,Reg}
			var subConstReg = new SubOperatorNode (
				                  new ConstantNode<long> (2),
				                  new HardwareRegisterNode ("a")
			                  );
			results.Add ("subConstReg", subConstReg.Compare (TileFactory.Sub.ConstReg<long> ()));

			// Mul{Reg,Reg}
			var mulRegReg = new MulOperatorNode (
				                new HardwareRegisterNode ("a"),
				                new HardwareRegisterNode ("b")
			                );
			results.Add ("mulRegReg", mulRegReg.Compare (TileFactory.Mul.RegReg ()));

			//Mul{Reg,Const}
			var mulRegConst = new MulOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new ConstantNode<long> (2)
			                  );
			results.Add ("mulRegConst", mulRegConst.Compare (TileFactory.Mul.RegConst<long> ()));

			//Mul{Const,Reg}
			var mulConstReg = new MulOperatorNode (
				                  new ConstantNode<long> (2),
				                  new HardwareRegisterNode ("a")
			                  );
			results.Add ("mulConstReg", mulConstReg.Compare (TileFactory.Mul.ConstReg<long> ()));

			//Div{Reg,Reg}
			var divRegReg = new DivOperatorNode (
				                new HardwareRegisterNode ("a"),
				                new HardwareRegisterNode ("b")
			                );
			results.Add ("divRegReg", divRegReg.Compare (TileFactory.Div.RegReg ()));

			//Div{Reg,Const}
			var divRegConst = new DivOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new ConstantNode<long> (2)
			                  );
			results.Add ("divRegConst", divRegConst.Compare (TileFactory.Div.RegConst<long> ()));

			//Div{Const,Reg}
			var divConstReg = new DivOperatorNode (
				                  new ConstantNode<long> (2),
				                  new HardwareRegisterNode ("a")
			                  );
			results.Add ("divConstReg", divConstReg.Compare (TileFactory.Div.ConstReg<long> ()));

			//Mod{Reg,Reg}
			var modRegReg = new ModOperatorNode (
				                new HardwareRegisterNode ("a"),
				                new HardwareRegisterNode ("b")
			                );
			results.Add ("modRegReg", modRegReg.Compare (TileFactory.Mod.RegReg ()));

			//Mod{Reg,Const}
			var modRegConst = new ModOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new ConstantNode<long> (2)
			                  );
			results.Add ("modRegConst", modRegConst.Compare (TileFactory.Mod.RegConst<long> ()));

			//Mod{Const,Reg}
			var modConstReg = new ModOperatorNode (
				                  new ConstantNode<long> (2),
				                  new HardwareRegisterNode ("a")
			                  );
			results.Add ("modConstReg", modConstReg.Compare (TileFactory.Mod.ConstReg<long> ()));

			//Shl{Reg,Const}
			var shlRegConst = new ShlOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new ConstantNode<long> (2)
			                  );
			results.Add ("shlRegConst", shlRegConst.Compare (TileFactory.Shl.RegConst<long> ()));

			//Shr{Reg,Const}
			var shrRegConst = new ShrOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new ConstantNode<long> (2)
			                  );
			results.Add ("shrRegConst", shrRegConst.Compare (TileFactory.Shr.RegConst<long> ()));

			//BitXor{Reg,Reg}
			var bitXorRegReg = new BitXorOperatorNode (
				                   new HardwareRegisterNode ("a"),
				                   new HardwareRegisterNode ("b")
			                   );
			results.Add ("bitXorRegReg", bitXorRegReg.Compare (TileFactory.BitXor.RegReg ()));

			//BitXor{Reg,Const}
			var bitXorRegConst = new BitXorOperatorNode (
				                     new HardwareRegisterNode ("a"),
				                     new ConstantNode<long> (2)
			                     );
			results.Add ("bitXorRegConst", bitXorRegConst.Compare (TileFactory.BitXor.RegConst<long> ()));

			//BitXor{Const,Reg}
			var bitXorConstReg = new BitXorOperatorNode (
				                     new ConstantNode<long> (2),
				                     new HardwareRegisterNode ("a")
			                     );
			results.Add ("bitXorConstReg", bitXorConstReg.Compare (TileFactory.BitXor.ConstReg<long> ()));

			//BitAnd{Reg,Reg}
			var bitAndRegReg = new BitAndOperatorNode (
				                   new HardwareRegisterNode ("a"),
				                   new HardwareRegisterNode ("b")
			                   );
			results.Add ("bitAndRegReg", bitAndRegReg.Compare (TileFactory.BitAnd.RegReg ()));

			//BitAnd{Reg,Const}
			var bitAndRegConst = new BitAndOperatorNode (
				                     new HardwareRegisterNode ("a"),
				                     new ConstantNode<long> (2)
			                     );
			results.Add ("bitAndRegConst", bitAndRegConst.Compare (TileFactory.BitAnd.RegConst<long> ()));

			//BitAnd{Const,Reg}
			var bitAndConstReg = new BitAndOperatorNode (
				                     new ConstantNode<long> (2),
				                     new HardwareRegisterNode ("a")
			                     );
			results.Add ("bitAndConstReg", bitAndConstReg.Compare (TileFactory.BitAnd.ConstReg<long> ()));

			//BitOr{Reg,Reg}
			var bitOrRegReg = new BitOrOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new HardwareRegisterNode ("b")
			                  );
			results.Add ("bitOrRegReg", bitOrRegReg.Compare (TileFactory.BitOr.RegReg ()));

			//BitOr{Reg,Const}
			var bitOrRegConst = new BitOrOperatorNode (
				                    new HardwareRegisterNode ("a"),
				                    new ConstantNode<long> (2)
			                    );
			results.Add ("bitOrRegConst", bitOrRegConst.Compare (TileFactory.BitOr.RegConst<long> ()));

			//BitOr{Const,Reg}
			var bitOrConstReg = new BitOrOperatorNode (
				                    new ConstantNode<long> (2),
				                    new HardwareRegisterNode ("a")
			                    );
			results.Add ("bitOrConstReg", bitOrConstReg.Compare (TileFactory.BitOr.ConstReg<long> ()));

			//LogAnd{Reg,Reg}
			var logAndRegReg = new LogAndOperatorNode (
				                   new HardwareRegisterNode ("a"),
				                   new HardwareRegisterNode ("b")
			                   );
			results.Add ("logAndRegReg", logAndRegReg.Compare (TileFactory.LogAnd.RegReg ()));

			//LogAnd{Reg,Const}
			var logAndRegConst = new LogAndOperatorNode (
				                     new HardwareRegisterNode ("a"),
				                     new ConstantNode<long> (2)
			                     );
			results.Add ("logAndRegConst", logAndRegConst.Compare (TileFactory.LogAnd.RegConst ()));

			//LogAnd{Const,Reg}
			var logAndConstReg = new LogAndOperatorNode (
				                     new ConstantNode<long> (2),
				                     new HardwareRegisterNode ("a")
			                     );
			results.Add ("logAndConstReg", logAndConstReg.Compare (TileFactory.LogAnd.ConstReg ()));

			//LogOr{Reg,Reg}
			var logOrRegReg = new LogOrOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new HardwareRegisterNode ("b")
			                  );
			results.Add ("logOrRegReg", logOrRegReg.Compare (TileFactory.LogOr.RegReg ()));

			//LogOr{Reg,Const}
			var logOrRegConst = new LogOrOperatorNode (
				                    new HardwareRegisterNode ("a"),
				                    new ConstantNode<long> (2)
			                    );
			results.Add ("logOrRegConst", logOrRegConst.Compare (TileFactory.LogOr.RegConst<long> ()));

			//LogOr{Const,Reg}
			var logOrConstReg = new LogOrOperatorNode (
				                    new ConstantNode<long> (2),
				                    new HardwareRegisterNode ("a")
			                    );
			results.Add ("logOrConstReg", logOrConstReg.Compare (TileFactory.LogOr.ConstReg<long> ()));

			//unopInc{Reg}
			var unopIncReg = new IncOperatorNode (new HardwareRegisterNode ("a"));
			results.Add ("unopIncReg", unopIncReg.Compare (TileFactory.Unop.Inc_Reg ()));

			//unopDec{Reg}
			var unopDecReg = new DecOperatorNode (new HardwareRegisterNode ("a"));
			results.Add ("unopDecReg", unopDecReg.Compare (TileFactory.Unop.Dec_Reg ()));

			//unopPlus{Reg}
			var unopPlusReg = new UnaryPlusOperatorNode (new HardwareRegisterNode ("a"));
			results.Add ("unopPlusReg", unopPlusReg.Compare (TileFactory.Unop.Plus_Reg ()));

			//unopMinus{Reg}
			var unopMinusReg = new UnaryMinusOperatorNode (new HardwareRegisterNode ("a"));
			results.Add ("unopMinusReg", unopMinusReg.Compare (TileFactory.Unop.Minus_Reg ()));

			//unopNeg{Reg}
			var unopNegReg = new NegOperatorNode (new HardwareRegisterNode ("a"));
			results.Add ("unopNegReg", unopNegReg.Compare (TileFactory.Unop.Neg_Reg ()));

			//unopBinNot{Reg}
			var unopBinNot = new BinNotOperatorNode (new HardwareRegisterNode ("a"));
			results.Add ("unopBinNot", unopBinNot.Compare (TileFactory.Unop.BinNot_Reg ()));

			//unopLogNot{Reg}
			var unopLogNot = new LogNotOperatorNode (new HardwareRegisterNode ("a"));
			results.Add ("unopLogNot", unopLogNot.Compare (TileFactory.Unop.LogNot_Reg ()));

			//cmpEq{Reg,Reg}
			var cmpRegRegEq = new EqOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new HardwareRegisterNode ("b")
			                  );
			results.Add ("cmpRegRegEq", cmpRegRegEq.Compare (TileFactory.Compare.RegReg_Eq ()));

			//cmpNeq{Reg,Reg}
			var cmpRegRegNeq = new NeqOperatorNode (
				                   new HardwareRegisterNode ("a"),
				                   new HardwareRegisterNode ("b")
			                   );
			results.Add ("cmpRegRegNeq", cmpRegRegNeq.Compare (TileFactory.Compare.RegReg_Neq ()));

			//cmpLt{Reg,Reg}
			var cmpRegRegLt = new LtOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new HardwareRegisterNode ("b")
			                  );
			results.Add ("cmpRegRegLt", cmpRegRegLt.Compare (TileFactory.Compare.RegReg_Lt ()));

			//cmpLe{Reg,Reg}
			var cmpRegRegLe = new LteOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new HardwareRegisterNode ("b")
			                  );
			results.Add ("cmpRegRegLe", cmpRegRegLe.Compare (TileFactory.Compare.RegReg_Le ()));

			//cmpGt{Reg,Reg}
			var cmpRegRegGt = new GtOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new HardwareRegisterNode ("b")
			                  );
			results.Add ("cmpRegRegGt", cmpRegRegGt.Compare (TileFactory.Compare.RegReg_Gt ()));

			//cmpGe{Reg,Reg}
			var cmpRegRegGe = new GteOperatorNode (
				                  new HardwareRegisterNode ("a"),
				                  new HardwareRegisterNode ("b")
			                  );
			results.Add ("cmpRegRegGe", cmpRegRegGe.Compare (TileFactory.Compare.RegReg_Ge ()));

			//cmpEq{Reg,Const}
			var cmpRegConstEq = new EqOperatorNode (
				                    new HardwareRegisterNode ("a"),
				                    new ConstantNode<long> (2)
			                    );
			results.Add ("cmpRegConstEq", cmpRegConstEq.Compare (TileFactory.Compare.RegConst_Eq<long> ()));

			//cmpNeq{Reg,Const}
			var cmpRegConstNeq = new NeqOperatorNode (
				new HardwareRegisterNode ("a"),
				new ConstantNode<long> (2)
			);
			results.Add ("cmpRegConstNeq", cmpRegConstNeq.Compare (TileFactory.Compare.RegConst_Neq<long> ()));

			//cmpLt{Reg,Const}
			var cmpRegConstLt = new LtOperatorNode (
				new HardwareRegisterNode ("a"),
				new ConstantNode<long> (2)
			);
			results.Add ("cmpRegConstLt", cmpRegConstLt.Compare (TileFactory.Compare.RegConst_Lt<long> ()));

			//cmpLe{Reg,Const}
			var cmpRegConstLe = new LteOperatorNode (
				new HardwareRegisterNode ("a"),
				new ConstantNode<long> (2)
			);
			results.Add ("cmpRegConstLe", cmpRegConstLe.Compare (TileFactory.Compare.RegConst_Le<long> ()));

			//cmpGt{Reg,Const}
			var cmpRegConstGt = new GtOperatorNode (
				new HardwareRegisterNode ("a"),
				new ConstantNode<long> (2)
			);
			results.Add ("cmpRegConstGt", cmpRegConstGt.Compare (TileFactory.Compare.RegConst_Gt<long> ()));

			//cmpGe{Reg,Const}
			var cmpRegConstGe = new GteOperatorNode (
				new HardwareRegisterNode ("a"),
				new ConstantNode<long> (2)
			);
			results.Add ("cmpRegConstGe", cmpRegConstGe.Compare (TileFactory.Compare.RegConst_Ge<long> ()));

			//cmpEq{Const,Reg}
			var cmpConstRegEq = new EqOperatorNode (
				new ConstantNode<long> (2),
				new HardwareRegisterNode("a")
			);
			results.Add ("cmpConstRegEq", cmpConstRegEq.Compare (TileFactory.Compare.ConstReg_Eq<long> ()));

			//cmpNeq{Const,Reg}
			var cmpConstRegNeq = new NeqOperatorNode (
				new ConstantNode<long> (2),
				new HardwareRegisterNode("a")
			);
			results.Add ("cmpConstRegNeq", cmpConstRegNeq.Compare (TileFactory.Compare.ConstReg_Neq<long> ()));

			//cmpLt{Const,Reg}
			var cmpConstRegLt = new LtOperatorNode (
				new ConstantNode<long> (2),
				new HardwareRegisterNode("a")
			);
			results.Add ("cmpConstRegLt", cmpConstRegLt.Compare (TileFactory.Compare.ConstReg_Lt<long> ()));

			//cmpLe{Const,Reg}
			var cmpConstRegLe = new LteOperatorNode (
				new ConstantNode<long> (2),
				new HardwareRegisterNode("a")
			);
			results.Add ("cmpConstRegLe", cmpConstRegLe.Compare (TileFactory.Compare.ConstReg_Le<long> ()));

			//cmpGt{Const,Reg}
			var cmpConstRegGt = new GtOperatorNode (
				new ConstantNode<long> (2),
				new HardwareRegisterNode("a")
			);
			results.Add ("cmpConstRegGt", cmpConstRegGt.Compare (TileFactory.Compare.ConstReg_Gt<long> ()));

			//cmpGe{Const,Reg}
			var cmpConstRegGe = new GteOperatorNode (
				new ConstantNode<long> (2),
				new HardwareRegisterNode("a")
			);
			results.Add ("cmpConstRegGe", cmpConstRegGe.Compare (TileFactory.Compare.ConstReg_Ge<long> ()));

			foreach(var res in results){
				var key = res.Key;
				var val = res.Value;
				Console.WriteLine (key);
				Assert.IsTrue (val.Item1); // compare returned true
			}
		}
	}
}
