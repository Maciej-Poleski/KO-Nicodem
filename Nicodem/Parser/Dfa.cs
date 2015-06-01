using System;
using Nicodem.Lexer;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Nicodem.Parser
{
	// Remember that Transitions is indexed by symbol **ranges**.
	public class Dfa<TSymbol>
		where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
	{
        /// <value>Starting state of this DFA.</value>
		public DfaState<TSymbol> Start { get; private set; }

		private static uint MaxAmbiguityHandler(uint leftState, uint rightState)
		{
			if (leftState < rightState) {
				return rightState;
			} else {
				return leftState;
			}
		}

        private IReadOnlyList<DfaState<TSymbol>> ListReachableStatesAndEnsureInitialized(DfaState<TSymbol> start)
        {
            var queue = new Queue<DfaState<TSymbol>>();
            var enqueued = new HashSet<DfaState<TSymbol>>();
            queue.Enqueue(start);
            enqueued.Add(start);
            var result = new List<DfaState<TSymbol>>();
            while (queue.Count > 0)
            {
                var s = queue.Dequeue();
                result.Add(s);
                if (s.Transitions == null)
                {
                    throw new ArgumentNullException("The state graph is not initialized.");
                }
                foreach (var i in s.Transitions)
                {
                    if (enqueued.Contains(i.Value))
                    {
                        continue;
                    }
                    queue.Enqueue(i.Value);
                    enqueued.Add(i.Value);
                }
            }
            return result;
        }

		public Dfa(DfaState<TSymbol> start)
		{
			Start = start;
            ListReachableStatesAndEnsureInitialized(start);
		}
			
		public Dfa(IDfa<TSymbol> lexerDfa)
		{
			var states = new Dictionary<IDfaState<TSymbol>, DfaState<TSymbol>>();
			var accepting = new Dictionary<DfaState<TSymbol>, uint>();
			var transitions = new Dictionary<DfaState<TSymbol>, List<KeyValuePair<TSymbol, DfaState<TSymbol>>>>();
			var queue = new Queue<IDfaState<TSymbol>>();

			states[lexerDfa.Start] = new DfaState<TSymbol>();
			accepting[states[lexerDfa.Start]] = lexerDfa.Start.Accepting;
			transitions[states[lexerDfa.Start]] = new List<KeyValuePair<TSymbol, DfaState<TSymbol>>>();
			queue.Enqueue(lexerDfa.Start);

			while (queue.Count > 0) {
				IDfaState<TSymbol> s1 = queue.Dequeue();
				DfaState<TSymbol> s2 = states[s1];
				foreach (var i in s1.Transitions) {
					if (!states.ContainsKey(i.Value)) {
						states[i.Value] = new DfaState<TSymbol>();
						accepting[states[i.Value]] = i.Value.Accepting;
						transitions[states[i.Value]] = new List<KeyValuePair<TSymbol, DfaState<TSymbol>>>();
						queue.Enqueue(i.Value);
					}
					transitions[s2].Add(new KeyValuePair<TSymbol, DfaState<TSymbol>>(i.Key, states[i.Value]));
				}
			}

			foreach (var i in states) {
				i.Value.Initialize(accepting[i.Value], transitions[i.Value]);
			}
			Start = states[lexerDfa.Start];
		}

		public static IDfa<TSymbol> RegexDfa(RegEx<TSymbol> RegEx, uint acceptingStateMarker)
		{
			if (acceptingStateMarker == 0) {
				throw new ArgumentOutOfRangeException();
			}
			AbstractDfa<DFAState<TSymbol>, TSymbol> factorDfa = 
				(AbstractDfa<DFAState<TSymbol>, TSymbol>) new RegExDfa<TSymbol>(RegEx, acceptingStateMarker);
			return DfaUtils.MakeMinimizedProductDfa<
					AbstractDfa<DFAState<TSymbol>, TSymbol>,
					DFAState<TSymbol>,	
					AbstractDfa<DFAState<TSymbol>, TSymbol>,
					DFAState<TSymbol>,
					TSymbol
				>(factorDfa, factorDfa, new DfaUtils.AmbiguityHandler(MaxAmbiguityHandler));
		}

		public static Dfa<TSymbol> ProductDfa(IDfa<TSymbol>[] dfas)
		{
			if (dfas.Length == 0) {
				throw new ArgumentException();
			} else if (dfas.Length == 1) {
				return new Dfa<TSymbol>(dfas[0]);
			} else {
				IDfa<TSymbol> cur = dfas[0];
				for (int i = 1; i < dfas.Length; i++) {
					cur = DfaUtils.MakeMinimizedProductDfa<
					TSymbol
					>(cur, dfas[i], new DfaUtils.AmbiguityHandler(MaxAmbiguityHandler));
				}
				return new Dfa<TSymbol>(cur);
			}
		}

        public override string ToString()
        {
            IReadOnlyList<DfaState<TSymbol>> states = ListReachableStatesAndEnsureInitialized(Start);
            var builder = new StringBuilder();
            int id = 0;
            var ids = new Dictionary<DfaState<TSymbol>, int>();
            foreach (var i in states) ids[i] = id++;

            builder.Append("{");
            foreach (var state in states) {
                builder.Append(state);
                builder.Append("; ");
                //builder.AppendLine();
            }
            builder.Append("}");
            return builder.ToString();
        }
	}

}

