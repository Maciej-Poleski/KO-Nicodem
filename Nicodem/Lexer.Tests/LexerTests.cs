using System.Linq;
using Nicodem.Lexer;
using Nicodem.Source.Tmp;
using NUnit.Framework;

namespace Lexer.Tests
{
    [TestFixture]
    internal class LexerTests
    {
        [Test]
        private void LexerEmpty()
        {
            foreach (var arr in new[] {new RegEx<char>[0], new[] {RegExFactory.Empty<char>()}  })
            {
                var lexer = new Nicodem.Lexer.Lexer(arr);
                Assert.IsEmpty(lexer.Process(new StringOrigin("")));
                Assert.IsEmpty(lexer.Process(new StringOrigin(" ")));
                Assert.IsEmpty(lexer.Process(new StringOrigin("a")));
                Assert.IsEmpty(lexer.Process(new StringOrigin("asdf")));
                Assert.IsEmpty(lexer.Process(new StringOrigin(" _\n")));
                Assert.IsEmpty(lexer.Process(new StringOrigin("*^& GY?'\t\n\t\tlikjd")));
                Assert.IsEmpty(lexer.Process(new StringOrigin("\\")));
                Assert.IsEmpty(lexer.Process(new StringOrigin("\\n")));
            }
        }

        [Test]
        private void LexerEpsilon()
        {
            var lexer = new Nicodem.Lexer.Lexer(new[] { RegExFactory.Epsilon<char>() });
            var result = lexer.Process(new StringOrigin("")).ToArray();
            Assert.Equals(result.Length, 1);
            Assert.Equals(result[0].Item2.ToArray(), new[] {0});
            // IFragment.GetString missing?
            Assert.IsEmpty(lexer.Process(new StringOrigin(" ")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("a")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("asdf")));
            Assert.IsEmpty(lexer.Process(new StringOrigin(" _\n")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("*^& GY?'\t\n\t\tlikjd")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("\\")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("\\n")));
        }

        private static RegEx<char> MakeCharRangeRegex(char a, char b)
        {
            return RegExFactory.Intersection(RegExFactory.Range(a), RegExFactory.Complement(RegExFactory.Range((char) (b + 1))));
        }
    }
}