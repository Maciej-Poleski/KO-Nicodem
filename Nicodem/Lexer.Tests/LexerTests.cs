using Nicodem.Lexer;
using NUnit.Framework;

namespace Lexer.Tests
{
    [TestFixture]
    internal class LexerTests
    {
        [Test]
        private void LexerEmpty()
        {
            var lexer = new Nicodem.Lexer.Lexer(new RegEx[0]);
        }
    }
}