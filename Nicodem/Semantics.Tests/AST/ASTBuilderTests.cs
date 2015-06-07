using NUnit.Framework;
using System;
using Nicodem.Parser;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Grammar;
using Nicodem.Source;

namespace Semantics.Tests
{
    using ParseTree = IParseTree<Symbol>;
    using P = NicodemGrammarProductions;
    using UniversalSymbol = NicodemGrammarProductions.UniversalSymbol;
    using Nicodem.Core;
    using System.Collections.Generic;

    [TestFixture()]
    public class ASTBuilderTests
    {
        private class TestSymbol : ISymbol<TestSymbol>
        {
            public TestSymbol(bool isTerminal, string description){
                Description = description;
                IsTerminal = isTerminal;
            }

            #region IEquatable implementation
            public bool Equals(TestSymbol other)
            {
                throw new NotImplementedException();
            }
            #endregion
            #region IComparable implementation
            public int CompareTo(TestSymbol other)
            {
                throw new NotImplementedException();
            }
            #endregion
            #region ISymbol implementation
            public string Description { get; private set; }
            public bool IsTerminal { get; private set; }
            #endregion
        }

        private class DummyFragment : IFragment
        {
            private string text;

            public DummyFragment(string text)
            {
                this.text = text;
            }

            public IOrigin Origin
            {
                get { throw new NotImplementedException(); }
            }
            public OriginPosition GetBeginOriginPosition() { throw new NotImplementedException(); }
            public OriginPosition GetEndOriginPosition() { throw new NotImplementedException(); }

            public string GetOriginText() { return text; }
        }

        private ASTBuilder builder;
        private Grammar<Symbol> grammar;
        private DummyFragment dummyFrag;
        private IProduction<Symbol> dummyProd;
        private UniversalSymbol[] operatorSymbol;
        private int operatorExpressionGoesTo = 17;

        [TestFixtureSetUp]
        public void Init()
        {
            TestsTraceListener.Setup();

            builder = new ASTBuilder();
            grammar = new Grammar<Symbol>(
                           NicodemGrammarProductions.StartSymbol(),
                           NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());
            dummyFrag = new DummyFragment("dummy");
            dummyProd = null;
            operatorSymbol = new UniversalSymbol[] {
                P.Operator0Expression,
                P.Operator1Expression,
                P.Operator2Expression,
                P.Operator3Expression,
                P.Operator4Expression,
                P.Operator5Expression,
                P.Operator6Expression,
                P.Operator7Expression,
                P.Operator8Expression,
                P.Operator9Expression,
                P.Operator10Expression,
                P.Operator11Expression,
                P.Operator12Expression,
                P.Operator13Expression,
                P.Operator14Expression,
                P.Operator15Expression,
                P.Operator16Expression,
                P.Operator17Expression,
                P.Operator18Expression,
                P.Operator19Expression
            };
        }

        [Test()]
        public void GetNameTest()
        {
            Assert.AreEqual(ASTBuilder.GetName(new TestSymbol(false, "Name-Operator-left")), "Name");
            Assert.AreEqual(ASTBuilder.GetName(new TestSymbol(false, "-Desc")), "");
        }

        [Test()]
        public void GetInformation()
        {
            Assert.AreEqual(ASTBuilder.GetInformation(new TestSymbol(false, "Name-Operator-left")), "Operator-left");
            Assert.AreEqual(ASTBuilder.GetInformation(new TestSymbol(false, "-Desc")), "Desc");
        }

        [Test()]
        public void IsLeftOperatorTest()
        {
            Assert.IsTrue(ASTBuilder.IsLeftOperator(new TestSymbol(false, "Name-Operator-left")));
            Assert.IsFalse(ASTBuilder.IsLeftOperator(new TestSymbol(false, "Name-Desc")));
            Assert.IsFalse(ASTBuilder.IsLeftOperator(new TestSymbol(false, "Name-Operator-right")));
        }

