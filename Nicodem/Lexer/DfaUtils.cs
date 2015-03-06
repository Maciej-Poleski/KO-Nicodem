using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

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
    }
}