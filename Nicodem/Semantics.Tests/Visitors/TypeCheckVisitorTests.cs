using System;
using NUnit.Framework;
using Nicodem.Semantics.Visitors;
using Nicodem.Semantics.AST;

namespace Semantics.Tests.Visitors
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

        /*
         * Block Expression 
         * 1. [] - Void
         * 2. [1; "s"; 2] - Int
         */
        [Test]
        public void TypeCheck_BlockExpressionEmpty_Tests()
        {
            var _block_exp = Utils.body();

            _block_exp.Accept(new TypeCheckVisitor());

            Assert.NotNull(_block_exp.ExpressionType);
            Assert.IsTrue(TypeNode.Compare(_block_exp.ExpressionType, Utils.MakeConstantVoid()));
        }

        [Test]
        public void TypeCheck_BlockExpressionThreeElements_Tests()
        {
            var _1 = Utils.Usage(Utils.Declaration("1", Utils.MakeConstantInt()));
            var _s = Utils.Usage(Utils.Declaration("s", Utils.MakeConstantChar()));
            var _2 = Utils.Usage(Utils.Declaration("2", Utils.MakeConstantInt()));
            var _block_exp = Utils.body(_1, _s, _2);

            _block_exp.Accept(new TypeCheckVisitor());

            Assert.NotNull(_block_exp.ExpressionType);
            Assert.IsTrue(TypeNode.Compare(_block_exp.ExpressionType, Utils.MakeConstantInt()));
        }
	}
}

