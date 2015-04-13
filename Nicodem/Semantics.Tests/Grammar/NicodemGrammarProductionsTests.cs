using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.Grammar;
using NUnit.Framework;

namespace Semantics.Tests.Grammar
{
    [TestFixture]
    class NicodemGrammarProductionsTests
    {
        [Test]
        public void GrammarConstructor()
        {
            new Nicodem.Parser.Grammar<Symbol>(
                NicodemGrammarProductions.StartSymbol(),
                NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());
        }
    }
}
