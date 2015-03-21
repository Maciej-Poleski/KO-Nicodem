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
            deadState.Accepting = 0;
            deadState.Transitions = new KeyValuePair<char, DFAState<char>>[] { new KeyValuePair<char, DFAState<char>>('\0', deadState) };
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

        //TODO(karol-banys)
        bool CompareDfaState<TDfaState, TSymbol>(IDfaState<TDfaState, TSymbol> a, IDfaState<TDfaState, TSymbol> b, Dictionary<Tuple<IDfaState<TDfaState, TSymbol> ,IDfaState<TDfaState, TSymbol> >, int> visited)
            where TDfaState : IDfaState<TDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            //if (visited.Contains(Tuple.Create(a, b))) return true;
            visited[Tuple.Create(a, b)] = 1;
            if (a == null || b == null) return false;
            if (a.Accepting != b.Accepting) return false;
            if (a.Transitions == null || b.Transitions == null) return false;
            if (a.Transitions.Length != b.Transitions.Length) return false;

            foreach (var transitionA in a.Transitions)
            {
                foreach (var transitionB in b.Transitions)
                {
                    bool statesAreEqual = false;
                    if (transitionA.Key.Equals(transitionB.Key))
                    {
                        if (!CompareDfaState(transitionA.Value, transitionB.Value, visited)) return false;
                        else
                        {
                            statesAreEqual = true;
                            break;
                        }
                    }
                    if (!statesAreEqual) return false;
                }
            }
            return true;
        }

        bool CompareDfa<TDfaState, TSymbol>(IDfa<TDfaState, TSymbol> a, IDfa<TDfaState, TSymbol> b)
            where TDfaState : IDfaState<TDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            var visited = new Dictionary<Tuple<IDfaState<TDfaState, TSymbol>, IDfaState<TDfaState, TSymbol>>, int>();
            return CompareDfaState(a.Start, b.Start, visited);
        }

         [Test]
         public void EmptyRegExTests()
         {
             RegExDfa<char> regExDfa = new RegExDfa<char>(RegExFactory.Empty<char>(), 0);
             CheckNullTransitionTests(regExDfa);
             var dfaEmpty = new RegExDfa<char>(new DFAState<char>(0, new KeyValuePair<char, DFAState<char>>[]{ deadTransition }));
            //Assert.IsTrue(CompareDfa(regExDfa, dfaEmpty)); <- comparison hasn't worked yet
         }


        [Test]
        public void EpsilonRegExTests()
        {
            RegExDfa<char> regExDfa = new RegExDfa<char>(RegExFactory.Epsilon<char>(), 1);
            CheckNullTransitionTests(regExDfa);
            var dfaEpsilon = new RegExDfa<char>(new DFAState<char>(1, new KeyValuePair<char, DFAState<char>>[] { deadTransition }));
            //Assert.IsTrue(CompareDfa(regExDfa, dfaEpsilon)); <- comparison hasn't worked yet
        }
    }
}
