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
        private int operatorExpressionGoesTo = 19;

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

        private ParseTree OperatorsExpression(int first, ParseTree wrapped)
        {
            return 
                Wrap(P.Expression,
                    Wrap(P.OperatorExpression,
                    Operators(operatorExpressionGoesTo, first, wrapped)
                ));
        }

        private ParseTree OperatorsExpression(int first, ParseTree[] wrapped)
        {
            return
                Wrap(P.Expression,
                    Wrap(P.OperatorExpression,
                    Operators(operatorExpressionGoesTo, first, wrapped)
                ));
        }

        private ParseTree VariableUseOperators(int last, string name)
        {
            return Operators(last, 0, 
                Wrap(P.AtomicExpression,
                Wrap(P.ObjectUseExpression,
                Leaf(P.ObjectName, name)
            )));
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
                    new AtomNode(NamedTypeNode.IntType(true), "42")
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
                    new AtomNode(NamedTypeNode.IntType(true), "42")
                ),
                new FunctionDefinitionNode(
                    "MyFunc",
                    new LinkedList<VariableDeclNode>(),
                    NamedTypeNode.IntType(true),
                    new AtomNode(NamedTypeNode.IntType(true), "42")
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
                            new AtomNode(NamedTypeNode.IntType(true), "42"),
                            new AtomNode(NamedTypeNode.IntType(true), "43"),
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
                            new AtomNode(NamedTypeNode.IntType(true), "1"),
                            new OperatorNode(
                                OperatorType.MUL,
                                new ExpressionNode[] {
                                    new AtomNode(NamedTypeNode.IntType(true), "2"),
                                    new AtomNode(NamedTypeNode.IntType(true), "3"),
                                }
                            )
                        }
                    )                    
                )
            }));
            ConductTest(tree, expected);
        }

        [Test()]
        public void ProgramWithRecursiveFunctionCalls()
        {
            /*
                int g(int n, int k, bool nothing) 0
                void f(int n) {
	                g(n, n, true);
                    f(n - 1);
                }
             */
            var tree = Wrap(P.Program, new ParseTree[] { 
                Wrap(P.Function, new ParseTree[] {
                    Leaf(P.ObjectName, "g"),
                    Str("("),
                    Wrap(P.ParametersList, new ParseTree[] {
                        Wrap(P.ObjectDeclaration, new ParseTree[] {
                            Wrap(P.TypeSpecifier, new ParseTree[] { Leaf(P.TypeName, "int") }),
                            Leaf(P.ObjectName, "n")
                        }),
                        Str(","),
                        Wrap(P.ObjectDeclaration, new ParseTree[] {
                            Wrap(P.TypeSpecifier, new ParseTree[] { Leaf(P.TypeName, "int") }),
                            Leaf(P.ObjectName, "k")
                        }),
                        Str(","),
                        Wrap(P.ObjectDeclaration, new ParseTree[] {
                            Wrap(P.TypeSpecifier, new ParseTree[] { Leaf(P.TypeName, "bool") }),
                            Leaf(P.ObjectName, "nothing")
                        })
                    }),
                    Str(")"),
                    Str("->"),
                    Wrap(P.TypeSpecifier, Leaf(P.TypeName, "int")),
                    NumberExpression(0)
                }),
                Wrap(P.Function, new ParseTree[] {
                    Leaf(P.ObjectName, "f"),
                    Str("("),
                    Wrap(P.ParametersList, new ParseTree[] {
                        Wrap(P.ObjectDeclaration, new ParseTree[] {
                            Wrap(P.TypeSpecifier, new ParseTree[] { Leaf(P.TypeName, "int") }),
                            Leaf(P.ObjectName, "n")
                        })
                    }),
                    Str(")"),
                    Str("->"),
                    Wrap(P.TypeSpecifier, Leaf(P.TypeName, "void")),
                    OperatorsExpression(0,
                        Wrap(P.AtomicExpression,
                        Wrap(P.BlockExpression, new ParseTree[] {
                            Str("{"),
                                OperatorsExpression(2, new ParseTree[] {
                                    VariableUseOperators(1, "g"),
                                    Str("("),
                                    OperatorsExpression(1, VariableUseOperators(0, "n")),
                                    Str(","),
                                    OperatorsExpression(1, VariableUseOperators(0, "n")),
                                    Str(","),
                                    OperatorsExpression(0,
                                        Wrap(P.AtomicExpression,
                                        Wrap(P.ObjectUseExpression,
                                        Wrap(P.Literals, 
                                        Wrap(P.BooleanLiteral,
                                        Leaf(P.BooleanLiteralToken, "true")
                                    ))))),
                                    Str(")")
                                }),
                                Str(";"),
                                OperatorsExpression(2, new ParseTree[] {
                                    VariableUseOperators(1, "f"),
                                    Str("("),
                                    OperatorsExpression(6, new ParseTree[] {
                                        VariableUseOperators(5, "n"),
                                        Str("-"),
                                        Operators(5, 0, NumberAtomicExpression(1))
                                    }),
                                    Str(")")
                                }),
                                Str(";"),
                            Str("}")
                        })
                    ))
                }),
                Leaf(P.Eof, "")
            });
            var expected = new ProgramNode(new LinkedList<FunctionDefinitionNode>(new FunctionDefinitionNode[]{
                new FunctionDefinitionNode(
                    "g",
                    new LinkedList<VariableDeclNode>(new VariableDeclNode[]{
                        new VariableDeclNode("n", NamedTypeNode.IntType(true), false),
                        new VariableDeclNode("k", NamedTypeNode.IntType(true), false),
                        new VariableDeclNode("nothing", NamedTypeNode.BoolType(true), false),
                    }),
                    NamedTypeNode.IntType(true),
                    new AtomNode(NamedTypeNode.IntType(true), "0")                   
                ),
                new FunctionDefinitionNode(
                    "f",
                    new LinkedList<VariableDeclNode>(new VariableDeclNode[]{
                        new VariableDeclNode("n", NamedTypeNode.IntType(true), false),
                    }),
                    NamedTypeNode.VoidType(true),
                    new BlockExpressionNode(
                        new ExpressionNode[] {
                            new FunctionCallNode(
                                "g", 
                                new ExpressionNode[] {
                                    new VariableUseNode("n", null),
                                    new VariableUseNode("n", null),
                                    new AtomNode(NamedTypeNode.BoolType(true), "true")
                                },
                                null
                            ),
                            new FunctionCallNode(
                                "f", 
                                new ExpressionNode[] {
                                    new OperatorNode(
                                        OperatorType.MINUS,
                                        new ExpressionNode[] {
                                            new VariableUseNode("n", null),
                                            new AtomNode(NamedTypeNode.IntType(true), "1")
                                        }
                                    )
                                },
                                null
                            )
                        }
                    )                 
                )
            }));
            ConductTest(tree, expected);
        }
    }
}

