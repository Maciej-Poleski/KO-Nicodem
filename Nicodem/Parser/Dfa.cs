using System;
using Nicodem.Lexer;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nicodem.Parser
{
	// Remember that Transitions is indexed by symbol **ranges**.
	public class Dfa<TSymbol> 
		where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
	{
		public DfaState<TSymbol> Start { get; private set; }

		private static uint MaxAmbiguityHandler(uint leftState, uint rightState)
		{
			if (leftState < rightState) {
				return rightState;
			} else {
				return leftState;
			}
		}

		public Dfa(DfaState<TSymbol> start)
		{
			Start = start;
			var queue = new Queue<DfaState<TSymbol>>();
			var enqueued = new HashSet<DfaState<TSymbol>>();
			queue.Enqueue(start);
			enqueued.Add(start);
			while (queue.Count > 0) {
				var s = queue.Dequeue();
				if (s.Transitions == null) {
					throw new ArgumentNullException("The state graph is not initialized.");
				}
				foreach (var i in s.Transitions) {
					if (enqueued.Contains(i.Value)) {
						continue;
					}
					queue.Enqueue(i.Value);
					enqueued.Add(i.Value);
				}
			}
		}
			
		public Dfa(DfaUtils.MinimizedDfa<TSymbol> lexerDfa)
		{
			var states = new Dictionary<DfaUtils.MinimizedDfaState<TSymbol>, DfaState<TSymbol>>();
			var accepting = new Dictionary<DfaState<TSymbol>, uint>();
			var transitions = new Dictionary<DfaState<TSymbol>, List<KeyValuePair<TSymbol, DfaState<TSymbol>>>>();
			var queue = new Queue<DfaUtils.MinimizedDfaState<TSymbol>>();

			states[lexerDfa.Start] = new DfaState<TSymbol>();
			accepting[states[lexerDfa.Start]] = lexerDfa.Start.Accepting;
			transitions[states[lexerDfa.Start]] = new List<KeyValuePair<TSymbol, DfaState<TSymbol>>>();
			queue.Enqueue(lexerDfa.Start);

			while (queue.Count > 0) {
				DfaUtils.MinimizedDfaState<TSymbol> s1 = queue.Dequeue();
				DfaState<TSymbol> s2 = states[s1];
				foreach (var i in s1.Transitions) {
					if (!states.ContainsKey(i.Value)) {
						states[i.Value] = new DfaState<TSymbol>();
						accepting[states[i.Value]] = s1.Accepting;
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

		public static DfaUtils.MinimizedDfa<TSymbol> RegexDfa(RegEx<TSymbol> RegEx, uint acceptingStateMarker)
		{
			if (acceptingStateMarker == 0) {
				throw new ArgumentOutOfRangeException();
			}
			IDfa<DFAState<TSymbol>, TSymbol> factorDfa = 
				(IDfa<DFAState<TSymbol>, TSymbol>) new RegExDfa<TSymbol>(RegEx, acceptingStateMarker);
			return DfaUtils.MakeMinimizedProductDfa<
					IDfa<DFAState<TSymbol>, TSymbol>,
					DFAState<TSymbol>,	
					IDfa<DFAState<TSymbol>, TSymbol>,
					DFAState<TSymbol>,
					TSymbol
				>(factorDfa, factorDfa, new DfaUtils.AmbiguityHandler(MaxAmbiguityHandler));
		}

		public static Dfa<TSymbol> ProductDfa(DfaUtils.MinimizedDfa<TSymbol>[] dfas)
		{
			if (dfas.Length == 0) {
				throw new ArgumentException();
			} else if (dfas.Length == 1) {
				return new Dfa<TSymbol>(dfas[0]);
			} else {
				DfaUtils.MinimizedDfa<TSymbol> cur = dfas[0];
				for (int i = 1; i < dfas.Length; i++) {
					cur = DfaUtils.MakeMinimizedProductDfa<
						DfaUtils.MinimizedDfa<TSymbol>,
					DfaUtils.MinimizedDfaState<TSymbol>,	
					DfaUtils.MinimizedDfa<TSymbol>,
					DfaUtils.MinimizedDfaState<TSymbol>,
					TSymbol
					>(cur, dfas[i], new DfaUtils.AmbiguityHandler(MaxAmbiguityHandler));
				}
				return new Dfa<TSymbol>(cur);
			}
		}
	}
}

