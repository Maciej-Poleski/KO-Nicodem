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
		 * f (mutable int a) -> mutable int
		 * {
		 * 		a = 1
		 * }
		 */
		[Test]
        public void TypeCheck_TestSimpleFunction()
        {
			var fParamA = Utils.DeclareInt ("a");
			var fBody = Utils.Assignment (fParamA, Utils.IntLiteral (1));
			var fFunction = Utils.FunctionDef ("f", Utils.parameters (fParamA), Utils.MakeMutableInt (), Utils.body (fBody));
			var program = Utils.Program (fFunction);
			program.Accept (new TypeCheckVisitor ());

			Assert.NotNull (fParamA.ExpressionType);
			Assert.NotNull (fBody.ExpressionType);
			Assert.NotNull (fFunction.ExpressionType);
			Assert.IsTrue (fBody.ExpressionType.Equals(fParamA.ExpressionType));
		}

        /*
         * [
         * int a = 3;
         * a = a + 1;
         * ]
         */
        [Test]
        [ExpectedException(typeof(TypeCheckException), ExpectedMessage = "Variable to assign is constant.")]
        public void TypeCheck_immutableTryAssingSum_Test()
        {
            var a = Utils.DefineConstantInt("a", 3);
            var a_use = Utils.Usage(a);
            var _a_plus_1 = Utils.Add(a_use, Utils.IntLiteral(1));
            var a_assign = Utils.Assignment(a_use, _a_plus_1);
            var be = Utils.body(new ExpressionNode[] { a, a_assign });

            be.Accept(new TypeCheckVisitor());
        }

        /*
         * [
         * int a = 3;
         * a = 1;
         * ]
         */
        [Test]
        [ExpectedException(typeof(TypeCheckException), ExpectedMessage = "Variable to assign is constant.")]
        public void TypeCheck_immutableTryAssingLiteral_Test()
        {
            var a = Utils.DefineConstantInt("a", 3);
            var a_use = Utils.Usage(a);
            var a_assign = Utils.Assignment(a_use, Utils.IntLiteral(1));
            var be = Utils.body(new ExpressionNode[] { a, a_assign });

            be.Accept(new TypeCheckVisitor());
        }

		/*
		 * 		if (true) {
		 * 			1
		 * 		} else {
		 * 			2
		 * 		}
		 */
		[Test]
        public void TypeCheck_TestIf1()
        {
			var then = Utils.IntLiteral (1);
			var ifst = Utils.IfElse (Utils.BoolLiteral (true), then, Utils.IntLiteral (2));
			ifst.Accept (new TypeCheckVisitor ());
			Assert.NotNull (ifst.ExpressionType);
			Assert.IsTrue (ifst.ExpressionType.Equals(then.ExpressionType));
		}

		/*
		 * mutable int a = 5
		 * while (a>0) {
		 *  	a = a - 1;
		 * }
		 */
		[Test]
        public void TypeCheck_TestWhile()
        {
			var a = Utils.DefineInt ("a", 5);
			var minus = Utils.Sub (a, Utils.IntLiteral (1));
			var whileExp1 = Utils.Definition (a, minus); 
			var whileCond = Utils.Greater (Utils.Usage (a), Utils.IntLiteral (0));
			var whileBody = Utils.body (whileExp1);
			var whileSt = Utils.While (whileCond, whileBody);

            a.Accept(new TypeCheckVisitor());
			whileSt.Accept (new TypeCheckVisitor ());
			Assert.IsTrue(minus.ExpressionType.Equals(Utils.MakeConstantInt(), false));
			Assert.NotNull (whileSt.ExpressionType);
			Assert.IsTrue (whileBody.ExpressionType.Equals (whileExp1.ExpressionType));
			Assert.IsFalse (whileSt.ExpressionType.Equals (whileBody.ExpressionType));
			Assert.IsTrue (whileSt.ExpressionType.Equals (Utils.MakeConstantVoid(), false));
		}

		/*
		 * int c = 1;
		 * int b = c;
		 */
		[Test]
		public void test_def() {
			var c = Utils.DefineInt ("c", 1);
			var b = Utils.Definition (Utils.DeclareInt("b"), Utils.Usage (c));
			b.Accept (new TypeCheckVisitor ());
			Assert.NotNull (b.Value.ExpressionType);
		}

		/* main() -> void
		 * {
		 * 	gcd(mutable int a, mutable int b) -> mutable int 
		 * 	{
		 * 		if a<b {
		 * 			mutable int c = a
		 * 			a = b
		 * 			b = c
		 * 		}
		 * 		while(b>0) {
		 * 			mutable int c = a
		 * 			a = b
		 * 			b = c % b
		 * 		}
		 * 		a
		 * 	}
		 *  if { 
		 * 		mutable int a = 20
		 * 		mutable int b = 10
		 * 		gcd(a,b) > 1
		 * 		} 1
		 * }
		 */
		// if the last if-statement is executed, main's body will have type INT, otherwise the type will be VOID ? Can't be...
		[Test]
        public void TypeCheck_TestLarge()
        {

			// gcd:
				// args
				var a = Utils.DeclareInt ("a");
				var b = Utils.DeclareInt ("b");
				
				// if
				var ifC = Utils.DeclareInt("c");
				var ifExp1 = Utils.Definition (ifC, Utils.Usage (a));  
				var ifExp2 = Utils.Assignment (Utils.Usage(a), Utils.Usage(b));
				var ifExp3 = Utils.Assignment (Utils.Usage(b), Utils.Usage (ifC));

				var ifCond = Utils.Less (Utils.Usage (a), Utils.Usage (b));
				var ifThen = Utils.body (ifExp1, ifExp2, ifExp3);
				var ifst = Utils.If (ifCond, ifThen);
		
				// while
				var whileC = Utils.DeclareInt("c");
				var whileExp1 = Utils.Definition (whileC, Utils.Usage (a)); 
				var whileExp2 = Utils.Definition (a, Utils.Usage (b));
				var modulo = Utils.Modulo (Utils.Usage (whileC), Utils.Usage (b));
				var whileExp3 = Utils.Definition (b, modulo);
				var whileCond = Utils.Greater (Utils.Usage (b), Utils.IntLiteral (0));
				var whileBody = Utils.body (whileExp1, whileExp2, whileExp3);
				var whileSt = Utils.While (whileCond, whileBody);

			var gcdBody = Utils.body (ifst, whileSt, Utils.Usage (a));
			var gcdFuncDef = Utils.FunctionDef ("gcd", Utils.parameters (a, b), Utils.MakeMutableInt (), gcdBody);

			// main
				// gcd - above
				// if
				a = Utils.DefineInt ("a", 20);
				b = Utils.DefineInt ("b", 10);
				var gcdCall = Utils.FunctionCall ("gcd", a, b);
				var mainIfCond = Utils.Greater (gcdCall, Utils.IntLiteral(1));
			var mainIf = Utils.If (mainIfCond, Utils.IntLiteral(1));

			var mainBody = Utils.body (gcdFuncDef, mainIf);
			var mainFuncDef = Utils.FunctionDef("main", Utils.parameters(), Utils.MakeConstantVoid(), mainBody);

			mainFuncDef.Accept (new NameResolutionVisitor ());
			mainFuncDef.Accept (new TypeCheckVisitor ());
			Assert.NotNull (gcdBody.ExpressionType);
			Assert.NotNull (gcdFuncDef.ExpressionType);
			Assert.NotNull (gcdCall.ExpressionType);
			Assert.IsTrue (gcdCall.ExpressionType.Equals (gcdFuncDef.ResultType));
			Assert.NotNull (mainFuncDef.ExpressionType);
			Assert.IsTrue (mainBody.ExpressionType.Equals (mainIf.ExpressionType));
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
            Assert.IsTrue(_block_exp.ExpressionType.Equals(Utils.MakeConstantVoid(), false));
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

        /*
         * int x = if true 5 else 2-3
         */
        [Test]
        public void TypeCheck_AssignIf_Test()
        {
            var _2_sub_3 = Utils.Sub(Utils.IntLiteral(2), Utils.IntLiteral(3));
            var _if = Utils.IfElse(Utils.BoolLiteral(true), Utils.IntLiteral(5), _2_sub_3);

            var _decl = Utils.Declaration("x", NamedTypeNode.IntType());
            var _def = Utils.Definition(_decl, _if);

            _def.Accept(new TypeCheckVisitor());

            Assert.IsTrue(NamedTypeNode.IntType().Equals(_def.ExpressionType));
        }
	}
}

