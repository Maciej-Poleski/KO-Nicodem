using System.Collections.Generic;
using System.Diagnostics;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Visitors;
using NUnit.Framework;
using Nicodem.Core;

namespace Semantics.Tests.Visitors
{
    internal class NestedUseVisitorTests
    {
        private const PrimitiveType Int = PrimitiveType.Int;
        private const PrimitiveType Byte = PrimitiveType.Byte;
        private const PrimitiveType Char = PrimitiveType.Char;
        private const PrimitiveType Bool = PrimitiveType.Bool;
        private const PrimitiveType Void = PrimitiveType.Void;
        private const Mutablitity Constant = Mutablitity.Constant;
        private const Mutablitity Mutable = Mutablitity.Mutable;

        [TestFixtureSetUp]
        public void init()
        {
            // We don't want message boxes in testing.
            TestsTraceListener.Setup(() => Assert.Fail());
        }

        [Test, Timeout(1000)]
        public void NoNestedUses()
        {
            Debug.Assert(false);
            return;
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
            var gParamA = new VariableDeclNode
            {
                Name = "a",
                Type = MakePrimitiveType(Char, Mutable)
            };
            var gParamE = new VariableDeclNode
            {
                Name = "e",
                Type = MakePrimitiveType(Bool, Mutable)
            };
            var gBodyEx1 = new VariableDefNode
            {
                Name = "f",
                Type = MakePrimitiveType(Int, Mutable),
                Value = IntLiteral(5)
            };
            var gBodyEx2 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {gParamA.Use(), CharLiteral('a')}
            };
            var gBodyEx3 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {gParamE.Use(), BoolLiteral(false)}
            };
            var gBodyEx4 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {gBodyEx1.Use(), IntLiteral(3)}
            };
            var gBody = new BlockExpressionNode
            {
                Elements = new ExpressionNode[] {gBodyEx1, gBodyEx2, gBodyEx3, gBodyEx4}
            };
            var gFunction = new FunctionDefinitionExpression
            {
                Name = "g",
                Parameters = new[] {gParamA, gParamE},
                ResultType = MakePrimitiveType(Void),
                Body = gBody
            };

            // function f
            var fParamA = new VariableDeclNode
            {
                Name = "a",
                Type = MakePrimitiveType(Int, Mutable)
            };
            var fParamB = new VariableDeclNode
            {
                Name = "b",
                Type = MakePrimitiveType(Int, Mutable)
            };
            var fBodyEx1 = new VariableDefNode
            {
                Name = "c",
                Type = MakePrimitiveType(Int, Mutable),
                Value = IntLiteral(-1)
            };
            var fBodyEx2 = new VariableDefNode
            {
                Name = "d",
                Type = MakePrimitiveType(Byte, Mutable),
                Value = ByteLiteral(0)
            };
            var fBodyEx3 = gFunction;
            var fBodyEx4 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fParamA.Use(), IntLiteral(1)}
            };
            var fBodyEx5 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fParamB.Use(), IntLiteral(2)}
            };
            var fBodyEx6 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fBodyEx1.Use(), IntLiteral(3)}
            };
            var fBodyEx7 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fBodyEx2.Use(), ByteLiteral(4)}
            };
            var fBodyEx8 = fParamB.Use();
            var fBody = new BlockExpressionNode
            {
                Elements =
                    new ExpressionNode[]
                    {fBodyEx1, fBodyEx2, fBodyEx3, fBodyEx4, fBodyEx5, fBodyEx6, fBodyEx7, fBodyEx8}
            };
            var fFunction = new FunctionDefinitionExpression
            {
                Name = "f",
                Parameters = new[] {fParamA, fParamB},
                ResultType = MakePrimitiveType(Int),
                Body = fBody
            };

            // Program
            var program = new ProgramNode(new LinkedList<FunctionDefinitionExpression>(new[] {fFunction}));

            program.FillInNestedUseFlag();

            Assert.False(gParamA.NestedUse);
            Assert.False(gParamE.NestedUse);
            Assert.False(gBodyEx1.NestedUse);
            Assert.False(fParamA.NestedUse);
            Assert.False(fParamB.NestedUse);
            Assert.False(fBodyEx1.NestedUse);
            Assert.False(fBodyEx2.NestedUse);
        }

        [Test, Timeout(1000)]
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

            var fParamA = new VariableDeclNode
            {
                Name = "a",
                Type = MakePrimitiveType(Int, Mutable)
            };
            var fParamB = new VariableDeclNode
            {
                Name = "b",
                Type = MakePrimitiveType(Int, Mutable)
            };
            var fVarC = new VariableDefNode
            {
                Name = "c",
                Type = MakePrimitiveType(Int, Mutable),
                Value = IntLiteral(-1)
            };
            var fVarD = new VariableDefNode
            {
                Name = "d",
                Type = MakePrimitiveType(Byte, Mutable),
                Value = ByteLiteral(0)
            };

            // function g
            var gParamG = new VariableDeclNode
            {
                Name = "g",
                Type = MakePrimitiveType(Char)
            };
            var gParamE = new VariableDeclNode
            {
                Name = "e",
                Type = MakePrimitiveType(Bool)
            };
            var gBodyEx1 = new VariableDefNode
            {
                Name = "f",
                Type = MakePrimitiveType(Int, Mutable),
                Value = IntLiteral(5)
            };
            var gBodyEx2 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fParamA.Use(), IntLiteral(1)}
            };
            var gBodyEx3 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {gParamE.Use(), BoolLiteral(false)}
            };
            var gBodyEx4 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {gBodyEx1.Use(), IntLiteral(3)}
            };
            var gBodeEx5 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fVarD.Use(), IntLiteral(2)}
            };
            var gBody = new BlockExpressionNode
            {
                Elements = new ExpressionNode[] {gBodyEx1, gBodyEx2, gBodyEx3, gBodyEx4, gBodeEx5}
            };
            var gFunction = new FunctionDefinitionExpression
            {
                Name = "g",
                Parameters = new[] {gParamG, gParamE},
                ResultType = MakePrimitiveType(Void),
                Body = gBody
            };

            // function f
            var fBodyEx1 = fVarC;
            var fBodyEx2 = fVarD;
            var fBodyEx3 = gFunction;
            var fBodyEx4 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fParamA.Use(), IntLiteral(1)}
            };
            var fBodyEx5 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fParamB.Use(), IntLiteral(2)}
            };
            var fBodyEx6 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fBodyEx1.Use(), IntLiteral(3)}
            };
            var fBodyEx7 = new OperatorNode
            {
                Operator = OperatorType.OT_ASSIGNMENT,
                Arguments = new ExpressionNode[] {fBodyEx2.Use(), ByteLiteral(4)}
            };
            var fBodyEx8 = fParamB.Use();
            var fBody = new BlockExpressionNode
            {
                Elements =
                    new ExpressionNode[]
                    {fBodyEx1, fBodyEx2, fBodyEx3, fBodyEx4, fBodyEx5, fBodyEx6, fBodyEx7, fBodyEx8}
            };
            var fFunction = new FunctionDefinitionExpression
            {
                Name = "f",
                Parameters = new[] {fParamA, fParamB},
                ResultType = MakePrimitiveType(Int),
                Body = fBody
            };

            // Program
            var program = new ProgramNode(new LinkedList<FunctionDefinitionExpression>(new[] {fFunction}));

            program.FillInNestedUseFlag();

            Assert.False(gParamG.NestedUse);
            Assert.False(gParamE.NestedUse);
            Assert.False(gBodyEx1.NestedUse);
            Assert.True(fParamA.NestedUse);
            Assert.False(fParamB.NestedUse);
            Assert.False(fVarC.NestedUse);
            Assert.True(fVarD.NestedUse);
        }

        private static NamedTypeNode MakePrimitiveType(PrimitiveType type, Mutablitity isConstant = Mutablitity.Constant)
        {
            NamedTypeNode result = null;
            switch (type)
            {
                case PrimitiveType.Int:
                    result = NamedTypeNode.IntType();
                    break;
                case PrimitiveType.Byte:
                    result = NamedTypeNode.ByteType();
                    break;
                case PrimitiveType.Char:
                    result = NamedTypeNode.CharType();
                    break;
                case PrimitiveType.Bool:
                    result = NamedTypeNode.BoolType();
                    break;
                case PrimitiveType.Void:
                    result = NamedTypeNode.VoidType();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            result.IsConstant = isConstant == Mutablitity.Constant;
            return result;
        }

        private static AtomNode IntLiteral(int value)
        {
            return new AtomNode(MakePrimitiveType(Int)) {Value = value.ToString()};
        }

        private static AtomNode CharLiteral(char value)
        {
            return new AtomNode(MakePrimitiveType(Char)) {Value = "'" + value + "'"};
        }

        private static AtomNode BoolLiteral(bool value)
        {
            return new AtomNode(MakePrimitiveType(Bool)) {Value = value ? "true" : "false"};
        }

        private static AtomNode ByteLiteral(int value)
        {
            return new AtomNode(MakePrimitiveType(Void)) {Value = value.ToString()};
        }

        private enum PrimitiveType
        {
            Int,
            Byte,
            Char,
            Bool,
            Void
        }

        private enum Mutablitity
        {
            Constant,
            Mutable
        }
    }

    internal static class Extensions
    {
        internal static VariableUseNode Use(this VariableDeclNode node)
        {
            return new VariableUseNode
            {
                Name = node.Name,
                Declaration = node
            };
        }
    }
}