        private Symbol Cast(object symbol)
        {
            return (Symbol)(NicodemGrammarProductions.UniversalSymbol)symbol;
        }

        private ParseTree Wrap(UniversalSymbol symbol, ParseTree wrapped)
        {
            return new ParseBranch<Symbol>(dummyFrag, Cast(symbol), dummyProd, new ParseTree[] { wrapped });
        }

        private ParseTree Wrap(UniversalSymbol symbol, ParseTree[] wrapped)
        {
            return new ParseBranch<Symbol>(dummyFrag, Cast(symbol), dummyProd, wrapped);
        }

        private ParseTree Wrap(UniversalSymbol symbol, string code, ParseTree wrapped)
        {
            return new ParseBranch<Symbol>(new DummyFragment(code), Cast(symbol), dummyProd, new ParseTree[] { wrapped });
        }

        private ParseTree Leaf(UniversalSymbol symbol, string code)
        {
            return new ParseLeaf<Symbol>(new DummyFragment(code), Cast(symbol));
        }

        private ParseTree Str(string code)
        {
            return Leaf((UniversalSymbol)code, code);
        }

        private ParseTree Operators(int last, int first, ParseTree wrapped)
        {
            ParseTree result = wrapped;
            for (int i = first; i <= last; i++) result = Wrap(operatorSymbol[i], result);
            return result;
        }

        private ParseTree Operators(int last, int first, ParseTree[] wrapped)
        {
            ParseTree result = Wrap(operatorSymbol[first++], wrapped);
            for (int i = first; i <= last; i++) result = Wrap(operatorSymbol[i], result);
            return result;
        }

        private ParseTree NumberAtomicExpression(int number=42)
        {
            return
                Wrap(P.AtomicExpression,
                Wrap(P.ObjectUseExpression,
                Wrap(P.Literals,
                Wrap(P.DecimalNumberLiteral, "DecimalNumberLiteral",
                Leaf(P.DecimalNumberLiteralToken, number.ToString())))));
        }

        private ParseTree NumberExpression(int number=42)
        {
            return Wrap(P.Expression, 
                Wrap(P.OperatorExpression,
                Operators(operatorExpressionGoesTo, 0, 
                NumberAtomicExpression(number))));
        }

        private ParseTree MulExpression()
        {
            return
                Wrap(P.Expression,
                Wrap(P.OperatorExpression,
                Operators(operatorExpressionGoesTo, 5, new ParseTree[] { 
                    Operators(4, 0, NumberAtomicExpression(42)),
                    Str("*"),
                    Operators(4, 0, NumberAtomicExpression(43))
                })));
        }

        private ParseTree AddMulExpression()
        {
            return 
                Wrap(P.Expression,
                Wrap(P.OperatorExpression,
                Operators(operatorExpressionGoesTo, 6, new ParseTree[] {
                    Operators(5, 0, NumberAtomicExpression(1)),
                    Str("+"),
                    Wrap(P.Operator5Expression, new ParseTree[] {
                        Operators(4, 0, NumberAtomicExpression(2)),
                        Str("*"),
                        Operators(4, 0, NumberAtomicExpression(3))
                    })
                })));
        }

        private ParseTree ExpressionFunction(ParseTree expressionTree)
        {
            return Wrap(P.Function, new ParseTree[] {
                Leaf(P.ObjectName, "MyFunc"),
                Str("("),
                Wrap(P.ParametersList, new ParseTree[] {}),
                Str(")"),
                Str("->"),
                Wrap(P.TypeSpecifier, Leaf(P.TypeName, "int")),
                expressionTree
            });
        }

        private ParseTree FunctionProgram(ParseTree functionTree)
        {
            return Wrap(P.Program, new ParseTree[] { 
                functionTree,
                Leaf(P.Eof, "")
            });
        }

        private ParseTree NumberFunction()
        {
            return ExpressionFunction(NumberExpression());
        }

