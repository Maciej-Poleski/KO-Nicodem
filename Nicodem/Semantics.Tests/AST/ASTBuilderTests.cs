using NUnit.Framework;
using System;
using Nicodem.Parser;
using Nicodem.Semantics.AST;

namespace Semantics.Tests
{
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
    }
}

