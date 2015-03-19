using Nicodem.Lexer;
using NUnit.Framework;
using System;

namespace Lexer.Tests
{

    [TestFixture]
    public class RegExParserTests
    {
        [Test]
        public void Test_Concats()
        {
            var result = RegExParser.Parse("abc");
            var regEx = RegExFactory.Concat(RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'), RegExFactory.Range('c', 'd'));
            Assert.AreEqual(result, regEx);
        }

        [Test]
        public void Test_Stars()
        {
            var result = RegExParser.Parse("\\(a\\*\\*\\*\\*\\*\\*b\\*\\*\\*\\*\\*\\*\\*\\*\\)\\*\\*\\*");
            var regEx = RegExFactory.Star(RegExFactory.Concat(RegExFactory.Star(RegExFactory.Range('a', 'b')), RegExFactory.Star(RegExFactory.Range('b','c'))));
            Assert.AreEqual (result, regEx);     
        }

        [Test]
        public void Test_Parentheses()
        {
            var result = RegExParser.Parse("\\(\\(\\(\\(a\\)b\\)\\(a\\(b\\)\\)\\)\\)");
            var regEx = RegExFactory.Concat(RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'), RegExFactory.Range('a', 'b'), RegExFactory.Range('b', 'c'));
            Assert.AreEqual (result, regEx);
        }

        [Test]
        public void Test_Complements()
        {
            var result = RegExParser.Parse("\\^\\(\\^a\\^b\\)");
            var regEx = RegExFactory.Complement(RegExFactory.Concat(RegExFactory.Complement(RegExFactory.Range('a', 'b')), RegExFactory.Complement(RegExFactory.Range('b', 'c'))));
            Assert.AreEqual (result, regEx);
        }

        [Test]
        public void Test_All_At_Once()
        {
            var result = RegExParser.Parse("\\(\\^\\(a\\|\\(b\\|c\\)\\*\\)\\)\\[a-f\\]");
            var regEx = RegExFactory.Concat(RegExFactory.Complement(RegExFactory.Union(RegExFactory.Range('a', 'b'), RegExFactory.Star(RegExFactory.Union(RegExFactory.Range('b', 'c'), RegExFactory.Range('c', 'd'))))), RegExFactory.Range('a','g'));
            Assert.AreEqual (result, regEx);
        }

        [Test]
        public void Test_Not_Balanced_Parenteses()
        {
            try {
                RegExParser.Parse("\\(\\(\\(a\\)\\)");
                Assert.Fail();
            } catch (ParseError)
            {
            }
        }
            
        [Test]
        public void Test_Syntax_Error()
        {
            try {
                RegExParser.Parse("\\(\\(a\\)\\*\\)\\|\\*");
                Assert.Fail();
            } catch (ParseError)
            {
            }
        }

    }
}

