using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Core;

namespace Nicodem.Lexer
{
    internal static class DfaUtils
    {
		internal static RegexDfa<TSymbol> Minimized<TDfa, TDfaState, TSymbol>(this TDfa dfa)
			where TDfa : IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol>
			where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            // Funkcja minimalizująca DFA.
            // Można zmienić typ rezultatu z DFA na coś innego,
            // ale to coś innego powinno implementowac IDfa.
            // Nie zapomnij o tym, że istnieje potencjalnie wiele
            // rodzajów stanów akcpetujących (rozróżnianych różnymi
            // wartościami własności DFAState.Accepting: 0 - nieakceptujący,
            // coś innego niż 0 - jakiś rodzaj akceptacji)
			return HopcroftAlgorithm<TDfa, TDfaState, TSymbol> (dfa);
        }

		private static SortedSet<TDfaState> DFS<TDfa, TDfaState, TSymbol> (TDfaState currentState, SortedSet<TDfaState> result) 
			where TDfa : IDfa<TDfaState, TSymbol> 
			where TDfaState : IDfaState<TDfaState, TSymbol>
			where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			if (!result.Contains (currentState))
				result.Add (currentState);

			var transitions = currentState.Transitions;

			foreach (var t in transitions) {
				if (!result.Contains (t.Value))
					result = DFS<TDfa, TDfaState, TSymbol> (t.Value, result);
			}

			return result;
		}

		private static IList<TDfaState> PrepareStateList<TDfa, TDfaState, TSymbol> (TDfa dfa) 
			where TDfa : IDfa<TDfaState, TSymbol> 
			where TDfaState : IDfaState<TDfaState, TSymbol>
			where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			SortedSet<TDfaState> result = new SortedSet<TDfaState>();
			var startState = dfa.Start;

			return DFS<TDfa, TDfaState, TSymbol> (startState, result).ToList ();
		}

		// returns sorted list of characters defining possible transition ranges in the given list of states
		private static TSymbol[] AlphabetRanges<TDfaState, TSymbol> (IList<TDfaState> stateList) 
			where TDfaState : IDfaState<TDfaState, TSymbol>
			where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			SortedSet<TSymbol> ranges = new SortedSet<TSymbol> ();
			foreach (var st in stateList) {
				foreach (var transition in st.Transitions)
					ranges.Add (transition.Key);
			}

			return ranges.ToArray ();
		}
			
		private class SimpleState<TSymbol>
		{
			public Dictionary<TSymbol, SimpleState<TSymbol> > Edges { get; set; }
		}

		private static RegexDfa<TSymbol> BuildNewDfa<TDfa, TDfaState, TSymbol> (TDfa dfa, PartitionRefinement<TDfaState> partition, IList<TDfaState> stateList) 
			where TDfa : IDfa<TDfaState, TSymbol> 
			where TDfaState : IDfaState<TDfaState, TSymbol>
			where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			var oldToSimpleState = new Dictionary<TDfaState, SimpleState<TSymbol> >();
			var setToState = new Dictionary<LinkedList<TDfaState>, SimpleState<TSymbol> >();

			foreach (var oldState in stateList) {
				var set = partition [oldState];

				if (!setToState.ContainsKey (set)) {
					setToState [set] = new SimpleState<TSymbol> ();
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

			var oldToNewState = new Dictionary<TDfaState, TDfaState> ();
			foreach (var oldState in stateList) {
				var set = partition [oldState];

				//.....
			}
			//return dfa;
			return null;
		}

		private static RegexDfa<TSymbol> HopcroftAlgorithm<TDfa, TDfaState, TSymbol> (TDfa dfa) 
			where TDfa : IDfa<TDfaState, TSymbol> 
			where TDfaState : IDfaState<TDfaState, TSymbol>
			where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			IList<TDfaState> stateList = PrepareStateList<TDfa, TDfaState, TSymbol> (dfa);
			TSymbol[] alphabet = AlphabetRanges<TDfaState, TSymbol> (stateList);

			var stateGroups = new Dictionary<uint, LinkedList<TDfaState> > ();

			foreach (var state in stateList) {
				if(!stateGroups.ContainsKey(state.Accepting))
					stateGroups[state.Accepting] = new LinkedList<TDfaState>();

				stateGroups [state.Accepting].AddLast (state);
			}

			var partition = new PartitionRefinement<TDfaState> (stateGroups.Values.ToList());

			ISet<LinkedList<TDfaState> > queue = new SortedSet<LinkedList<TDfaState>>();

			foreach (var set in stateGroups) {
				if (set.Key != 0)
					queue.Add (set.Value);
			}

			while (queue.Count > 0) {
				var mainSet = queue.First();
				queue.Remove (mainSet);

				foreach (TSymbol c in alphabet)
				{
					var prevSet = new LinkedList<TDfaState> (); 

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

			var newDfa = BuildNewDfa<TDfa, TDfaState, TSymbol> (dfa, partition, stateList);

			return newDfa;
		}
    }
}