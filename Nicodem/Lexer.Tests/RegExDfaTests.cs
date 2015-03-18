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

                foreach (var transition in state.Transitions)
                {
                    Assert.IsNotNull(transition.Value);
                    if(!visited.ContainsKey(transition.Value))
                        queue.Enqueue(transition.Value);
                }

            }

        }

        //Create Dfa from empty RegEx
        [Test]
        public void EmptyRegExTests()
        {
            RegExDfa<char> regExDfa = new RegExDfa<char>(RegExFactory.Empty<char>(), 1);
            CheckNullTransitionTests(regExDfa);
        }

        [Test]
        public void EpsilonRegExTests()
        {
            RegExDfa<char> regExDfa = new RegExDfa<char>(RegExFactory.Epsilon<char>(), 1);
            CheckNullTransitionTests(regExDfa);
        }
    }
}
