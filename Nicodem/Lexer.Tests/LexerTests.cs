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

        private static RegEx<char> MakeCharRegex(char c)
        {
            return MakeCharRangeRegex(c, c);
        }

        private static Tuple<string, IEnumerable<int>> T(string token, params int[] categories)
        {
            return new Tuple<string, IEnumerable<int>>(token, categories);
        }

        [Test]
        public void LexerComplexLanguages()
        {
            // 0: a+
            // 1: a*b
            // 2: a*b+

            var regex0 = RegExFactory.Concat(MakeCharRegex('a'), RegExFactory.Star(MakeCharRegex('a')));
            var regex1 = RegExFactory.Concat(RegExFactory.Star(MakeCharRegex('a')), MakeCharRegex('b'));
            var regex2 = RegExFactory.Concat(RegExFactory.Star(MakeCharRegex('a')),
                RegExFactory.Concat(MakeCharRegex('b'), RegExFactory.Star(MakeCharRegex('b'))));

            var lexex = new Nicodem.Lexer.Lexer(regex0, regex1, regex2);
            Assert.IsTrue(lexex.Process("b").TokenizedTo(T("b", 1, 2)));
            Assert.IsEmpty(lexex.Process(""));
            Assert.IsTrue(lexex.Process("a").TokenizedTo(T("a", 0)));
            Assert.IsTrue(lexex.Process("aaa").TokenizedTo(T("aaa", 0)));
            Assert.IsTrue(lexex.Process("ab").TokenizedTo(T("ab", 1, 2)));
            Assert.IsTrue(lexex.Process("abb").TokenizedTo(T("abb", 2)));
            Assert.IsTrue(lexex.Process("bb").TokenizedTo(T("bb", 2)));
            Assert.IsTrue(lexex.Process("baabab").TokenizedTo(T("b", 1, 2), T("aab", 1, 2), T("ab", 1, 2)));
            Assert.IsTrue(lexex.Process("bbab").TokenizedTo(T("bb", 2), T("ab", 1, 2)));
            Assert.IsTrue(lexex.Process("bbacb").TokenizedTo(T("bb", 2), T("a", 0)));
        }

        [Test]
        public void LexerEmpty()
        {
            foreach (var arr in new[] {new RegEx<char>[0], new[] {RegExFactory.Empty<char>()}})
            {
                var lexer = new Nicodem.Lexer.Lexer(arr);
                Assert.IsEmpty(lexer.Process(""));
                Assert.IsEmpty(lexer.Process(" "));
                Assert.IsEmpty(lexer.Process("a"));
                Assert.IsEmpty(lexer.Process("asdf"));
                Assert.IsEmpty(lexer.Process(" _\n"));
                Assert.IsEmpty(lexer.Process("*^& GY?'\t\n\t\tlikjd"));
                Assert.IsEmpty(lexer.Process("\\"));
                Assert.IsEmpty(lexer.Process("\\n"));
            }
        }

        [Test]
        public void LexerSimpleRanges()
        {
            var lexer =
                new Nicodem.Lexer.Lexer(MakeCharRangeRegex('b', 'd'), MakeCharRangeRegex('d', 'g'),
                    MakeCharRangeRegex('g', 'i'));
            Assert.IsEmpty(lexer.Process("a"));
            Assert.IsEmpty(lexer.Process("ab"));
            Assert.IsEmpty(lexer.Process("j"));
            Assert.IsTrue(lexer.Process("b").TokenizedTo(T("b", 0)));
            Assert.IsTrue(lexer.Process("c").TokenizedTo(T("c", 0)));
            Assert.IsTrue(lexer.Process("d").TokenizedTo(T("d", 0, 1)));
            Assert.IsTrue(lexer.Process("e").TokenizedTo(T("e", 1)));
            Assert.IsTrue(lexer.Process("f").TokenizedTo(T("f", 1)));
            Assert.IsTrue(lexer.Process("g").TokenizedTo(T("g", 1, 2)));
            Assert.IsTrue(lexer.Process("h").TokenizedTo(T("h", 2)));
            Assert.IsTrue(lexer.Process("i").TokenizedTo(T("i", 2)));
            Assert.IsTrue(lexer.Process("bbdgheijb").TokenizedTo(
                T("b", 0),
                T("b", 0),
                T("d", 0, 1),
                T("g", 1, 2),
                T("h", 2),
                T("e", 1),
                T("i", 2)));
        }

        [Test]
        public void LexerSingleRange()
        {
            var lexer = new Nicodem.Lexer.Lexer(MakeCharRangeRegex('b', 'd'));
            Assert.IsEmpty(lexer.Process(""));
            Assert.IsEmpty(lexer.Process(" "));
            Assert.IsEmpty(lexer.Process("a"));
            Assert.IsEmpty(lexer.Process("asdf"));
            Assert.IsEmpty(lexer.Process(" _\n"));
            Assert.IsEmpty(lexer.Process("*^& GY?'\t\n\t\tlikjd"));
            Assert.IsEmpty(lexer.Process("\\"));
            Assert.IsEmpty(lexer.Process("\\n"));
            Assert.IsTrue(lexer.Process("ba").Tokens().SequenceEqual(new[] {"b"}));
            Assert.IsTrue(lexer.Process("bb").Tokens().SequenceEqual(new[] {"b", "b"}));
            Assert.IsEmpty(lexer.Process("ab"));
            Assert.IsTrue(lexer.Process("db").Tokens().SequenceEqual(new[] {"d", "b"}));
            Assert.IsEmpty(lexer.Process("eb"));
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

        internal static bool TokenizedTo(this IEnumerable<Tuple<string, IEnumerable<int>>> tokens,
            params Tuple<string, IEnumerable<int>>[] expectation)
        {
            var i = 0;
            foreach (var token in tokens)
            {
                Assert.Less(i, expectation.Length);
                Assert.AreEqual(token.Item1, expectation[i].Item1);
                var tok1 = token.Item2.ToList();
                tok1.Sort();
                var tok2 = expectation[i].Item2.ToList();
                tok2.Sort();
                Assert.IsTrue(tok1.SequenceEqual(tok2));
                i += 1;
            }
            return i == expectation.Length;
        }
    }
}