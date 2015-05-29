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

        [TestFixtureSetUp]
        public void Init()
        {
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
                P.Operator17Expression
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

        private ParseTree NumberAtomicExpression()
        {
            return
                Wrap(P.AtomicExpression,
                Wrap(P.ObjectUseExpression,
                Leaf(P.Literals, "42")));
        }

        private ParseTree NumberExpression()
        {
            return Wrap(P.Expression, 
                Wrap(P.OperatorExpression, 
                Operators(17, 0, 
                NumberAtomicExpression())));
        }

        private ParseTree MulExpression()
        {
            return
                Wrap(P.Expression,
                Wrap(P.OperatorExpression,
                Operators(17, 5, new ParseTree[] { 
                    Operators(4, 0, NumberAtomicExpression()),
                    Str("*"),
                    Operators(4, 0, NumberAtomicExpression())
                })));
        }

        private ParseTree AddMulExpression()
        {
            return 
                Wrap(P.Expression,
                Wrap(P.OperatorExpression,
                Operators(17, 6, new ParseTree[] {  
                    NumberAtomicExpression(),
                    Str("+"),
                    Wrap(P.Operator5Expression, new ParseTree[] {
                        Operators(4, 0, NumberAtomicExpression()),
                        Str("*"),
                        Operators(4, 0, NumberAtomicExpression())
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
            return Wrap(P.Program, new ParseTree[] { functionTree });
        }

        private ParseTree NumberFunction()
        {
            return ExpressionFunction(NumberExpression());
        }

        private ParseTree ExpressionProgram(ParseTree expressionTree)
        {
            return FunctionProgram(ExpressionFunction(expressionTree));
        }

        [Test()]
        public void EmptyProgramTest()
        {
            var tree = Wrap(P.Program, Leaf(P.Eof, ""));
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            // TODO(guspiel): When it becomes clear what AST to expect, write an AssertEquals here.
        }

        [Test()]
        public void ProgramWithTheSimplestFunction()
        { 
            var tree = ExpressionProgram(NumberExpression());
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            // TODO(guspiel): When it becomes clear what AST to expect, write an AssertEquals here.
        }

        [Test()]
        public void ProgramWithTwoFunctions()
        { 
            var tree = Wrap(P.Program, new ParseTree[] { 
                NumberFunction(),
                NumberFunction()
            });
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            // TODO(guspiel): When it becomes clear what AST to expect, write an AssertEquals here.
        }

        [Test()]
        public void ProgramWithMultiplication()
        {
            var tree = ExpressionProgram(MulExpression());
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            // TODO(guspiel): When it becomes clear what AST to expect, write an AssertEquals here.
        }

        [Test()]
        public void ProgramWithAdditionAndMultiplication()
        {
            var tree = ExpressionProgram(AddMulExpression());
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            // TODO(guspiel): When it becomes clear what AST to expect, write an AssertEquals here.
        }
    }
}

