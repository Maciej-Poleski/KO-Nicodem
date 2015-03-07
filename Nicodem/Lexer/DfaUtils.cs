using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Core;

namespace Nicodem.Lexer
{
    internal static class DfaUtils
    {
        internal static DFA Minimized<T,TU>(this T dfa) where T : IDfa<TU> where TU : IDfaState<TU>
        {
            // Funkcja minimalizująca DFA.
            // Można zmienić typ rezultatu z DFA na coś innego,
            // ale to coś innego powinno implementowac IDfa.
            // Nie zapomnij o tym, że istnieje potencjalnie wiele
            // rodzajów stanów akcpetujących (rozróżnianych różnymi
            // wartościami własności DFAState.Accepting: 0 - nieakceptujący,
            // coś innego niż 0 - jakiś rodzaj akceptacji)
            throw new NotImplementedException();
        }

		private static SortedSet<TU> DFS<T, TU> (TU currentState, SortedSet<TU> result) where T : IDfa<TU> where TU : IDfaState<TU>
		{
			if (!result.Contains (currentState))
				result.Add (currentState);

			var transitions = currentState.Transitions;

			foreach (var t in transitions) {
				if (!result.Contains (t.Value))
					result = DFS<T, TU> (t.Value, result);
			}

			return result;
		}

		private static IList<TU> PrepareStateList<T, TU> (T dfa) where T : IDfa<TU> where TU : IDfaState<TU>
		{
			SortedSet<TU> result = new SortedSet<TU>();
			var startState = dfa.Start;

			return DFS<T, TU> (startState, result).ToList ();
		}

		// returns sorted list of characters defining possible transition ranges in the given list of states
		private static char[] AlphabetRanges<TU> (IList<TU> stateList) where TU : IDfaState<TU>
		{
			SortedSet<char> ranges = new SortedSet<char> ();
			foreach (var st in stateList) {
				foreach (var transition in st.Transitions)
					ranges.Add (transition.Key);
			}

			return ranges.ToArray ();
		}

		private static T HopcroftAlgorithm<T, TU> (T dfa) where T : IDfa<TU> where TU : IDfaState<TU>
		{
			IList<TU> stateList = PrepareStateList<T, TU> (dfa);
			char[] alphabet = AlphabetRanges(stateList);

			var stateGroups = new Dictionary<uint, List<TU> > ();
			var stateMaps = new Dictionary<TU, List<TU> >();

			foreach (var state in stateList) {
				if(!stateGroups.ContainsKey(state.Accepting))
					stateGroups[state.Accepting] = new List<TU>();

				stateGroups [state.Accepting].Add (state);
				stateMaps [state] = stateGroups [state.Accepting];
			}

			var partition = new PartitionRefinement<TU, List<TU>  > (stateGroups.Values.ToList());

			ISet<List<TU> > queue = new SortedSet<List<TU>>();

			foreach (var set in stateGroups) {
				if (set.Key != 0)
					queue.Add (set.Value);
			}

			while (queue.Count > 0) {
				var mainSet = queue.First();
				queue.Remove (mainSet);

				foreach (char c in alphabet)
				{
					var prevSet = new List<TU> (); 

					foreach (var state in stateList) {
						var deltaState = state.Transitions[state.Transitions.GetLowerBound[c]].Value; //or upperbound?
						if (stateMaps[deltaState] == mainSet) //??????????
							prevSet.Add (state); //problem with mapping state - set
					}

				}
			}

			return dfa; //need build new DFA
		}
    }
}