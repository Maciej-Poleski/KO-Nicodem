using NUnit.Framework;
using Nicodem.Semantics.Visitors;

namespace Semantics.Tests
{
	[TestFixture]
	public class NameResolutionVisitorTests
	{
		/*
		 * f(int mutable a) -> int
		 * {
		 *   g(byte mutable b) -> byte
		 *   {
		 *     4
		 *   }
		 *   g(12)
		 *   h(int mutable b) -> int
		 *   {
		 *     44
		 *   }
		 *   h(3)
		 *   g(h(0))
		 * }
		 */
		[Test]
		public void FunctionCall() {
			// function g
			var gArg = Utils.DeclareByte ("b");
			var gExpr = Utils.ByteLiteral (4);
			var gFunction = Utils.FunctionDef ("g",
				Utils.parameters (gArg),
				Utils.MakeConstantByte (),
				Utils.body (gExpr)
			);

			// function h
			var hArg = Utils.DeclareInt ("b");
			var hExpr = Utils.IntLiteral (44);
			var hFunction = Utils.FunctionDef ("h",
				Utils.parameters (hArg),
				Utils.MakeConstantInt (),
				Utils.body (hExpr)
			);

			//function f
			var fParam = Utils.DeclareInt ("a");
			var fExprCallg12 = Utils.FunctionCall ("g", Utils.ByteLiteral (12));
			var fExprCallh3 = Utils.FunctionCall ("h", Utils.ByteLiteral (3));
			var fExprCallgh0Inner = Utils.FunctionCall ("h", Utils.ByteLiteral (0));
			var fExprCallgh0 = Utils.FunctionCall ("g", fExprCallgh0Inner);
			var fFunction = Utils.FunctionDef ("f",
				Utils.parameters (fParam),
				Utils.MakeConstantInt (),
				Utils.body (gFunction, fExprCallg12, hFunction, fExprCallh3, fExprCallgh0)
			);

			Assert.IsNull (fExprCallg12.Definition);
			Assert.IsNull (fExprCallgh0.Definition);
			Assert.IsNull (fExprCallgh0Inner.Definition);
			Assert.IsNull (fExprCallh3.Definition);

			var program = Utils.Program (fFunction);
			program.Accept (new NameResolutionVisitor ());

			Assert.AreEqual (gFunction, fExprCallg12.Definition);
			Assert.AreEqual (gFunction, fExprCallgh0.Definition);
			Assert.AreEqual (hFunction, fExprCallgh0Inner.Definition);
			Assert.AreEqual (hFunction, fExprCallh3.Definition);
		}

		/*
		 * f(int mutable a) -> int
		 * {
		 *   int mutable c
		 *   g(byte mutable b) -> byte
		 *   {
		 *     c = 12
		 *     4
		 *   }
		 *   int mutable a
		 *   int mutable b
		 *   g(12)
		 *   h(int mutable b) -> int
		 *   {
		 *     44
		 *     c = a - b
		 *     a = b
		 *   }
		 *   b = 12
		 *   h(3)
		 *   g(h(a+b))
		 * }
		 */
		[Test]
		public void Complex() {
			var fVarC = Utils.DeclareInt ("c");
			var fVarA = Utils.DeclareInt ("a");
			var fVarB = Utils.DeclareInt ("b");

			// function g
			var gArg = Utils.DeclareByte ("b");
			var gUseC = Utils.Usage (fVarC, false);
			var gExpr1 = Utils.Assignment (gUseC, Utils.IntLiteral (12));
			var gExpr2 = Utils.ByteLiteral (4);
			var gFunction = Utils.FunctionDef ("g",
				Utils.parameters (gArg),
				Utils.MakeConstantByte (),
				Utils.body (gExpr1, gExpr2)
			);

			// function h
			var hArg = Utils.DeclareInt ("b");
			var hExpr1 = Utils.IntLiteral (44);
			var hUseC = Utils.Usage (fVarC, false);
			var hUseA = Utils.Usage (fVarA, false);
			var hUseB = Utils.Usage (hArg, false);
			var hExpr2 = Utils.Assignment (hUseC, Utils.Sub (hUseA, hUseB));
			var hExpr3 = Utils.Assignment (hUseA, hUseB);
			var hFunction = Utils.FunctionDef ("h",
				Utils.parameters (hArg),
				Utils.MakeConstantInt (),
				Utils.body (hExpr1, hExpr2, hExpr3)
			);

			//function f
			var fParam = Utils.DeclareInt ("a");
			var useB = Utils.Usage (fVarB, false);
			var useA = Utils.Usage (fVarA, false);
			var fExpr0 = Utils.Assignment (useB, Utils.IntLiteral (12));
			var fExprCallg12 = Utils.FunctionCall ("g", Utils.ByteLiteral (12));
			var fExprCallh3 = Utils.FunctionCall ("h", Utils.ByteLiteral (3));
			var fExprCallgh0Inner = Utils.FunctionCall ("h", Utils.Add (useA, useB));
			var fExprCallgh0 = Utils.FunctionCall ("g", fExprCallgh0Inner);
			var fFunction = Utils.FunctionDef ("f",
				Utils.parameters (fParam),
				Utils.MakeConstantInt (),
				Utils.body (fVarC, gFunction, fVarA, fVarB, fExprCallg12, hFunction, fExpr0, fExprCallh3, fExprCallgh0)
			);

			Assert.IsNull (gUseC.Declaration);
			Assert.IsNull (hUseA.Declaration);
			Assert.IsNull (hUseB.Declaration);
			Assert.IsNull (hUseC.Declaration);
			Assert.IsNull (useA.Declaration);
			Assert.IsNull (useB.Declaration);
			Assert.IsNull (fExprCallg12.Definition);
			Assert.IsNull (fExprCallgh0.Definition);
			Assert.IsNull (fExprCallgh0Inner.Definition);
			Assert.IsNull (fExprCallh3.Definition);

			var program = Utils.Program (fFunction);
			program.Accept (new NameResolutionVisitor ());

			Assert.AreEqual (fVarC, gUseC.Declaration);
			Assert.AreEqual (fVarC, hUseC.Declaration);
			Assert.AreEqual (fVarA, hUseA.Declaration);
			Assert.AreEqual (hArg, hUseB.Declaration);
			Assert.AreEqual (fVarA, useA.Declaration);
			Assert.AreEqual (fVarB, useB.Declaration);
			Assert.AreEqual (gFunction, fExprCallg12.Definition);
			Assert.AreEqual (gFunction, fExprCallgh0.Definition);
			Assert.AreEqual (hFunction, fExprCallgh0Inner.Definition);
			Assert.AreEqual (hFunction, fExprCallh3.Definition);
		}
	}
}

