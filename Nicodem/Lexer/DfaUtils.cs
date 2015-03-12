using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using Nicodem.Core;

namespace Nicodem.Lexer
{
    internal static class DfaUtils
    {
        internal static MinimizedDfa<TSymbol> Minimized<TDfa, TDfaState, TSymbol>(this TDfa dfa)
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
				
        internal struct MinimizedDfa<TSymbol> : IDfa<MinimizedDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
			public MinimizedDfaState<TSymbol> Start { get; internal set; }
        }

        internal class MinimizedDfaState<TSymbol> : IDfaState<MinimizedDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
			public uint Accepting { get; internal set; }
			public KeyValuePair<TSymbol, MinimizedDfaState<TSymbol> >[] Transitions { get; internal set; }
        }
			
			
		private class SimpleState<TSymbol>
		{
			public uint Accepting { get; internal set; }
			internal Dictionary<TSymbol, SimpleState<TSymbol> > Edges { get; set; }
		}

		private static MinimizedDfa<TSymbol> BuildNewDfa<TDfa, TDfaState, TSymbol> (TDfa dfa, PartitionRefinement<TDfaState> partition, IList<TDfaState> stateList) 
			where TDfa : IDfa<TDfaState, TSymbol> 
			where TDfaState : IDfaState<TDfaState, TSymbol>
			where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			var oldToSimpleState = new Dictionary<TDfaState, SimpleState<TSymbol> >();
			var setToState = new Dictionary<LinkedList<TDfaState>, SimpleState<TSymbol> >();
			var simpleStateToMinimizedState = new Dictionary<SimpleState<TSymbol>, MinimizedDfaState<TSymbol> > ();

			foreach (var oldState in stateList) {
				var set = partition [oldState];

				if (!setToState.ContainsKey (set)) {
					var newSimpleState = new SimpleState<TSymbol> ();
					setToState [set] = newSimpleState;
					simpleStateToMinimizedState [newSimpleState] = new MinimizedDfaState<TSymbol> ();

					newSimpleState.Accepting = oldState.Accepting;
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


			foreach (var statePair in simpleStateToMinimizedState) {
				var simpleState = statePair.Key;
				var minimizedState = statePair.Value;

				minimizedState.Accepting = simpleState.Accepting;

				var newTransitions = new List<KeyValuePair<TSymbol, MinimizedDfaState<TSymbol> > >();

				foreach (var edge in simpleState.Edges) {
					var newEdge = new KeyValuePair<TSymbol, MinimizedDfaState<TSymbol> > (edge.Key, simpleStateToMinimizedState[edge.Value]);
					newTransitions.Add (newEdge);
				}

				minimizedState.Transitions = newTransitions.ToArray();

			}

			var newMinimizedDfa = new MinimizedDfa<TSymbol>();
			var startSet = partition [dfa.Start];
			newMinimizedDfa.Start = simpleStateToMinimizedState [setToState [startSet]];

			return newMinimizedDfa;
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

		private static MinimizedDfa<TSymbol> HopcroftAlgorithm<TDfa, TDfaState, TSymbol> (TDfa dfa) 
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