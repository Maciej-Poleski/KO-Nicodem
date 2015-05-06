using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Nicodem.Core;

namespace Nicodem.Lexer
{
    public static class DfaUtils
    {
        #region MinimizedDfa

        public static MinimizedDfa<TSymbol> Minimized<TDfa, TDfaState, TSymbol>(this TDfa dfa)
            where TDfa : AbstractDfa<TDfaState, TSymbol> where TDfaState : AbstractDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            // Funkcja minimalizująca DFA.
            // Można zmienić typ rezultatu z DFA na coś innego,
            // ale to coś innego powinno implementowac AbstractDfa.
            // Nie zapomnij o tym, że istnieje potencjalnie wiele
            // rodzajów stanów akcpetujących (rozróżnianych różnymi
            // wartościami własności DFAState.Accepting: 0 - nieakceptujący,
            // coś innego niż 0 - jakiś rodzaj akceptacji)
			return HopcroftAlgorithm<TDfa, TDfaState, TSymbol>(dfa);
        }

        public class MinimizedDfa<TSymbol> : AbstractDfa<MinimizedDfaState<TSymbol>, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            internal MinimizedDfaState<TSymbol> _start;

            public override MinimizedDfaState<TSymbol> Start
            {
                get { return _start; }
            }
        }

        public class MinimizedDfaState<TSymbol> : AbstractDfaState<MinimizedDfaState<TSymbol>, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            internal uint _accepting;
            internal KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>[] _transitions;

            public override uint Accepting
            {
                get { return _accepting; }
            }

            public override KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>[] Transitions
            {
                get { return _transitions; }
            }
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

		private static int CompareByKey<TDfa, TDfaState, TSymbol>(KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>> a, KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>> b)
			where TDfa : AbstractDfa<TDfaState, TSymbol> where TDfaState : AbstractDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			return a.Key.CompareTo(b.Key);
		}

		private static MinimizedDfa<TSymbol> BuildNewDfa<TDfa, TDfaState, TSymbol> (TDfa dfa, PartitionRefinement<TDfaState> partition, IList<TDfaState> stateList) 
			where TDfa : AbstractDfa<TDfaState, TSymbol> where TDfaState : AbstractDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
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

				minimizedState._accepting = simpleState.Accepting;

				var newTransitions = new List<KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>>();

				foreach (var edge in simpleState.Edges) {
					var newEdge = new KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>(edge.Key, simpleStateToMinimizedState[edge.Value]);
					newTransitions.Add (newEdge);
				}

				newTransitions.Sort (CompareByKey<TDfa, TDfaState, TSymbol>);

				minimizedState._transitions = newTransitions.ToArray();

			}

			var newMinimizedDfa = new MinimizedDfa<TSymbol>();
			var startSet = partition [dfa.Start];
			newMinimizedDfa._start = simpleStateToMinimizedState [setToState [startSet]];

			return newMinimizedDfa;
		}

        private static HashSet<TDfaState> DFS<TDfa, TDfaState, TSymbol> (TDfaState currentState, HashSet<TDfaState> result) 
			where TDfa : AbstractDfa<TDfaState, TSymbol> where TDfaState : AbstractDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
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

        public static IList<TDfaState> PrepareStateList<TDfa, TDfaState, TSymbol> (TDfa dfa) 
			where TDfa : AbstractDfa<TDfaState, TSymbol> where TDfaState : AbstractDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
            HashSet<TDfaState> result = new HashSet<TDfaState>();
			var startState = dfa.Start;

			return DFS<TDfa, TDfaState, TSymbol>(startState, result).ToList ();
		}

		// returns sorted list of characters defining possible transition ranges in the given list of states
		private static TSymbol[] AlphabetRanges<TDfaState, TSymbol> (IList<TDfaState> stateList) 
			where TDfaState : AbstractDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
		{
			SortedSet<TSymbol> ranges = new SortedSet<TSymbol>();
			foreach (var st in stateList) {
				foreach (var transition in st.Transitions)
					ranges.Add (transition.Key);
			}

			return ranges.ToArray ();
		}

        private static int SimpleBinarySearch<TDfaState, TSymbol> (KeyValuePair<TSymbol, TDfaState>[] array, TSymbol c) 
            where TDfaState : AbstractDfaState<TDfaState, TSymbol> 
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            int L = 0, P = array.Length, S;
            while(L < P) {
                S = (L + P) / 2;
                if(array[S].Key.CompareTo(c) < 0)
                    L = S + 1;
                else
                    P = S;
            }

			if (L == array.Length || array [L].Key.CompareTo(c) != 0)
				L--;

            return L;
        }

		private static MinimizedDfa<TSymbol> HopcroftAlgorithm<TDfa, TDfaState, TSymbol> (TDfa dfa) 
			where TDfa : AbstractDfa<TDfaState, TSymbol> where TDfaState : AbstractDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
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

            ISet<LinkedList<TDfaState>> queue = new HashSet<LinkedList<TDfaState>>();

            foreach (var state in stateList) {
                var set = partition[state];
                if (state.Accepting != 0 && !queue.Contains(set))
					queue.Add (set);
			}
 
			while (queue.Count > 0) {
				var mainSet = queue.First();
				queue.Remove (mainSet);

				foreach (TSymbol c in alphabet)
				{
					var prevSet = new LinkedList<TDfaState>(); 

					foreach (var state in stateList) {
                        var deltaState = state.Transitions[SimpleBinarySearch<TDfaState, TSymbol> (state.Transitions, c)].Value; //or upperbound?

                        if(partition[deltaState] == mainSet) 
                            prevSet.AddFirst(state);
					}
						
					var setsPartition = partition.Refine (prevSet);

					foreach (var setPartition in setsPartition) {
                        if(setPartition.Difference.Count == 0 || setPartition.Intersection.Count == 0) {
                            continue;
                        }

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

        private static readonly MethodInfo MakeMinimizedProductDfaMethod;

        static DfaUtils()
        {
            MakeMinimizedProductDfaMethod = typeof(DfaUtils)
                .GetMethods()
                .First(mi => mi.Name == "MakeMinimizedProductDfa" && mi.GetGenericArguments().Length == 5);
        }

        public delegate uint AmbiguityHandler(uint leftState, uint rightState);

        private static ProductDfa<TSymbol> MakeProductDfa<TLastDfa, TLastDfaState, TNewDfa, TNewDfaState, TSymbol>(
            TLastDfa lastDfa, TNewDfa newDfa, AmbiguityHandler handler)
            where TLastDfa : AbstractDfa<TLastDfaState, TSymbol> where TLastDfaState : AbstractDfaState<TLastDfaState, TSymbol>
            where TNewDfa : AbstractDfa<TNewDfaState, TSymbol>
            where TNewDfaState : AbstractDfaState<TNewDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            var result = new ProductDfaBuilder<TLastDfaState, TNewDfaState, TSymbol>(handler).Build(lastDfa.Start,
                newDfa.Start);
            DfaStatesNotNullConcpetCheck<TSymbol>.CheckDfaStatesNotNull(result);
            return result;
        }

        public static MinimizedDfa<TSymbol> MakeMinimizedProductDfa
            <TLastDfa, TLastDfaState, TNewDfa, TNewDfaState, TSymbol>(TLastDfa lastDfa, TNewDfa newDfa,
                AmbiguityHandler handler)
            where TLastDfa : AbstractDfa<TLastDfaState, TSymbol>
            where TLastDfaState : AbstractDfaState<TLastDfaState, TSymbol>
            where TNewDfa : AbstractDfa<TNewDfaState, TSymbol>
            where TNewDfaState : AbstractDfaState<TNewDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            var result =
                MakeProductDfa<TLastDfa, TLastDfaState, TNewDfa, TNewDfaState, TSymbol>(lastDfa, newDfa, handler)
                    .Minimized<ProductDfa<TSymbol>, ProductDfaState<TSymbol>, TSymbol>();
            DfaStatesNotNullConcpetCheck<TSymbol>.CheckDfaStatesNotNull(result);
            return result;
        }

        public static IDfa<TSymbol> MakeMinimizedProductDfa<TSymbol>(IDfa<TSymbol> lastDfa, IDfa<TSymbol> newDfa,
            AmbiguityHandler handler)
            where TSymbol : IEquatable<TSymbol>, IComparable<TSymbol>
        {
            // This implementation assumes that all instances of IDfa are in fact instances of AbstractDfa
            var lastDfaType = lastDfa.GetType();
            var newDfaType = newDfa.GetType();
            var lastDfaStateType = lastDfa.Start.GetType();
            var newDfaStateType = newDfa.Start.GetType();

            return (IDfa<TSymbol>) MakeMinimizedProductDfaMethod
                .MakeGenericMethod(lastDfaType, lastDfaStateType, newDfaType, newDfaStateType, typeof (TSymbol))
                .Invoke(null, new object[] {lastDfa, newDfa, handler});
        }

        private struct ProductDfaBuilder<TLastDfaState, TNewDfaState, TSymbol>
            where TLastDfaState : AbstractDfaState<TLastDfaState, TSymbol>
            where TNewDfaState : AbstractDfaState<TNewDfaState, TSymbol>
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
                return new ProductDfa<TSymbol> {_start = GetProductState(lastDfaStart, newDfaStart)};
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
                    _accepting = _getProductAccepting(lastDfaState.Accepting, newDfaState.Accepting)
                };
                _productionMapping[statePack] = productState;
                // Przygotowuję przejścia
                var productTransitions = MakeProductTransitions(lastDfaState.Transitions, newDfaState.Transitions);
                // Zapisuję wygenerowane przejścia w stanie
                productState._transitions = productTransitions.ToArray();
                // Stan jest gotowy
                return productState;
            }

            private List<KeyValuePair<TSymbol, ProductDfaState<TSymbol>>> MakeProductTransitions(
                KeyValuePair<TSymbol, TLastDfaState>[] lastTransitions,
                KeyValuePair<TSymbol, TNewDfaState>[] newTransitions)
            {
                Debug.Assert(EqualityComparer<TSymbol>.Default.Equals(lastTransitions[0].Key, newTransitions[0].Key));
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

        private class ProductDfaState<TSymbol> : AbstractDfaState<ProductDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            internal uint _accepting;
            internal KeyValuePair<TSymbol, ProductDfaState<TSymbol>>[] _transitions;

            public override uint Accepting
            {
                get { return _accepting; }
            }

            public override KeyValuePair<TSymbol, ProductDfaState<TSymbol>>[] Transitions
            {
                get { return _transitions; }
            }

            internal static ProductDfaState<TSymbol> MakeDeadState()
            {
                var result = new ProductDfaState<TSymbol> {_accepting = 0};
                result._transitions = new[]
                {new KeyValuePair<TSymbol, ProductDfaState<TSymbol>>(default(TSymbol), result)};
                return result;
            }
        }

        private class ProductDfa<TSymbol> : AbstractDfa<ProductDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            internal ProductDfaState<TSymbol> _start;

            public override ProductDfaState<TSymbol> Start
            {
                get { return _start; }
            }
        }

        internal static MinimizedDfa<TSymbol> MakeEmptyLanguageDfa<TSymbol>()
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            var result = new ProductDfa<TSymbol> {_start = ProductDfaState<TSymbol>.MakeDeadState()}
                    .Minimized<ProductDfa<TSymbol>, ProductDfaState<TSymbol>, TSymbol>();
            DfaStatesNotNullConcpetCheck<TSymbol>.CheckDfaStatesNotNull(result);
            return result;

        }

        #endregion

        #region DfaConceptsChecks

        internal class DfaStatesNotNullConcpetCheck<TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            private readonly HashSet<IDfaState<TSymbol>> _visitedStates = new HashSet<IDfaState<TSymbol>>();

            private DfaStatesNotNullConcpetCheck()
            {
            }

            [Conditional("DEBUG")]
            internal static void CheckDfaStatesNotNull(IDfa<TSymbol> dfa)
            {
                new DfaStatesNotNullConcpetCheck<TSymbol>().Check(dfa.Start);
            }

            private void Check(IDfaState<TSymbol> state)
            {
                if (state == null)
                {
                    Debug.Fail("Found null state in DFA");
                }
                if (_visitedStates.Contains(state))
                    return;
                _visitedStates.Add(state);
                Debug.Assert(state.Transitions != null);
                Debug.Assert(state.Transitions.Count > 0);
                foreach (var transition in state.Transitions)
                {
                    Check(transition.Value);
                }
            }
        } 

        #endregion

        #region CompareDfa
        internal class CompareDfa<TDfa, TDfaState, TSymbol> 
            where TDfa : AbstractDfa<TDfaState, TSymbol> 
            where TDfaState : AbstractDfaState<TDfaState, TSymbol> 
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {

            private CompareDfa() { }

            private static Dictionary<Tuple<TDfaState, TDfaState>, int> visited = new Dictionary<Tuple<TDfaState, TDfaState>, int>();

            private bool CompareStates(TDfaState a, TDfaState b)
            {
                if (visited.ContainsKey(Tuple.Create(a, b))) return true;
                visited[Tuple.Create(a, b)] = 1;
                if (a == null || b == null) return false;
                if (a.Accepting != b.Accepting) return false;
                if (a.Transitions == null || b.Transitions == null) return false;
                if (a.Transitions.Length != b.Transitions.Length) return false;

                foreach (var transitionA in a.Transitions)
                {
                    bool statesAreEqual = false;
                    foreach (var transitionB in b.Transitions)
                    {
                        
                        if (transitionA.Key.Equals(transitionB.Key))
                        {
                            if (!CompareStates(transitionA.Value, transitionB.Value)) return false;
                            else
                            {
                                statesAreEqual = true;
                                break;
                            }
                        }
                    }
                    if (!statesAreEqual) return false;
                }
                visited[Tuple.Create(a, b)] = 2;
                return true;
            }

            internal static bool Compare(TDfa a, TDfa b)
            {
                return new CompareDfa<TDfa, TDfaState, TSymbol>().CompareStates(a.Start, b.Start);
            }
        }
        #endregion
    }
}