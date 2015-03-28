using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	public static class GrammarUtils
	{
		// Returns the list of all accepting states of an automaton.
		private static List<DfaState<ISymbol>> GetAllAcceptingStates(Dfa<ISymbol> dfa) 
		{
			var result = new List<DfaState<ISymbol>>();
			var queue = new Queue<DfaState<ISymbol>>();
			var visited = new HashSet<DfaState<ISymbol>>();
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
		private static IDictionary<DfaState<ISymbol>,List<KeyValuePair<ISymbol, DfaState<ISymbol>>>> ComputePredecessors(IDictionary<ISymbol, Dfa<ISymbol>> automatons)
		{
			var predecessors = new Dictionary<DfaState<ISymbol>, List<KeyValuePair<ISymbol, DfaState<ISymbol>>>>();
			foreach(var dfa in automatons.Values) {
				var queue = new Queue<DfaState<ISymbol>>();
				var visited = new HashSet<DfaState<ISymbol>>();
				queue.Enqueue(dfa.Start);
				while(queue.Count > 0) {
					var state = queue.Dequeue();
					foreach(var kv in state.Transitions) {
						var nextState = kv.Value;
						if (!predecessors.ContainsKey (nextState))
							predecessors [nextState] = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
						predecessors[nextState].Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(kv.Key,state));
						if(!visited.Contains(nextState)) {
							visited.Add(nextState);
							queue.Enqueue(nextState);
						}
					}
				}
			}
			return predecessors;
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

		public static ISet<ISymbol> ComputeNullable(IDictionary<ISymbol, Dfa<ISymbol>> automatons) 
		{
			// For productions A -> E startStatesLhs maps start state of automaton of E to A
			var startStatesLhs = new Dictionary<DfaState<ISymbol>, ISymbol>();
			// conditional stores states which will be enqueued as soon as T turns out to be nullable.
			var conditional = new Dictionary<ISymbol, List<DfaState<ISymbol>>>();
			var queue = new Queue<DfaState<ISymbol>>();
			var nullable = new HashSet<ISymbol>();

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
						foreach(var condState in conditional[lhsT]){
							queue.Enqueue(condState);
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
								conditional.Add(kv.Key, new List<DfaState<ISymbol>>());
							conditional[kv.Key].Add(kv.Value);
						}
					}
				}
			}
			return nullable;
		}

		public static Dictionary<ISymbol, ISet<ISymbol>> ComputeFollow (IDictionary<ISymbol, Dfa<ISymbol>> automatons, ISet<ISymbol> nullable, IDictionary<ISymbol, ISet<ISymbol>> first)
		{
			var follow = new Dictionary<ISymbol, ISet<ISymbol>>();
			var queue = new Queue<DfaState<ISymbol>>();
			// stores (lhs symbol A, s) of production A -> dfa(E) where s is accepting state in dfa(E).
			var lhsForAccStates = new Dictionary<DfaState<ISymbol>, ISymbol>();
			var predecessors = ComputePredecessors(automatons);
			// at the beginning enqueue all accepting states
			foreach(var kv in automatons) {
				var dfa = kv.Value;
				var lhsT = kv.Key;
				foreach(var accstate in GetAllAcceptingStates(dfa))
					lhsForAccStates[accstate] = lhsT;
			}

			bool changes = true;
			// While there are any changes
			while(changes) {
				changes = false;
				// Enqueue all accepting states from all automatons
				foreach(var dfa in automatons.Values)
					foreach(var accstate in GetAllAcceptingStates(dfa))
						queue.Enqueue(accstate);

				while(queue.Count > 0) {
					var state = queue.Dequeue();
					// compute Follow set in the state i.e. the set-sum of 
					// Follow sets of all symbols from its outgoing edges.
					var followSetSum = new HashSet<ISymbol>();
					foreach(var kv in state.Transitions){
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
						// if localChange = true, i.e. follow set of the inT
						// has been enlarged, a predecessor state is enqueued. 
						var localChange = false;
						if(!follow.ContainsKey(inT))
							follow[inT] = new HashSet<ISymbol>();
						foreach(var s in followSetSum)
							if(!follow[inT].Contains(s)) {
								changes = true;
								localChange = true;
								follow[inT].Add(s);
							}
						if(localChange)
							queue.Enqueue(kv.Value);
					}
				}//end while queue not empty
			}//end while there are no changes
			return follow;
		}


		// FIRST(A) = { B \in Sigma : A ->* B\beta }
		public static IDictionary<ISymbol, ISet<ISymbol>> ComputeFirst(IDictionary<ISymbol, Dfa<ISymbol>> automatons, ISet<ISymbol> nullable)
		{
			// declare local vars
			var first = new Dictionary<ISymbol, ISet<ISymbol>>();

			// begin with FIRST(A) := { A } foreach A
			foreach(var A in automatons.Keys) {
				first [A] = new HashSet<ISymbol> ();
				first [A].Add (A);
			}

			// foreach production A -> E
			// where A - symbol, E - automata
			foreach(var symbol in automatons.Keys) {
				var automata = automatons [symbol];

				// perform BFS from automata startstate
				// using only edges labeled by symbols from nullable set
				var Q = new Queue<DfaState<ISymbol>>();
				Q.Enqueue(automata.Start);

				while(Q.Count > 0) {
					var state = Q.Dequeue();
					// check all edges
					foreach(var transition in state.Transitions) {
						// add label to FIRST(symbol)
						first [symbol].Add (transition.Key);
						// if label is in NULLABLE set continue BFS using this edge
						if(nullable.Contains(transition.Key))
							Q.Enqueue(transition.Value);
					}
				}
			}

			/*
			// B \in FIRST(A) => FIRST(B) <= FIRST(A)
			var change = true;
			while(change) {
				change = false;
				// diff between before-phase and after-phase sets
				var diff = new Dictionary<ISymbol, ISet<ISymbol>> ();

				foreach(var A in first.Keys) {
					diff [A] = new HashSet<ISymbol> ();

					foreach(var B in first[A]) {
						if (first.ContainsKey (B)) {
							foreach (var x in first[B]) {
								change |= !(first [A].Contains (x) || diff [A].Contains (x));
								diff [A].Add (x);
							}
						}
					}
				}

				// update set
				foreach (var A in first.Keys)
					foreach (var x in diff[A])
						first [A].Add (x);
			}
			*/

			ComputeTransitiveComplement (first);
			return first;
		}
			
	}
}

