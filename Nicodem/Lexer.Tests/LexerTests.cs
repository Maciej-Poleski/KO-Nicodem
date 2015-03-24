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
        private static RegEx<char> MakeCharRangeRegex(char a, char b)
        {
            return RegExFactory.Intersection(RegExFactory.Range(a),
                RegExFactory.Complement(RegExFactory.Range((char) (b + 1))));
        }

        [Test]
        public void LexerEmpty()
        {
            foreach (var arr in new[] {new RegEx<char>[0], new[] {RegExFactory.Empty<char>()}})
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
            var lexer = new Nicodem.Lexer.Lexer(new[] {MakeCharRangeRegex('b', 'd')});
            Assert.IsEmpty(lexer.Process(new StringOrigin("")));
            Assert.IsEmpty(lexer.Process(new StringOrigin(" ")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("a")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("asdf")));
            Assert.IsEmpty(lexer.Process(new StringOrigin(" _\n")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("*^& GY?'\t\n\t\tlikjd")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("\\")));
            Assert.IsEmpty(lexer.Process(new StringOrigin("\\n")));
            Assert.IsTrue(lexer.Process("ba").Tokens().SequenceEqual(new[] {"b"}));
            Assert.IsTrue(lexer.Process("bb").Tokens().SequenceEqual(new[] {"b", "b"}));
            Assert.IsEmpty(lexer.Process("ab"));
            Assert.IsTrue(lexer.Process("db").Tokens().SequenceEqual(new [] {"d","b"}));
        }
    }

    internal static class Extensions
    {
        internal static IEnumerable<Tuple<string, IEnumerable<int>>> Process(this Nicodem.Lexer.Lexer lexer, string s)
        {
            var origin = new StringOrigin(s);
            return lexer
                .Process(origin)
                .Select(t => new Tuple<string, IEnumerable<int>>(origin.GetText(t.Item1), t.Item2));
        }

        internal static IEnumerable<string> Tokens(this IEnumerable<Tuple<string, IEnumerable<int>>> tokens)
        {
            return tokens.Select(t => t.Item1);
        }
    }
}