        private ParseTree ExpressionProgram(ParseTree expressionTree)
        {
            return FunctionProgram(ExpressionFunction(expressionTree));
        }

        private void ConductTest(ParseTree tree, Node expected)
        {
            Console.Write("\ntree:\n" + tree.ToString());
            Console.Write("\nexpected:\n" + expected.ToString());
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            Console.Write("\nresult:\n" + result.ToString() + "\n");
            Assert.AreEqual(expected, result);
        }

        [Test()]
        public void EmptyProgramTest()
        {
            var tree = Wrap(P.Program, Leaf(P.Eof, ""));
            var expected = new ProgramNode(new LinkedList<FunctionDefinitionNode>());
            ConductTest(tree, expected);
        }

        [Test()]
        public void ProgramWithTheSimplestFunction()
        {
            var tree = ExpressionProgram(NumberExpression());
            var expected = new ProgramNode(new LinkedList<FunctionDefinitionNode>(new FunctionDefinitionNode[]{
                new FunctionDefinitionNode(
                    "MyFunc",
                    new LinkedList<VariableDeclNode>(),
                    NamedTypeNode.IntType(true),
                    new AtomNode(NamedTypeNode.IntType(), "42")
                )
            }));
            ConductTest(tree, expected);
        }

        [Test()]
        public void ProgramWithTwoFunctions()
        { 
            var tree = Wrap(P.Program, new ParseTree[] { 
                NumberFunction(),
                NumberFunction(),
                Leaf(P.Eof, "")
            });
            var expected = new ProgramNode(new LinkedList<FunctionDefinitionNode>(new FunctionDefinitionNode[]{
                new FunctionDefinitionNode(
                    "MyFunc",
                    new LinkedList<VariableDeclNode>(),
                    NamedTypeNode.IntType(true),
                    new AtomNode(NamedTypeNode.IntType(), "42")
                ),
                new FunctionDefinitionNode(
                    "MyFunc",
                    new LinkedList<VariableDeclNode>(),
                    NamedTypeNode.IntType(true),
                    new AtomNode(NamedTypeNode.IntType(), "42")
                )
            }));
            ConductTest(tree, expected);
        }

        [Test()]
        public void ProgramWithMultiplication()
        {
            var tree = ExpressionProgram(MulExpression());
            var expected = new ProgramNode(new LinkedList<FunctionDefinitionNode>(new FunctionDefinitionNode[]{
                new FunctionDefinitionNode(
                    "MyFunc",
                    new LinkedList<VariableDeclNode>(),
                    NamedTypeNode.IntType(true),
                    new OperatorNode(
                        OperatorType.MUL,
                        new ExpressionNode[] {
                            new AtomNode(NamedTypeNode.IntType(), "42"),
                            new AtomNode(NamedTypeNode.IntType(), "43"),
                        }
                    )
                )
            }));
            ConductTest(tree, expected);
        }

        [Test()]
        public void ProgramWithAdditionAndMultiplication()
        {
            var tree = ExpressionProgram(AddMulExpression());
            var expected = new ProgramNode(new LinkedList<FunctionDefinitionNode>(new FunctionDefinitionNode[]{
                new FunctionDefinitionNode(
                    "MyFunc",
                    new LinkedList<VariableDeclNode>(),
                    NamedTypeNode.IntType(true),
                    new OperatorNode(
                        OperatorType.PLUS,
                        new ExpressionNode[] {
                            new AtomNode(NamedTypeNode.IntType(), "1"),
                            new OperatorNode(
                                OperatorType.MUL,
                                new ExpressionNode[] {
                                    new AtomNode(NamedTypeNode.IntType(), "2"),
                                    new AtomNode(NamedTypeNode.IntType(), "3"),
                                }
                            )
                        }
                    )                    
                )
            }));
            ConductTest(tree, expected);
        }
    }
}

