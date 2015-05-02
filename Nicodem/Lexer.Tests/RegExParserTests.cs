using Nicodem.Lexer;
using NUnit.Framework;
using System;

namespace Lexer.Tests
{

    [TestFixture]
    public class RegExParserTests
    {
        [Test]
        public void TestConcats()
        {
            var result = RegExParser.Parse("abc");
            var regEx = RegExFactory.Concat(RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'), RegExFactory.Range('c', 'd'));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestClasses()
        {
            var result1 = RegExParser.Parse("[:digit:]");
            var regEx1 = CharactersClasses.digit;
            Assert.AreEqual(result1, RegExFactory.Intersection(regEx1, RegExFactory.Star(CharactersClasses.print)));

            var result2 = RegExParser.Parse("[:space:]");
            var regEx2 = CharactersClasses.space;
            Assert.AreEqual(result2, RegExFactory.Intersection(regEx2, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestPlus()
        {
            var result = RegExParser.Parse("a*+*+*");
            var regEx = RegExFactory.Concat(RegExFactory.Range('a', 'b'), RegExFactory.Star(RegExFactory.Range('a', 'b')));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestNotAllowedCharacters()
        {
            var result = RegExParser.Parse("[^ab]");
            var regEx = RegExFactory.Intersection (RegExFactory.Range ((char)0), RegExFactory.Complement (RegExFactory.Union(RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'))));

            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestIntersection()
        {
            var result = RegExParser.Parse("a&b&c");
			var regEx = RegExFactory.Intersection(RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'), RegExFactory.Range('c', 'd'));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestStars()
        {
            var result = RegExParser.Parse("(a******b********)***");
            var regEx = RegExFactory.Star(RegExFactory.Concat(RegExFactory.Star(RegExFactory.Range('a', 'b')), RegExFactory.Star(RegExFactory.Range('b', 'c'))));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestParentheses()
        {
            var result = RegExParser.Parse("((a(b))((a)((b))))");
            var regEx = RegExFactory.Concat(RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'), RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestComplements()
        {
            var result = RegExParser.Parse("^(^a^b)");
            var regEx = RegExFactory.Complement(RegExFactory.Concat(RegExFactory.Complement(RegExFactory.Range('a', 'b')), RegExFactory.Complement(RegExFactory.Range('b', 'c'))));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestAllAtOnce()
        {
            var result = RegExParser.Parse("(^(a|(b|c)*))[^:space:]");
            var regEx = RegExFactory.Concat(RegExFactory.Complement(RegExFactory.Union(RegExFactory.Range('a', 'b'), RegExFactory.Star(RegExFactory.Union(RegExFactory.Range('b', 'c'), RegExFactory.Range('c', 'd'))))),  RegExFactory.Intersection (RegExFactory.Range ((char)0), RegExFactory.Complement (CharactersClasses.space)));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));
        }

        [Test]
        public void TestNotEscaped()
        {
            var result = RegExParser.Parse("(\\(\\+\\))*");
            var regEx = RegExFactory.Star(RegExFactory.Concat(RegExFactory.Range('(', (char)('(' + 1)), RegExFactory.Range('+', (char)('+' + 1)), RegExFactory.Range(')', (char)(')' + 1))));
            Assert.AreEqual(result, RegExFactory.Intersection(regEx, RegExFactory.Star(CharactersClasses.print)));            
        }

        [Test]
        public void TestNotBalancedParenteses()
        {
            try {
                RegExParser.Parse("(((a))");
                Assert.Fail();
            } catch (ParseError) {
            }
        }

        [Test]
        public void TestSyntaxError()
        {
            try {
                RegExParser.Parse("((a)*)|*");
                Assert.Fail();
            } catch (ParseError) {
            }
        }

    }
}
