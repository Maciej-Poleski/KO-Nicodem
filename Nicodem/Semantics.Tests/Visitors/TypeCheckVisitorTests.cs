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
			Assert.IsTrue (fBody.ExpressionType.Equals(fParamA.ExpressionType));
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
			Assert.IsTrue (ifst.ExpressionType.Equals(then.ExpressionType));
		}

        /*
         * Block Expression 
         * 1. [] - Void
         * 2. [1; "s"; 2] - Int
         */
        [Test]
        public void TypeCheck_BlockExpressionEmpty_Test()
        {
            var _block_exp = Utils.body();

            _block_exp.Accept(new TypeCheckVisitor());

            Assert.NotNull(_block_exp.ExpressionType);
            Assert.IsTrue(_block_exp.ExpressionType.Equals(Utils.MakeConstantVoid()));
        }

        [Test]
        public void TypeCheck_BlockExpressionThreeElements_Test()
        {
            var _block_exp = Utils.body(Utils.IntLiteral(1), Utils.CharLiteral('s'), Utils.IntLiteral(2));

            _block_exp.Accept(new TypeCheckVisitor());

            Assert.NotNull(_block_exp.ExpressionType);
            Assert.IsTrue(_block_exp.ExpressionType.Equals(Utils.MakeConstantInt()));
        }

        /*
         * Atom
         * 1. Int
         * 2. Char
         * 3. Byte
         * 
         */
        [Test]
        public void TypeCheck_AtomInt_Test()
        {
            var _atom_int = Utils.IntLiteral(1);

            _atom_int.Accept(new TypeCheckVisitor());

            Assert.NotNull(_atom_int.ExpressionType);
            Assert.IsTrue(_atom_int.ExpressionType.Equals(Utils.MakeConstantInt()));
        }


        [Test]
        public void TypeCheck_AtomChar_Test()
        {
            
            var _atom_char = Utils.CharLiteral('s');

            _atom_char.Accept(new TypeCheckVisitor());

            Assert.NotNull(_atom_char.ExpressionType);
            Assert.IsTrue(_atom_char.ExpressionType.Equals(Utils.MakeConstantChar()));
        }


        [Test]
        public void TypeCheck_AtomByte_Test()
        {
            
            var _atom_byte = Utils.ByteLiteral(1);

            _atom_byte.Accept(new TypeCheckVisitor());

            Assert.NotNull(_atom_byte.ExpressionType);
            Assert.IsTrue(_atom_byte.ExpressionType.Equals(Utils.MakeConstantByte()));
        }
	}
}

