using System;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Lexer;
using Nicodem.Source;
using NUnit.Framework;

namespace Lexer.Tests
{
    [TestFixture]
    internal class LexerTests
    {
        [Test]
        public void LexerEmpty()
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
        public void LexerSingleRange()
        {
            var lexer = new Nicodem.Lexer.Lexer(new[] { MakeCharRangeRegex('b','d')});
            Assert.IsEmpty(lexer.Process(new StringOrigin("")));
            Assert.IsEmpty(lexer.Process(new StringOrigin(" ")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("a")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("asdf")));
            Assert.IsEmpty(lexer.Process(new StringOrigin(" _\n")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("*^& GY?'\t\n\t\tlikjd")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("\\")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("\\n")));
            Assert.IsEmpty(Process(lexer, "ba"));
            Assert.IsEmpty(Process(lexer, "bb"));
            Assert.IsEmpty(Process(lexer, "ab"));
            Assert.IsEmpty(Process(lexer, "db"));
        }

        private static IEnumerable<Tuple<IFragment, IEnumerable<int>>> Process(Nicodem.Lexer.Lexer lexer, string s)
        {
            return lexer.Process(new StringOrigin(s));
        }

        private static RegEx<char> MakeCharRangeRegex(char a, char b)
        {
            return RegExFactory.Intersection(RegExFactory.Range(a), RegExFactory.Complement(RegExFactory.Range((char) (b + 1))));
        }
    }
}