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
			return HopcroftAlgorithm<T, TU> (dfa);
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
			
		private class SimpleState
		{
			public Dictionary<char, SimpleState> Edges { get; set; }
		}

		private static T BuildNewDfa<T, TU> (T dfa, PartitionRefinement<TU> partition, IList<TU> stateList) where T : IDfa<TU> where TU : IDfaState<TU>
		{
			var oldToSimpleState = new Dictionary<TU, SimpleState>();
			var setToState = new Dictionary<LinkedList<TU>, SimpleState>();

			foreach (var oldState in stateList) {
				var set = partition [oldState];

				if (!setToState.ContainsKey (set)) {
					setToState [set] = new SimpleState ();
				}

				oldToSimpleState [oldState] = setToState [set];
			}

			foreach (var oldState in stateList) {
				var stateA = oldToSimpleState [oldState];

				foreach(var transtion in oldState.Transitions) {
					var stateB = oldToSimpleState [transtion.Value];

					if (!stateA.Edges.ContainsKey (transtion.Key))
						stateA.Edges [transtion.Key] = stateB;
				}
			}

			var oldToNewState = new Dictionary<TU, TU> ();
			foreach (var oldState in stateList) {
				var set = partition [oldState];

				//.....
			}
			return dfa;
		}

		private static T HopcroftAlgorithm<T, TU> (T dfa) where T : IDfa<TU> where TU : IDfaState<TU>
		{
			IList<TU> stateList = PrepareStateList<T, TU> (dfa);
			char[] alphabet = AlphabetRanges(stateList);

			var stateGroups = new Dictionary<uint, LinkedList<TU> > ();

			foreach (var state in stateList) {
				if(!stateGroups.ContainsKey(state.Accepting))
					stateGroups[state.Accepting] = new LinkedList<TU>();

				stateGroups [state.Accepting].AddLast (state);
			}

			var partition = new PartitionRefinement<TU> (stateGroups.Values.ToList());

			ISet<LinkedList<TU> > queue = new SortedSet<LinkedList<TU>>();

			foreach (var set in stateGroups) {
				if (set.Key != 0)
					queue.Add (set.Value);
			}

			while (queue.Count > 0) {
				var mainSet = queue.First();
				queue.Remove (mainSet);

				foreach (char c in alphabet)
				{
					var prevSet = new LinkedList<TU> (); 

					foreach (var state in stateList) {
						var deltaState = state.Transitions[Array.BinarySearch(state.Transitions, c)].Value; //or upperbound?
						if (partition [deltaState] == mainSet)
							prevSet.AddFirst (state);
					}

					var setsPartition = partition.Refine (prevSet);

					foreach (var setPartition in setsPartition) {
						if (queue.Contains (setPartition.Difference)) {
							queue.Add (setPartition.Intersection);
						} else {
							if (setPartition.Difference.Count <= setPartition.Intersection.Count) {
								queue.Add (setPartition.Difference);
							} else {
								queue.Add (setPartition.Intersection);
							}
						}
					}

				}
			}

			var newDfa = BuildNewDfa<T, TU> (dfa, partition, stateList);

			return newDfa;
		}
    }
}