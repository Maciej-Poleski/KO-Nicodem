using NUnit.Framework;
using System;
using Nicodem.Parser;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Grammar;
using Nicodem.Source;

namespace Semantics.Tests
{
    using ParseTree = IParseTree<Symbol>;

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
        private Production dummyProd;
        private Symbol eofSymbol;
        private Symbol programSymbol;
        private Symbol functionSymbol;

        [TestFixtureSetUp]
        public void Init()
        {
            builder = new ASTBuilder();
            grammar = new Grammar<Symbol>(
                           NicodemGrammarProductions.StartSymbol(),
                           NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());
            dummyFrag = new DummyFragment("dummy");
            dummyProd = null;
            eofSymbol = (Symbol)(NicodemGrammarProductions.UniversalSymbol)NicodemGrammarProductions.Eof;
            programSymbol = (Symbol)(NicodemGrammarProductions.UniversalSymbol)NicodemGrammarProductions.Program;
            functionSymbol = (Symbol)(NicodemGrammarProductions.UniversalSymbol)NicodemGrammarProductions.Function;
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

        [Test()]
        public void EmptyProgramTest()
        {
            var tree = new ParseBranch<Symbol>(dummyFrag, programSymbol, dummyProd, new ParseTree[] {
                new ParseLeaf<Symbol>(new DummyFragment(""), eofSymbol)
            });
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            // TODO(guspiel): When it becomes clear what AST to expect, write an AssertEquals here.
        }

        [Test()]
        public void ProgramWithTwoEmptyFunctionsTest()
        {
            var tree = new ParseBranch<Symbol>(dummyFrag, programSymbol, dummyProd, new ParseTree[] {
                new ParseBranch<Symbol>(dummyFrag, functionSymbol, dummyProd, new ParseTree[] {
                    // TODO(guspiel): invalid without a body?
                }),
                new ParseBranch<Symbol>(dummyFrag, functionSymbol, dummyProd, new ParseTree[] {
                    // TODO(guspiel): invalid without a body?
                })
            });
            ProgramNode result = builder.BuildAST<Symbol>(tree);
            // TODO(guspiel): When it becomes clear what AST to expect, write an AssertEquals here.
        }
    }
}

