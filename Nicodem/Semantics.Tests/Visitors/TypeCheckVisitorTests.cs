using System;
using NUnit.Framework;
using Nicodem.Semantics.Visitors;
using Nicodem.Semantics.AST;

namespace Semantics.Tests
{
	[TestFixture]
	public class TypeCheckVisitorTests
	{
		/*
		 * f (mutable int a) -> int
		 * {
		 * 		a = 1
		 * }
		 */
		[Test]
		public void TestUtilsSimpleFunction() {
			var fParamA = Utils.DeclareInt ("a");
			var fBody = Utils.Assignment (fParamA, Utils.IntLiteral (1));
			var fFunction = Utils.FunctionDef ("f", Utils.parameters (fParamA), Utils.MakeConstantInt (), Utils.body (fBody));
			var program = Utils.Program (fFunction);
			program.Accept (new TypeCheckVisitor ());

			Assert.NotNull (fParamA.ExpressionType);
			Assert.NotNull (fBody.ExpressionType);
			Assert.NotNull (fFunction.ExpressionType);
			Assert.IsTrue (TypeNode.Compare(fBody.ExpressionType, fParamA.ExpressionType));
		}

		/*
		 * 		if (true) {
		 * 			1
		 * 		} else {
		 * 			2
		 * 		}
		 */
		[Test]
		public void TestIf1() {
			var then = Utils.IntLiteral (1);
			var ifst = Utils.If (Utils.BoolLiteral (true), then, Utils.IntLiteral (2));
			ifst.Accept (new TypeCheckVisitor ());
			Assert.NotNull (ifst.ExpressionType);
			Assert.IsTrue (TypeNode.Compare(ifst.ExpressionType, then.ExpressionType));
		}
	}
}

