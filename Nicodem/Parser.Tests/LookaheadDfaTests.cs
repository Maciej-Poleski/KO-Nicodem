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
             * 
             * Considered production 0 --A--> 1
             * 
             * a,b,c,x - nonterminals
             * 
             * Productions in Grammatic:
             * A: 2 ---B--> 3 | 2 ---C--> 4
             * B: 5 --x--> 5 | 5 --b--> 6(acc) | 4 --D--> 6(acc)
             * C: 7 --x--> 7 | 7 --c--> 8(acc)
             * D: 9 --a--> 10(acc)
             * 
             * Correct automaton: 
             * 
             * s1: --x--> s1 | --a--> s2 | s1 --b--> s3 |--c--> s4
             * s2: acc state 'B'
             * s3: acc state 'B'
             * s4: acc state 'C'
             */

            /* More Complex Dfa Test
             * 
             * Considered production 0 --A--> 1 --b--> 15
             *
             * a,b,c - nonterminals
             * 
             * Productions in Grammatic:
             * A: 2 ---B--> 3 | 2 ---C--> 4 | 2 --D--> 5
             * B: 6 --b-->7(acc) | 6 --a-->8
             * C: 9 --a--> 9 | 9 --c--> 10 (acc)
             * D: 11 --a-->12 | 11 --E--> 13
             * E: 14 --a--> 14 | 14 --b--> 14
             * 
             * Correct automaton: 
             * 
             * s1: --a--> s2 | --b--> s6 | --c--> s4
             * s2: --a--> s3 | --b--> s7 | --c--> s4
             * s3: --a--> s3 | --b--> s5 | --c--> s4
             * s4: acc state 'C'
             * s5: acc state 'D'
             * s6: --a--> s8 | --b--> s7 | --c--> null?
             * s7: --a--> s8 | --b--> s5 | --c--> null?
             * s8: acc state 'D'
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
