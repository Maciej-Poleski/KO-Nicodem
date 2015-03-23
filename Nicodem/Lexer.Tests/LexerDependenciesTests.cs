using Nicodem.Lexer;
using NUnit.Framework;

namespace Lexer.Tests
{
    [TestFixture]
    internal class LexerDependenciesTests
    {
        [Test]
        public void DfaConstructionFromRegex()
        {
            var regex = RegExFactory.Empty<char>();
            var dfa = new RegExDfa<char>(regex, 1);
            DfaUtils.DfaStatesNotNullConcpetCheck<char>.CheckDfaStatesNotNull(dfa);
        }
    }
}