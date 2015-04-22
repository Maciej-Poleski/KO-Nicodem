using Nicodem.Semantics.Grammar;
using NUnit.Framework;

namespace Semantics.Tests.Grammar
{
    public class SymbolTests
    {
        [Test]
        public void MinValueIsLessThanEverything()
        {
            var symbol1 = new Symbol(0);
            var symbol2 = new Symbol(1);
            var symbol3 = new Symbol(2);
            var symbol4 = new Symbol(3);
            var symbol5 = new Symbol(12334);
            var symbol6 = new Symbol(int.MaxValue - 1);
            var symbol7 = new Symbol(int.MaxValue);

            Assert.Less(Symbol.MinValue.CompareTo(Symbol.EOF), 0);
            Assert.True(Symbol.MinValue < Symbol.EOF);
            Assert.Less(Symbol.MinValue.CompareTo(symbol1), 0);
            Assert.True(Symbol.MinValue < symbol1);
            Assert.Less(Symbol.MinValue.CompareTo(symbol2), 0);
            Assert.True(Symbol.MinValue < symbol2);
            Assert.Less(Symbol.MinValue.CompareTo(symbol3), 0);
            Assert.True(Symbol.MinValue < symbol3);
            Assert.Less(Symbol.MinValue.CompareTo(symbol4), 0);
            Assert.True(Symbol.MinValue < symbol4);
            Assert.Less(Symbol.MinValue.CompareTo(symbol5), 0);
            Assert.True(Symbol.MinValue < symbol5);
            Assert.Less(Symbol.MinValue.CompareTo(symbol6), 0);
            Assert.True(Symbol.MinValue < symbol6);
            Assert.Less(Symbol.MinValue.CompareTo(symbol7), 0);
            Assert.True(Symbol.MinValue < symbol7);
        }
    }
}