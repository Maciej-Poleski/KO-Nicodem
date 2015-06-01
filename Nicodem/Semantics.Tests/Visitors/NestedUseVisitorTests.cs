using Nicodem.Semantics.AST;
using Nicodem.Semantics.Visitors;
using NUnit.Framework;

namespace Semantics.Tests.Visitors
{
	[TestFixture]
    public class NestedUseVisitorTests
    {
        [Test]
        public void NoNestedUses()
        {
            /* f(int mutable a, int mutable b) -> int
             * {
             *   int mutable c = -1
             *   byte mutable d = 0
             *   g(char mutable a, bool mutable e) -> void
             *   {
             *     int mutable f = 5
             *     a = 'a'
             *     e = false
             *     f = 3
             *   }
             *   a = 1
             *   b = 2
             *   c = 3
             *   d = 4
             *   b
             * }
             */

            // function g
			var gParamA = Utils.DeclareChar ("a");
			var gParamE = Utils.DeclareBool ("e");
			var gBodyEx1 = Utils.DefineInt ("f", 5);
			var gParamAuse = Utils.Usage (gParamA);
			var gParamEuse = Utils.Usage (gParamE);
			var gBodyEx1use = Utils.Usage (gBodyEx1);
			var gBodyEx2 = Utils.Assignment (gParamAuse, Utils.CharLiteral ('a'));
			var gBodyEx3 = Utils.Assignment (gParamEuse, Utils.BoolLiteral (false));
			var gBodyEx4 = Utils.Assignment (gBodyEx1use, Utils.IntLiteral (3));
			var gFunction = Utils.FunctionDef ("g",
				Utils.parameters (gParamA, gParamE),
				Utils.MakeConstantVoid (),
			    Utils.body (gBodyEx1, gBodyEx2, gBodyEx3, gBodyEx4)
			);

            // function f
			var fParamA = Utils.DeclareInt ("a");
			var fParamB = Utils.DeclareInt ("b");
			var fBodyEx1 = Utils.DefineInt ("c", -1);
			var fBodyEx2 = Utils.DefineByte ("d", 0);
			var fParamAuse = Utils.Usage (fParamA);
			var fParamBuse = Utils.Usage (fParamB);
			var fBodyEx1use = Utils.Usage (fBodyEx1);
			var fBodyEx2use = Utils.Usage (fBodyEx2);
			var fBodyEx3 = gFunction;
			var fBodyEx4 = Utils.Assignment (fParamAuse, Utils.IntLiteral (1));
			var fBodyEx5 = Utils.Assignment (fParamBuse, Utils.IntLiteral (2));
			var fBodyEx6 = Utils.Assignment (fBodyEx1use, Utils.IntLiteral (3));
			var fBodyEx7 = Utils.Assignment (fBodyEx2use, Utils.IntLiteral (4));
            var fBodyEx8 = fParamBuse;
			var fFunction = Utils.FunctionDef ("f",
				Utils.parameters (fParamA, fParamB),
				Utils.MakeConstantInt (),
				Utils.body (fBodyEx1, fBodyEx2, fBodyEx3, fBodyEx4, fBodyEx5, fBodyEx6, fBodyEx7, fBodyEx8)
			);

			var program = Utils.Program (fFunction);

            program.FillInNestedUseFlag();

			Assert.False (gParamA.NestedUse);
			Assert.False (gParamE.NestedUse);
			Assert.False (gBodyEx1.NestedUse);
			Assert.False (fParamA.NestedUse);
			Assert.False (fParamB.NestedUse);
			Assert.False (fBodyEx1.NestedUse);
			Assert.False (fBodyEx2.NestedUse);
        }

        [Test]
        public void SimpleNestedUses()
        {
            /* f(int mutable a, int mutable b) -> int
             * {
             *   int mutable c = -1
             *   byte mutable d = 0
             *   g(char g, bool e) -> void
             *   {
             *     int mutable f = 5
             *     a = 1
             *     e = false
             *     f = 3
             *     d = 2
             *   }
             *   a = 1
             *   b = 2
             *   c = 3
             *   d = 4
             *   b
             * }
             */

			var fParamA = Utils.DeclareInt ("a");
			var fParamB = Utils.DeclareInt ("b");
			var fVarC = Utils.DefineInt ("c", -1);
			var fVarD = Utils.DefineByte ("d", 0);
			var fParamAuse = Utils.Usage (fParamA);
			var fParamBuse = Utils.Usage (fParamB);
			var fVarCuse = Utils.Usage (fVarC);
			var fVarDuse = Utils.Usage (fVarD);

            // function g
			var gParamG = Utils.DeclareChar ("g");
			var gParamE = Utils.DeclareBool ("e");
			var gBodyEx1 = Utils.DefineInt ("f", 5);
			var gParamEuse = Utils.Usage (gParamE);
			var gBodyEx1use = Utils.Usage (gBodyEx1);
			var gBodyEx2 = Utils.Assignment (fParamAuse, Utils.IntLiteral (1));
			var gBodyEx3 = Utils.Assignment (gParamEuse, Utils.BoolLiteral (false));
			var gBodyEx4 = Utils.Assignment (gBodyEx1use, Utils.IntLiteral (3));
			var gBodeEx5 = Utils.Assignment (fVarDuse, Utils.IntLiteral (2));
			var gFunction = Utils.FunctionDef ("g",
				Utils.parameters (gParamG, gParamE),
				Utils.MakeConstantVoid (),
				Utils.body (gBodyEx1, gBodyEx2, gBodyEx3, gBodyEx4, gBodeEx5)
			);

            // function f
            var fBodyEx1 = fVarC;
            var fBodyEx2 = fVarD;
            var fBodyEx3 = gFunction;
			var fBodyEx4 = Utils.Assignment (fParamAuse, Utils.IntLiteral (1));
			var fBodyEx5 = Utils.Assignment (fParamBuse, Utils.IntLiteral (2));
			var fBodyEx6 = Utils.Assignment (fVarCuse, Utils.IntLiteral (3));
			var fBodyEx7 = Utils.Assignment (fVarDuse, Utils.ByteLiteral (4));
			var fBodyEx8 = fParamBuse;
			var fFunction = Utils.FunctionDef ("f",
				Utils.parameters (fParamA, fParamB),
				Utils.MakeConstantInt (),
				Utils.body (fBodyEx1, fBodyEx2, fBodyEx3, fBodyEx4, fBodyEx5, fBodyEx6, fBodyEx7, fBodyEx8)
			);

			var program = Utils.Program (fFunction);

            program.FillInNestedUseFlag();

			Assert.False (gParamG.NestedUse);
			Assert.False (gParamE.NestedUse);
			Assert.False (gBodyEx1.NestedUse);
			Assert.True (fParamA.NestedUse);
			Assert.False (fParamB.NestedUse);
			Assert.False (fVarC.NestedUse);
			Assert.True (fVarD.NestedUse);
        }
	}
}