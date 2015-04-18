using System.Linq;
using Nicodem.Parser;
using Nicodem.Semantics.Grammar;
using NUnit.Framework;

namespace Semantics.Tests.Grammar
{
    [TestFixture]
    internal class NicodemGrammarProductionsTests
    {
        [Test]
        public void GrammarConstructor()
        {
            new Grammar<Symbol>(
                NicodemGrammarProductions.StartSymbol(),
                NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());
        }

        [Test]
        public void WhiteSpacesAreCategories()
        {
            Assert.True(
                NicodemGrammarProductions.TokenCategories.Intersect(NicodemGrammarProductions.WhiteSpaceTokenCategories)
                    .SequenceEqual(NicodemGrammarProductions.WhiteSpaceTokenCategories));
        }
    }
}