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
             * A, B, C, D - terminals
             * 
             * Productions in Grammatic:
             * A: 2 ---B--> 3 | 2 ---C--> 4
             * B: 5 --x--> 5 | 5 --b--> 6(acc) | 4 --D--> 6(acc)
             * C: 7 --x--> 7 | 7 --c--> 8(acc)
             * D: 9 --a--> 10(acc)
             * 
             * Correct automaton: 
             * state | acc value (set)
             * s1 | {'B','C'}
             * s2 | {'B'}
             * s3 | {'B'}
             * s4 | {'C'}
             * 
             * s1: --x--> s1 | --a--> s2 | s1 --b--> s3 |--c--> s4
             * s2: acc state
             * s3: acc state
             * s4: acc state
             */

            /* More Complex Dfa Test
             * 
             * Considered production 0 --A--> 1 --b--> 15
             *
             * a,b,c - nonterminals
             * A, B, C, D, E - terminals
             * 
             * Grammatic:
             * A: B | C
             * B: a | b
             * C: a*c
             * D: a | E
             * E: (a*b*)*
             * 
             * Productions in Grammatic:
             * A: 2 ---B--> 3 | 2 ---C--> 4 | 2 --D--> 5
             * B: 6 --b-->7(acc) | 6 --a-->8(acc)
             * C: 9 --a--> 9 | 9 --c--> 10 (acc)
             * D: 11 --a-->12(acc) | 11 --E--> 13
             * E: 14 --a--> 14(acc) | 14 --b--> 14(acc)
             * 
             * Correct automaton: 
             * state | acc value 
             * s1 | {'B', 'C', 'D'}
             * s2 | {'B', 'C', 'D'}
             * s3 | {'C', 'D'}
             * s4 | {'C'}
             * s5 | {'D'}
             * s6 | {'B', 'D'}
             * s7 | {'B', 'D'}
             * s8 | {'D'}
             * s9 | {} - empty
             * 
             * s1: --a--> s2 | --b--> s6 | --c--> s4
             * s2: --a--> s3 | --b--> s7 | --c--> s4
             * s3: --a--> s3 | --b--> s5 | --c--> s4
             * s4: acc state
             * s5: acc state
             * s6: --a--> s8 | --b--> s7 | --c--> s9
             * s7: --a--> s8 | --b--> s5 | --c--> s9
             * s8: acc state
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
