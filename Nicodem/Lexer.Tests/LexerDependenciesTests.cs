using Nicodem.Lexer;
using Nicodem.Source;
using NUnit.Framework;
using System;
using System.Linq;

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
            DfaUtils.DfaStatesConcpetCheck<char>.CheckDfaStates(dfa);
        }

        [Test]
        public void MutableKeywordTest()
        {
            var mutableRegex = "mutable";
            var theRestRegex = "(((([^:space:]+)&(^((.*(mutable).*)|(.*(\\.\\.).*)|(.*(else).*))))))";
            var lexer = new Nicodem.Lexer.Lexer(RegExParser.Parse(theRestRegex));
            var so = new StringOrigin("mutable");
            var result = lexer.Process(so);
            Assert.AreEqual(result.LastParsedLocation.GetOriginPosition(), result.FailedAtLocation.GetOriginPosition());
            Assert.AreNotEqual(1, result.Tokens.Count());
        }
    }
}