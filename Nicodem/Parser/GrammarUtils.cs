using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	public static class GrammarUtils<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
	{

		// Returns the list of all accepting states of an automaton.
		private static List<DfaState<TSymbol>> GetAllAcceptingStates(Dfa<TSymbol> dfa) 
		{
			var result = new List<DfaState<TSymbol>>();
			var queue = new Queue<DfaState<TSymbol>>();
			var visited = new HashSet<DfaState<TSymbol>>();
			queue.Enqueue(dfa.Start);

			while(queue.Count > 0) {
				var state = queue.Dequeue();
				if(state.Accepting > 0)
					result.Add(state);
				foreach(var kv in state.Transitions) {
					var nextState = kv.Value;
					if(!visited.Contains(nextState)) {
						visited.Add(nextState);
						queue.Enqueue(nextState);
					}
				}
			}
			return result;
		}

		// Computes predecessors of states in all the grammar's automatons.
		private static IDictionary<DfaState<TSymbol>,List<KeyValuePair<TSymbol, DfaState<TSymbol>>>> ComputePredecessors(IDictionary<TSymbol, Dfa<TSymbol>> automatons)
		{
			var predecessors = new Dictionary<DfaState<TSymbol>, List<KeyValuePair<TSymbol, DfaState<TSymbol>>>>();
			foreach(var dfa in automatons.Values) {
				var queue = new Queue<DfaState<TSymbol>>();
				var visited = new HashSet<DfaState<TSymbol>>();
				queue.Enqueue(dfa.Start);
				while(queue.Count > 0) {
					var state = queue.Dequeue();
					if (!predecessors.ContainsKey (state)) // each state should have, even empty, list of predecessors.
						predecessors [state] = new List<KeyValuePair<TSymbol, DfaState<TSymbol>>> ();
					foreach(var kv in state.Transitions) {
						var nextState = kv.Value;
						if (!predecessors.ContainsKey (nextState))
							predecessors [nextState] = new List<KeyValuePair<TSymbol, DfaState<TSymbol>>> ();
						predecessors[nextState].Add(new KeyValuePair<TSymbol, DfaState<TSymbol>>(kv.Key,state));
						if(!visited.Contains(nextState)) {
							visited.Add(nextState);
							queue.Enqueue(nextState);
						}
					}
				}
			}
			return predecessors;
		}

		// computes a dictionary which maps an accepting state to a symbol (nonterminal) whose DFA contains this state. 
		public static IDictionary<DfaState<TSymbol>, TSymbol> computeAccStateOwnerDictionary (IDictionary<TSymbol, Dfa<TSymbol>> automatons) {
			var resultDictionary = new Dictionary<DfaState<TSymbol>, TSymbol> ();
			foreach(var automaton in automatons) {
				var dfa = automaton.Value;
				var owner = automaton.Key;
				var queue = new Queue<DfaState<TSymbol>>();
				var visited = new HashSet<DfaState<TSymbol>>();
				queue.Enqueue(dfa.Start);
				while(queue.Count > 0) {
					var state = queue.Dequeue();
					if (state.Accepting > 0)
						resultDictionary [state] = owner;
					foreach(var kv in state.Transitions) {
						var nextState = kv.Value;
						if(!visited.Contains(nextState)) {
							visited.Add(nextState);
							queue.Enqueue(nextState);
						}
					}
				}
			}
			return resultDictionary;
		}

		// computes dictionary which for each symbol keeps a list of states s.t. there exits an edge labelled by this symbol to the given state.
		public static IDictionary<TSymbol, List<DfaState<TSymbol>>> computeTargetStatesDictionary (IDictionary<TSymbol, Dfa<TSymbol>> automatons){
			var resultDictionary = new Dictionary<TSymbol, List<DfaState<TSymbol>>> ();
			foreach(var dfa in automatons.Values) {
				var queue = new Queue<DfaState<TSymbol>>();
				var visited = new HashSet<DfaState<TSymbol>>();
				queue.Enqueue(dfa.Start);
				while(queue.Count > 0) {
					var state = queue.Dequeue();

					foreach(var kv in state.Transitions) {
						var nextState = kv.Value;
						var symbol = kv.Key;
						if (!resultDictionary.ContainsKey (symbol))
							resultDictionary [symbol] = new List<DfaState<TSymbol>> ();
						resultDictionary [symbol].Add (nextState);
						if(!visited.Contains(nextState)) {
							visited.Add(nextState);
							queue.Enqueue(nextState);
						}
					}
				}
			}
			return resultDictionary;
		}

		public static void ComputeTransitiveComplement<T>(IDictionary<T, ISet<T>> dict) 
		{
			// B \in D(A) => D(B) <= D(A)
			var change = true;
			while(change) {
				change = false;
				// diff between before-phase and after-phase sets
				var diff = new Dictionary<T, ISet<T>> ();

				foreach(var A in dict.Keys) {
					diff [A] = new HashSet<T> ();

					foreach(var B in dict[A]) {
						if (dict.ContainsKey (B)) {
							foreach (var x in dict[B]) {
								change |= !(dict [A].Contains (x) || diff [A].Contains (x));
								diff [A].Add (x);
							}
						}
					}
				}

				// update set
				foreach (var A in dict.Keys)
					foreach (var x in diff[A])
						dict [A].Add (x);
			}
		}

		public static ISet<TSymbol> ComputeNullable(IDictionary<TSymbol, Dfa<TSymbol>> automatons) 
		{
			// For productions A -> E startStatesLhs maps start state of automaton of E to A
			var startStatesLhs = new Dictionary<DfaState<TSymbol>, TSymbol>();
			// conditional stores states which will be enqueued as soon as T turns out to be nullable.
			var conditional = new Dictionary<TSymbol, List<DfaState<TSymbol>>>();
			var queue = new Queue<DfaState<TSymbol>>();
			var nullable = new HashSet<TSymbol>();

			foreach(var symbol in automatons.Keys){
				startStatesLhs.Add(automatons[symbol].Start, symbol);
			}

			// at the beginning enqueue all accepting states
			foreach(var dfa in automatons.Values){
				foreach(var accstate in GetAllAcceptingStates(dfa)){
					queue.Enqueue(accstate);
				}
			}

			var predecessors = ComputePredecessors(automatons);

			while(queue.Count > 0){
				var state = queue.Dequeue();
				// if it is a start state for the production A -> E
				// add A to nullable and enqueue all its related conditional states
				if(startStatesLhs.ContainsKey(state)){
					if(!nullable.Contains(startStatesLhs[state])){
						var lhsT = startStatesLhs[state];
						nullable.Add(lhsT);
						if(conditional.ContainsKey(lhsT)){
							foreach(var condState in conditional[lhsT]){
								queue.Enqueue(condState);
							}
						}
					}
				} else {
					// else, get the state's predecessors and enqueue all which have edge
					// to the state over a nullable symbol. Put the rest into the conditional lists.
					foreach(var kv in predecessors[state]){
						if(nullable.Contains(kv.Key))
							queue.Enqueue(kv.Value);
						else {
							if(!conditional.ContainsKey(kv.Key))
								conditional.Add(kv.Key, new List<DfaState<TSymbol>>());
							conditional[kv.Key].Add(kv.Value);
						}
					}
				}
			}
			return nullable;
		}

        public static Dictionary<TSymbol, ISet<TSymbol>> ComputeFollow (IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable, FirstSet<TSymbol> first)
        {
            var follow = new Dictionary<TSymbol, ISet<TSymbol>>();
            var queue = new Queue<DfaState<TSymbol>>();
            // for accepting state s it stores A - nonterminal whose automaton contains s.
            var lhsForAccStates = new Dictionary<DfaState<TSymbol>, TSymbol>();
            var predecessors = ComputePredecessors(automatons);
            var acceptingStates = new Dictionary<Dfa<TSymbol>,List<DfaState<TSymbol>>> ();

            foreach (var dfa in automatons.Values)
                acceptingStates [dfa] = GetAllAcceptingStates (dfa);

            foreach(var kv in automatons) {
                var dfa = kv.Value;
                var lhsT = kv.Key;
                foreach(var accstate in acceptingStates[dfa])
                    lhsForAccStates[accstate] = lhsT;
            }

            bool changes = true;
            // While there are any changes
            while(changes) {
                changes = false;
                // Enqueue all accepting states from all automatons
                foreach(var dfa in automatons.Values)
                    foreach(var accstate in acceptingStates[dfa])
                        queue.Enqueue(accstate);

                while(queue.Count > 0) {
                    var state = queue.Dequeue();
                    // compute Follow set in the state i.e. the set-sum of 
                    // Follow sets of all symbols from its outgoing edges.
                    var followSetSum = new HashSet<TSymbol>();
                    foreach(var kv in state.Transitions) {
                        // symbol on the outgoing edge
                        var outT = kv.Key;
                        // copy all symbols from First set
                        foreach(var s in first[outT])
                            followSetSum.Add(s);
                        // if the outT is nullable, copy also all symbols from its Follow set.
                        if(nullable.Contains(outT) && follow.ContainsKey(outT))
                            foreach(var s in follow[outT])
                                followSetSum.Add(s);
                    }

                    // Additionally, if state is an accepting state in the dfa for production A -> dfa(E),
                    // we should add to followSetSum all the symbols from Follow(A).
                    if(state.Accepting > 0) {
                        // lhsT=A in production A->E
                        var lhsT = lhsForAccStates[state];
                        if(follow.ContainsKey(lhsT))
                            foreach(var s in follow[lhsT])
                                followSetSum.Add(s);
                    }

                    // For all symbols on ingoing edges update their
                    // follow sets adding symbols from followSetSum.
                    foreach(var kv in predecessors[state]) {
                        // symbol on the ingoing edge
                        var inT = kv.Key;
                        if (!follow.ContainsKey (inT))
                            follow [inT] = new HashSet<TSymbol> ();

                        foreach(var s in followSetSum)
                            if(!follow[inT].Contains(s)) {
                                changes = true;
                                follow[inT].Add(s);
                            }

                        queue.Enqueue(kv.Value);
                    }
                }//end while queue not empty
            }//end while there are no changes
            return follow;
        }

		public static Dictionary<TSymbol, ISet<TSymbol>> _ComputeFollow (IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable, IDictionary<TSymbol, ISet<TSymbol>> first)
		{
			var follow = new Dictionary<TSymbol, ISet<TSymbol>>();
			var queue = new Queue<DfaState<TSymbol>>();
			// for accepting state s it stores A - nonterminal whose automaton contains s.
			var lhsForAccStates = new Dictionary<DfaState<TSymbol>, TSymbol>();
			var predecessors = ComputePredecessors(automatons);
			var acceptingStates = new Dictionary<Dfa<TSymbol>,List<DfaState<TSymbol>>> ();

			foreach (var dfa in automatons.Values)
				acceptingStates [dfa] = GetAllAcceptingStates (dfa);

			foreach(var kv in automatons) {
				var dfa = kv.Value;
				var lhsT = kv.Key;
				foreach(var accstate in acceptingStates[dfa])
					lhsForAccStates[accstate] = lhsT;
			}

			bool changes = true;
			// While there are any changes
			while(changes) {
				changes = false;
				// Enqueue all accepting states from all automatons
				foreach(var dfa in automatons.Values)
					foreach(var accstate in acceptingStates[dfa])
						queue.Enqueue(accstate);

				while(queue.Count > 0) {
					var state = queue.Dequeue();
					// compute Follow set in the state i.e. the set-sum of 
					// Follow sets of all symbols from its outgoing edges.
					var followSetSum = new HashSet<TSymbol>();
					foreach(var kv in state.Transitions) {
						// symbol on the outgoing edge
						var outT = kv.Key;
						// copy all symbols from First set
						if(first.ContainsKey(outT))
							foreach(var s in first[outT])
								followSetSum.Add(s);
						// if the outT is nullable, copy also all symbols from its Follow set.
						if(nullable.Contains(outT) && follow.ContainsKey(outT))
							foreach(var s in follow[outT])
								followSetSum.Add(s);
					}

					// Additionally, if state is an accepting state in the dfa for production A -> dfa(E),
					// we should add to followSetSum all the symbols from Follow(A).
					if(state.Accepting > 0) {
						// lhsT=A in production A->E
						var lhsT = lhsForAccStates[state];
						if(follow.ContainsKey(lhsT))
							foreach(var s in follow[lhsT])
								followSetSum.Add(s);
					}

					// For all symbols on ingoing edges update their
					// follow sets adding symbols from followSetSum.
					foreach(var kv in predecessors[state]) {
						// symbol on the ingoing edge
						var inT = kv.Key;
						if (!follow.ContainsKey (inT))
							follow [inT] = new HashSet<TSymbol> ();

						foreach(var s in followSetSum)
							if(!follow[inT].Contains(s)) {
								changes = true;
								follow[inT].Add(s);
							}

						queue.Enqueue(kv.Value);
					}
				}//end while queue not empty
			}//end while there are no changes
			return follow;
		}

        // FIRST(A) = { B \in Sigma : A ->* B\beta }
        public static FirstSet<TSymbol> ComputeFirst(IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable)
        {
            return new FirstSet<TSymbol>(automatons, nullable);
        }

		// FIRST(A) = { B \in Sigma : A ->* B\beta }
		public static IDictionary<TSymbol, ISet<TSymbol>> _ComputeFirst(IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable)
		{
			// declare local vars
			var first = new Dictionary<TSymbol, ISet<TSymbol>>();

			// begin with FIRST(A) := { A } foreach A
			foreach(var A in automatons.Keys) {
				first [A] = new HashSet<TSymbol> ();
				first [A].Add (A);
			}

			// foreach production A -> E
			// where A - symbol, E - automata
			foreach(var symbol in automatons.Keys) {
				var automata = automatons [symbol];

				// perform BFS from automata startstate
				// using only edges labeled by symbols from nullable set
				var Q = new Queue<DfaState<TSymbol>>();
				var visited = new HashSet<DfaState<TSymbol>> ();

				Q.Enqueue (automata.Start);
				visited.Add (automata.Start);

				while(Q.Count > 0) {
					var state = Q.Dequeue();
					// check all edges
					foreach(var transition in state.Transitions) {
						// add label to FIRST(symbol)
						first [symbol].Add (transition.Key);
						// if label is in NULLABLE set continue BFS using this edge
						if (nullable.Contains (transition.Key) && !visited.Contains (transition.Value)) {
							Q.Enqueue (transition.Value);
							visited.Add (transition.Value);
						}
					}
				}
			}

			ComputeTransitiveComplement (first);
			return first;
		}


        private static bool DepthFirstSearchOnSymbol(TSymbol symbol, Dictionary<DfaState<TSymbol>, int> color, IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable)
        {
            if (!automatons.ContainsKey(symbol))
                return false;
            var automata = automatons[symbol];
            return DepthFirstSearchOnState(automata.Start, color, automatons, nullable);
        }

        private static bool DepthFirstSearchOnState(DfaState<TSymbol> state, Dictionary<DfaState<TSymbol>, int> color, IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable)
        {

            //check if is visited or not
            if (color.ContainsKey(state))
            {
                if (color[state] == 1)
                    return true;
                else
                    return false;
            }

            color[state] = 1;

            foreach (var transition in state.Transitions)
            {
                //for nullable keys go on the same automata
                if (nullable.Contains(transition.Key))
                {
                    if (DepthFirstSearchOnState(transition.Value, color, automatons, nullable))
                        return true;
                }

                //for every key go on symbol
                if (DepthFirstSearchOnSymbol(transition.Key, color, automatons, nullable))
                    return true;
            }


            color[state] = 2;

            return false;
        }

        /// <summary>
        /// Function check presence of left recursion
        /// </summary>
        /// <param name="automatons">Automatons for grammatic</param>
        /// <param name="nullable">Nullable set in grammatic</param>
        /// <returns></returns>
        public static bool HasLeftRecursion(IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable)
        {
            //check if is nullable available
            if (nullable == null)
                throw new ArgumentNullException("Can not check left recursion without knowledge about Nullable");

            //check if there is at least one automaton
            if (automatons == null)
                throw new ArgumentNullException("Can not check left recursion on null automaton");

            //color of state white(0)/gray(1)/black(2)
            var color = new Dictionary<DfaState<TSymbol>, int>();

            //try to search on every symbol
            foreach (var symbol in automatons.Keys)
            {
                if (DepthFirstSearchOnSymbol(symbol, color, automatons, nullable))
                    return true;
            }

            return false;
        }
			
	}
}

