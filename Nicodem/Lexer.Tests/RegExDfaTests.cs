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
        KeyValuePair<char, DFAState<char>> deadTransition;
        DFAState<char> deadState;

        [SetUp]
        public void init()
        {
            deadState = new DFAState<char>();
            deadState._accepting = 0;
            deadState._transitions = new KeyValuePair<char, DFAState<char>>[] { new KeyValuePair<char, DFAState<char>>('\0', deadState) };
            deadTransition = new KeyValuePair<char, DFAState<char>>('\0', deadState);
        }

        [TearDown]
        public void Cleanup() { }

        public void CheckNullTransitionTests(RegExDfa<char> regExDfa)
        {
            var queue = new Queue<DFAState<char>>();
            var visited = new Dictionary<DFAState<char>, int>();

            DFAState<char> start = regExDfa.Start;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var state = queue.Dequeue();
                if (visited.ContainsKey(state))
                    continue;
                visited[state] = 1;

                Assert.IsNotNull(state.Transitions);

                Assert.IsTrue(state.Transitions.Length > 0);

                foreach (var transition in state.Transitions)
                {
                    Assert.IsNotNull(transition.Value);
                    if(!visited.ContainsKey(transition.Value))
                        queue.Enqueue(transition.Value);
                }

            }

        }

        [Test]
        public void RegExDfaEmptyTest()
        {
            RegEx<char> regEx = RegExFactory.Empty<char>();
            RegExDfa<char> regExDfa = new RegExDfa<char>(regEx, 1);
            CheckNullTransitionTests(regExDfa);
            var dfaEmpty = new RegExDfa<char>(new DFAState<char>(0, new KeyValuePair<char, DFAState<char>>[]{ deadTransition }));
            Assert.IsTrue(DfaUtils.CompareDfa<RegExDfa<char>, DFAState<char>, char>.Compare(regExDfa, dfaEmpty));
        }


        [Test]
        public void RegExDfaEpsilonTest()
        {
            RegEx<char> regEx = RegExFactory.Epsilon<char>();
            RegExDfa<char> regExDfa = new RegExDfa<char>(regEx, 1);
            CheckNullTransitionTests(regExDfa);
            var dfaEpsilon = new RegExDfa<char>(new DFAState<char>(1, new KeyValuePair<char, DFAState<char>>[] { deadTransition }));
            Assert.IsTrue(DfaUtils.CompareDfa<RegExDfa<char>, DFAState<char>, char>.Compare(regExDfa, dfaEpsilon));
        }

        [Test]
        public void RegExDfaStarTest()
        {
            RegEx<char> regEx = RegExFactory.Star<char>(RegExFactory.Intersection<char>(RegExFactory.Range<char>('a'), RegExFactory.Complement<char>(RegExFactory.Range<char>('b'))));
            RegExDfa<char> regExDfa = new RegExDfa<char>(regEx, 1);
            CheckNullTransitionTests(regExDfa);
            DFAState<char> state_1 = new DFAState<char>();
            state_1._accepting = 1;
            state_1._transitions = new KeyValuePair<char, DFAState<char>>[] { deadTransition, new KeyValuePair<char, DFAState<char>>('a', state_1), new KeyValuePair<char, DFAState<char>>('b', deadState)};

            var dfaStar = new RegExDfa<char>(state_1);
            Assert.IsTrue(DfaUtils.CompareDfa<RegExDfa<char>, DFAState<char>, char>.Compare(regExDfa, dfaStar));
        }
    }
}
