using System.Linq;
using Nicodem.Parser;
using Nicodem.Semantics.Grammar;
using NUnit.Framework;

namespace Semantics.Tests.Grammar
{
    [TestFixture]
    internal class NicodemGrammarProductionsTests
    {
        private static Grammar<Symbol> MakeGrammar()
        {
            return new Grammar<Symbol>(
                NicodemGrammarProductions.StartSymbol(),
                NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());
        }

        [Test]
        public void GrammarConstructor()
        {
            MakeGrammar();
        }

        [Test]
        public void ParserConstructor()
        {
            new LLStarParser<Symbol>(MakeGrammar());
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