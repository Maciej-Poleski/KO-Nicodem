using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nicodem.Core;

namespace Nicodem.Lexer
{
    public static class DfaUtils
    {
        #region MinimizedDfa

        internal static MinimizedDfa<TSymbol> Minimized<TDfa, TDfaState, TSymbol>(this TDfa dfa)
            where TDfa : IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            // Funkcja minimalizująca DFA.
            // Można zmienić typ rezultatu z DFA na coś innego,
            // ale to coś innego powinno implementowac IDfa.
            // Nie zapomnij o tym, że istnieje potencjalnie wiele
            // rodzajów stanów akcpetujących (rozróżnianych różnymi
            // wartościami własności DFAState.Accepting: 0 - nieakceptujący,
            // coś innego niż 0 - jakiś rodzaj akceptacji)
			return HopcroftAlgorithm<TDfa, TDfaState, TSymbol>(dfa);
        }

        public struct MinimizedDfa<TSymbol> : IDfa<MinimizedDfaState<TSymbol>, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            public MinimizedDfaState<TSymbol> Start { get; internal set; }
        }

        public class MinimizedDfaState<TSymbol> : IDfaState<MinimizedDfaState<TSymbol>, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
			public uint Accepting { get; internal set; }
			public KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>[] Transitions { get; internal set; }
        }
			
			
		private class SimpleState<TSymbol>
		{
			public uint Accepting { get; internal set; }
			internal Dictionary<TSymbol, SimpleState<TSymbol>> Edges { get; set; }
            internal SimpleState() 
            {
                Edges = new Dictionary<TSymbol, SimpleState<TSymbol>>();
            }
		}

		private static MinimizedDfa<TSymbol> BuildNewDfa<TDfa, TDfaState, TSymbol> (TDfa dfa, PartitionRefinement<TDfaState> partition, IList<TDfaState> stateList) 
			where TDfa : IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			var oldToSimpleState = new Dictionary<TDfaState, SimpleState<TSymbol>>();
			var setToState = new Dictionary<LinkedList<TDfaState>, SimpleState<TSymbol>>();
			var simpleStateToMinimizedState = new Dictionary<SimpleState<TSymbol>, MinimizedDfaState<TSymbol>>();

			foreach (var oldState in stateList) {
				var set = partition [oldState];

				if (!setToState.ContainsKey (set)) {
					var newSimpleState = new SimpleState<TSymbol>();
					setToState [set] = newSimpleState;
					simpleStateToMinimizedState [newSimpleState] = new MinimizedDfaState<TSymbol>();

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

				var newTransitions = new List<KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>>();

				foreach (var edge in simpleState.Edges) {
					var newEdge = new KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>(edge.Key, simpleStateToMinimizedState[edge.Value]);
					newTransitions.Add (newEdge);
				}

				minimizedState.Transitions = newTransitions.ToArray();

			}

			var newMinimizedDfa = new MinimizedDfa<TSymbol>();
			var startSet = partition [dfa.Start];
			newMinimizedDfa.Start = simpleStateToMinimizedState [setToState [startSet]];

			return newMinimizedDfa;
		}

        private static HashSet<TDfaState> DFS<TDfa, TDfaState, TSymbol> (TDfaState currentState, HashSet<TDfaState> result) 
			where TDfa : IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			if (!result.Contains (currentState))
				result.Add (currentState);

			var transitions = currentState.Transitions;

			foreach (var t in transitions) {
				if (!result.Contains (t.Value))
					result = DFS<TDfa, TDfaState, TSymbol>(t.Value, result);
			}

			return result;
		}

		private static IList<TDfaState> PrepareStateList<TDfa, TDfaState, TSymbol> (TDfa dfa) 
			where TDfa : IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
            HashSet<TDfaState> result = new HashSet<TDfaState>();
			var startState = dfa.Start;

			return DFS<TDfa, TDfaState, TSymbol>(startState, result).ToList ();
		}

		// returns sorted list of characters defining possible transition ranges in the given list of states
		private static TSymbol[] AlphabetRanges<TDfaState, TSymbol> (IList<TDfaState> stateList) 
			where TDfaState : IDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			SortedSet<TSymbol> ranges = new SortedSet<TSymbol>();
			foreach (var st in stateList) {
				foreach (var transition in st.Transitions)
					ranges.Add (transition.Key);
			}

			return ranges.ToArray ();
		}

		private static MinimizedDfa<TSymbol> HopcroftAlgorithm<TDfa, TDfaState, TSymbol> (TDfa dfa) 
			where TDfa : IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			IList<TDfaState> stateList = PrepareStateList<TDfa, TDfaState, TSymbol>(dfa);
			TSymbol[] alphabet = AlphabetRanges<TDfaState, TSymbol>(stateList);

			var stateGroups = new Dictionary<uint, LinkedList<TDfaState>>();

			foreach (var state in stateList) {
				if(!stateGroups.ContainsKey(state.Accepting))
					stateGroups[state.Accepting] = new LinkedList<TDfaState>();

				stateGroups [state.Accepting].AddLast (state);
			}

			var partition = new PartitionRefinement<TDfaState>(stateGroups.Values.ToList());

			ISet<LinkedList<TDfaState>> queue = new SortedSet<LinkedList<TDfaState>>();

			foreach (var set in stateGroups) {
				if (set.Key != 0)
					queue.Add (set.Value);
			}

			while (queue.Count > 0) {
				var mainSet = queue.First();
				queue.Remove (mainSet);

				foreach (TSymbol c in alphabet)
				{
					var prevSet = new LinkedList<TDfaState>(); 

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

			var newDfa = BuildNewDfa<TDfa, TDfaState, TSymbol>(dfa, partition, stateList);

			return newDfa;
        }

        #endregion

        #region ProductDfa

        public delegate uint AmbiguityHandler(uint leftState, uint rightState);

        internal class DfaStatesNotNullConcpetCheck<TDfa, TDfaState, TSymbol>
            where TDfa : IDfa<TDfaState, TSymbol>
            where TDfaState : IDfaState<TDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            private readonly HashSet<TDfaState> _visitedStates = new HashSet<TDfaState>();

            private DfaStatesNotNullConcpetCheck()
            {
            }

            [Conditional("DEBUG")]
            internal static void CheckDfaStatesNotNull(TDfa dfa)
            {
                new DfaStatesNotNullConcpetCheck<TDfa, TDfaState, TSymbol>().Check(dfa.Start);
            }

            private void Check(TDfaState state)
            {
                if (state == null)
                {
                    Debug.Fail("Found null state in DFA");
                }
                if (_visitedStates.Contains(state))
                    return;
                _visitedStates.Add(state);
                Debug.Assert(state.Transitions != null);
                Debug.Assert(state.Transitions.Length > 0);
                foreach (var transition in state.Transitions)
                {
                    Check(transition.Value);
                }
            }
        }

        private static ProductDfa<TSymbol> MakeProductDfa<TLastDfa, TLastDfaState, TNewDfa, TNewDfaState, TSymbol>(
            TLastDfa lastDfa, TNewDfa newDfa, AmbiguityHandler handler)
            where TLastDfa : IDfa<TLastDfaState, TSymbol> where TLastDfaState : IDfaState<TLastDfaState, TSymbol>
            where TNewDfa : IDfa<TNewDfaState, TSymbol>
            where TNewDfaState : IDfaState<TNewDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            var result = new ProductDfaBuilder<TLastDfaState, TNewDfaState, TSymbol>(handler).Build(lastDfa.Start,
                newDfa.Start);
            DfaStatesNotNullConcpetCheck<ProductDfa<TSymbol>, ProductDfaState<TSymbol>, TSymbol>.CheckDfaStatesNotNull(
                result);
            return result;
        }

        public static MinimizedDfa<TSymbol> MakeMinimizedProductDfa
            <TLastDfa, TLastDfaState, TNewDfa, TNewDfaState, TSymbol>(TLastDfa lastDfa, TNewDfa newDfa,
                AmbiguityHandler handler)
            where TLastDfa : IDfa<TLastDfaState, TSymbol>
            where TLastDfaState : IDfaState<TLastDfaState, TSymbol>
            where TNewDfa : IDfa<TNewDfaState, TSymbol>
            where TNewDfaState : IDfaState<TNewDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            var result =
                MakeProductDfa<TLastDfa, TLastDfaState, TNewDfa, TNewDfaState, TSymbol>(lastDfa, newDfa, handler)
                    .Minimized<ProductDfa<TSymbol>, ProductDfaState<TSymbol>, TSymbol>();
            DfaStatesNotNullConcpetCheck<MinimizedDfa<TSymbol>, MinimizedDfaState<TSymbol>, TSymbol>
                .CheckDfaStatesNotNull(result);
            return result;
        }

        private struct ProductDfaBuilder<TLastDfaState, TNewDfaState, TSymbol>
            where TLastDfaState : IDfaState<TLastDfaState, TSymbol>
            where TNewDfaState : IDfaState<TNewDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            private readonly AmbiguityHandler _getProductAccepting;
            private readonly Dictionary<Tuple<TLastDfaState, TNewDfaState>, ProductDfaState<TSymbol>> _productionMapping;

            public ProductDfaBuilder(AmbiguityHandler handler)
            {
                _getProductAccepting = handler;
                _productionMapping = new Dictionary<Tuple<TLastDfaState, TNewDfaState>, ProductDfaState<TSymbol>>();
            }

            public ProductDfa<TSymbol> Build(TLastDfaState lastDfaStart, TNewDfaState newDfaStart)
            {
                return new ProductDfa<TSymbol> {Start = GetProductState(lastDfaStart, newDfaStart)};
            }

            private ProductDfaState<TSymbol> GetProductState(TLastDfaState lastDfaState, TNewDfaState newDfaState)
            {
                var statePack = new Tuple<TLastDfaState, TNewDfaState>(lastDfaState, newDfaState);
                if (_productionMapping.ContainsKey(statePack))
                {
                    return _productionMapping[statePack];
                }
                // Tworzę stub stanu, tak aby można było się do niego odnosić
                var productState = new ProductDfaState<TSymbol>
                {
                    Accepting = _getProductAccepting(lastDfaState.Accepting, newDfaState.Accepting)
                };
                _productionMapping[statePack] = productState;
                // Przygotowuję przejścia
                var productTransitions = MakeProductTransitions(lastDfaState.Transitions, newDfaState.Transitions);
                // Zapisuję wygenerowane przejścia w stanie
                productState.Transitions = productTransitions.ToArray();
                // Stan jest gotowy
                return productState;
            }

            private List<KeyValuePair<TSymbol, ProductDfaState<TSymbol>>> MakeProductTransitions(
                KeyValuePair<TSymbol, TLastDfaState>[] lastTransitions,
                KeyValuePair<TSymbol, TNewDfaState>[] newTransitions)
            {
                Debug.Assert(EqualityComparer<TSymbol>.Default.Equals(lastTransitions[0].Key, default(TSymbol)));
                Debug.Assert(EqualityComparer<TSymbol>.Default.Equals(newTransitions[0].Key, default(TSymbol)));
                int lastIndex = 0, newIndex = 0; // Zakładam że na pozycji 0 są znaki '\0'
                var productTransitions =
                    new List<KeyValuePair<TSymbol, ProductDfaState<TSymbol>>>(Math.Max(lastTransitions.Length,
                        newTransitions.Length));
                while (lastIndex < lastTransitions.Length && newIndex < newTransitions.Length)
                {
                    // Dodaj obecnie widoczne przejście
                    var c = lastTransitions[lastIndex].Key.CompareTo(newTransitions[newIndex].Key) > 0
                        ? lastTransitions[lastIndex].Key
                        : newTransitions[newIndex].Key;
                    productTransitions.Add(new KeyValuePair<TSymbol, ProductDfaState<TSymbol>>(c,
                        GetProductState(lastTransitions[lastIndex].Value, newTransitions[newIndex].Value)));
                    // Szukaj granicy kolejnego przejścia
                    if (lastIndex == lastTransitions.Length - 1)
                    {
                        newIndex += 1;
                    }
                    else if (newIndex == newTransitions.Length - 1)
                    {
                        lastIndex += 1;
                    }
                    else
                    {
                        Debug.Assert(lastTransitions[lastIndex].Key.CompareTo(lastTransitions[lastIndex + 1].Key) < 0);
                        Debug.Assert(newTransitions[newIndex].Key.CompareTo(newTransitions[newIndex + 1].Key) < 0);
                        if (lastTransitions[lastIndex + 1].Key.Equals(newTransitions[newIndex + 1].Key))
                        {
                            lastIndex += 1;
                            newIndex += 1;
                        }
                        else if (lastTransitions[lastIndex + 1].Key.CompareTo(newTransitions[newIndex + 1].Key) < 0)
                        {
                            lastIndex += 1;
                            Debug.Assert(lastTransitions[lastIndex].Key.CompareTo(newTransitions[newIndex].Key) > 0);
                        }
                        else
                        {
                            newIndex += 1;
                            Debug.Assert(newTransitions[newIndex].Key.CompareTo(lastTransitions[lastIndex].Key) > 0);
                        }
                    }
                }
                return productTransitions;
            }
        }

        private class ProductDfaState<TSymbol> : IDfaState<ProductDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            public uint Accepting { get; internal set; }
            public KeyValuePair<TSymbol, ProductDfaState<TSymbol>>[] Transitions { get; internal set; }

            internal static ProductDfaState<TSymbol> MakeDeadState()
            {
                var result = new ProductDfaState<TSymbol> {Accepting = 0};
                result.Transitions = new[]
                {new KeyValuePair<TSymbol, ProductDfaState<TSymbol>>(default(TSymbol), result)};
                return result;
            }
        }

        private struct ProductDfa<TSymbol> : IDfa<ProductDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            public ProductDfaState<TSymbol> Start { get; internal set; }
        }

        internal static MinimizedDfa<TSymbol> MakeEmptyLanguageDfa<TSymbol>()
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            var result = new ProductDfa<TSymbol> {Start = ProductDfaState<TSymbol>.MakeDeadState()}
                    .Minimized<ProductDfa<TSymbol>, ProductDfaState<TSymbol>, TSymbol>();
            DfaStatesNotNullConcpetCheck<MinimizedDfa<TSymbol>,MinimizedDfaState<TSymbol>,TSymbol>.CheckDfaStatesNotNull(result);
            return result;

        }

        #endregion
    }
}