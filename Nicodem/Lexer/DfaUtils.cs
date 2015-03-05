using System;
using System.Net;
using System.Collections.Generic;

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
            // coś innego niż - jakiś rodzaj akceptacji)
            throw new NotImplementedException();
        }

		private static IList<TU> DFS<T, TU> (TU currentState, IList<TU> result) where T : IDfa<TU> where TU : IDfaState<TU>
		{
			if (result.Contains (currentState) == false)
				result.Add (currentState);

			var transitions = currentState.Transitions;

			foreach (var t in transitions) {
				if (result.Contains (t.Key) == false)
					result = DFS (t.Key, result);
			}

			return result;
		}

		private static IList<TU> prepareStateList<T, TU> (T dfa) where T : IDfa<TU> where TU : IDfaState<TU>
		{
			IList<TU> result = new List<TU>();
			var startState = dfa.Start;

			return DFS<T, TU> (startState, result);
		}
    }
}