using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Nicodem.Lexer;

namespace Lexer.Tests
{
    [TestFixture]
    class RegExDfaTests
    {
        //Create Dfa from empty RegEx
        [Test]
        public void EmptyRegexTests()
        {
            RegexDfa<char> regEx = new RegexDfa<char>(RegExFactory.Empty<char>(), 1);
        }
    }
}
