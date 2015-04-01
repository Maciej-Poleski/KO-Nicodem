using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nicodem.Parser.Tests
{
    [TestFixture]
    class LookaheadDfaTests
    {

        [TestFixtureSetUp]
        public void init()
        {
            /* One Level Dfa Test
             * Considered production 0 --A--> 1
             * a,b,c,x - nonterminals
             * Productions in Grammatic:
             * A: 2 ---B--> 3 | 2 ---C--> 4
             * B: 5 --x--> 5 | 5 --b--> 6(acc) | 4 --D--> 6(acc)
             * C: 7 --x--> 7 | 7 --c--> 8(acc)
             * D: 9 --a--> 10(acc)
             * 
             * Correct automaton: 
             * state | acc value 
             * s1 | 0
             * s2 | 'B'
             * s3 | 'B'
             * s4 | 'C'
             * 
             * s1 ---> s1 
             * s1 ---> s2
             * s1 ---> s3
             * s1 ---> s4
             */

            /* MoreComplexDfaTest
             * 
             */
        }

        [Test]
        public void LookaheadDfaOneLevelTest()
        {

        }

        [Test]
        public void LookaheadDfaMoreComplexTest()
        {

        }
    }
}
