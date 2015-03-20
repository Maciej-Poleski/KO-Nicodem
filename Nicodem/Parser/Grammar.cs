using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	// TODO: This class should be immutable. Find a way to use IImmutableDictionary and IImmutableSet.
	public class Grammar<TProduction> where TProduction:IProduction
	{
		public Symbol Start { get; private set; }
		public IDictionary<Symbol, TProduction[]> Productions { get; private set; } 
		internal IDictionary<Symbol, Dfa<Symbol>> Automatons { get; private set; }
		internal IDictionary<uint, TProduction> WhichProduction { get; private set; } // accepting state marker -> production
		internal ISet<Symbol> Nullable { get; private set; }
		internal IDictionary<Symbol, ISet<Symbol>> First { get; private set; }
		internal IDictionary<Symbol, ISet<Symbol>> Follow { get; private set; }
		IDictionary<Dfa<Symbol>, List<DfaState<Symbol>>> acceptingStates;
		IDictionary<DfaState<Symbol>,List<KeyValuePair<Symbol, DfaState<Symbol>>>> predecessors;
		
        // returns true if word belongs to the FIRST+ set for given term
		internal bool InFirstPlus(Symbol term, Symbol word)
		{
			return First[term].Contains(word) || (Nullable.Contains(term) && Follow[term].Contains(word));
		}

		internal bool IsTerminal(Symbol term)
		{
			return Productions[term].Length == 0;
		}
			
        public Grammar(IDictionary<Symbol, TProduction[]> productions)
		{
            Productions = productions;
			// Here we prepare automatons for each symbol.
			uint productionMarker = 1;
			foreach (var symbolProductions in Productions)
			{
				var automatons = new List<Dfa<Symbol>>();
				foreach (var production in symbolProductions.Value)
				{
					automatons.Add(Dfa<Symbol>.RegexDfa(production.Rhs, productionMarker));
                    WhichProduction[productionMarker] = production;
					productionMarker++;
				}
				Automatons[symbolProductions.Key] = Dfa<Symbol>.ProductDfa(automatons.ToArray());
			}
            // necessary for computing Nullable and Follow sets
			acceptingStates = new Dictionary<Dfa<Symbol>, List<DfaState<Symbol>>>();
		}

        private bool DepthFirstSearchOnSymbol(Symbol symbol, Dictionary<DfaState<Symbol>,int> color)
        {
            var automata = Automatons[symbol];
            return DepthFirstSearchOnState(automata.Start, color);
        }

        private bool DepthFirstSearchOnState(DfaState<Symbol> state, Dictionary<DfaState<Symbol>, int> color)
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
                if (Nullable.Contains(transition.Key))
                {
                    if (DepthFirstSearchOnState(transition.Value, color)) 
                        return true;
                }

                //for every key go on symbol
                if (DepthFirstSearchOnSymbol(transition.Key, color)) 
                    return true;
            }

            
            color[state] = 2;

            return false;
        }

		internal bool HasLeftRecursion()
		{
            //check if is nullable available
            if (Nullable == null)
                throw new ArgumentNullException("Can not check left recursion without knowledge about Nullable");

            //color of state white(0)/gray(1)/black(2)
            var color = new Dictionary<DfaState<Symbol>, int>();

            //try to search on every symbol
            foreach (var symbol in Automatons.Keys)
            {
                if (DepthFirstSearchOnSymbol(symbol, color))
                    return true;
            }

            return false;
		}

        // Returns the list of all accepting states of an automaton.
		private List<DfaState<Symbol>> GetAllAcceptingStates(Dfa<Symbol> dfa)
        {
            if(!acceptingStates.ContainsKey(dfa))
                acceptingStates[dfa] = new List<DfaState<Symbol>>();
            else
                return acceptingStates[dfa];
            
            var result = new List<DfaState<Symbol>>();
            var queue = new Queue<DfaState<Symbol>>();
            var visited = new HashSet<DfaState<Symbol>>();
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
            acceptingStates[dfa] = result;
            return result;
        }

        // Computes predecessors of states in all the grammar's automatons.
        private void ComputePredecessors()
        {
            predecessors = new Dictionary<DfaState<Symbol>, List<KeyValuePair<Symbol, DfaState<Symbol>>>>();
            foreach(var dfa in Automatons.Values) {
                var queue = new Queue<DfaState<Symbol>>();
                var visited = new HashSet<DfaState<Symbol>>();
                queue.Enqueue(dfa.Start);
                while(queue.Count > 0) {
                    var state = queue.Dequeue();
                    foreach(var kv in state.Transitions) {
                        var nextState = kv.Value;
                        predecessors[nextState].Add(new KeyValuePair<Symbol, DfaState<Symbol>>(kv.Key,state));
                        if(!visited.Contains(nextState)) {
                            visited.Add(nextState);
                            queue.Enqueue(nextState);
                        }
                    }
                }
            }
        }

        internal void ComputeNullable()
        {
            // For productions A -> E startStatesLhs maps start state of automaton of E to A
            var startStatesLhs = new Dictionary<DfaState<Symbol>, Symbol>();
            // conditional stores states which will be enqueued as soon as Symbol turns out to be nullable.
            var conditional = new Dictionary<Symbol, List<DfaState<Symbol>>>();
            var queue = new Queue<DfaState<Symbol>>();

            foreach(var symbol in Automatons.Keys)
            {
                startStatesLhs.Add(Automatons[symbol].Start, symbol);
            }

            // at the beginning enqueue all accepting states
            foreach(var dfa in Automatons.Values)
            {
                foreach(var accstate in GetAllAcceptingStates(dfa))
                {
                    queue.Enqueue(accstate);
                }
            }

            if(predecessors==null)ComputePredecessors();

            while(queue.Count > 0)
            {
                var state = queue.Dequeue();
                // if it is a start state for the production A -> E
                // add A to nullable and enqueue all its related conditional states
                if(startStatesLhs.ContainsKey(state))
                {
                    if(!Nullable.Contains(startStatesLhs[state]))
                    {
                        var lhsSymbol = startStatesLhs[state];
                        Nullable.Add(lhsSymbol);
                        foreach(var condState in conditional[lhsSymbol])
                        {
                            queue.Enqueue(condState);

                        }
                    }
                } else {
                // else, get the state's predecessors and enqueue all which have edge
                // to the state over a nullable symbol. Put the rest into the conditional lists.
                    foreach(var kv in predecessors[state])
                    {
                        if(Nullable.Contains(kv.Key))
                            queue.Enqueue(kv.Value);
                        else {
                            if(!conditional.ContainsKey(kv.Key))
                                conditional.Add(kv.Key, new List<DfaState<Symbol>>());
                            conditional[kv.Key].Add(kv.Value);
                        }
                    }
                }
            }
        }

        private void ComputeFollow()
        {
            var follow = new Dictionary<Symbol, ISet<Symbol>>();
            var queue = new Queue<DfaState<Symbol>>();
            // stores (lhs symbol A, s) of production A -> dfa(E) where s is accepting state in dfa(E).
            var lhsForAccStates = new Dictionary<DfaState<Symbol>, Symbol>();
            if(predecessors==null)ComputePredecessors();
            // at the beginning enqueue all accepting states
            foreach(var kv in Automatons) {
                var dfa = kv.Value;
                var lhsSymbol = kv.Key;
                foreach(var accstate in GetAllAcceptingStates(dfa))
                    lhsForAccStates[accstate] = lhsSymbol;
            }

            bool changes = true;
            // While there are any changes
            while(changes) {
                changes = false;
                // Enqueue all accepting states from all automatons
                foreach(var dfa in Automatons.Values)
                    foreach(var accstate in GetAllAcceptingStates(dfa))
                        queue.Enqueue(accstate);
                   
                while(queue.Count > 0) {
                    var state = queue.Dequeue();
                    // compute Follow set in the state i.e. the set-sum of 
                    // Follow sets of all symbols from its outgoing edges.
                    var followSetSum = new HashSet<Symbol>();
                    foreach(var kv in state.Transitions){
                        // symbol on the outgoing edge
                        var outSymbol = kv.Key;
                        // copy all symbols from First set
                        if(First.ContainsKey(outSymbol))
                            foreach(var s in First[outSymbol])
                                followSetSum.Add(s);
                        // if the outSymbol is nullable, copy also all symbols from its Follow set.
                        if(Nullable.Contains(outSymbol) && follow.ContainsKey(outSymbol))
                            foreach(var s in follow[outSymbol])
                                followSetSum.Add(s);
                    }

                    // Additionally, if state is an accepting state in the dfa for production A -> dfa(E),
                    // we should add to followSetSum all the symbols from Follow(A).
                    if(state.Accepting > 0) {
                        // lhsSymbol=A in production A->E
                        var lhsSymbol = lhsForAccStates[state];
                        if(follow.ContainsKey(lhsSymbol))
                            foreach(var s in follow[lhsSymbol])
                                followSetSum.Add(s);
                    }
                        
                    // For all symbols on ingoing edges update their
                    // follow sets adding symbols from followSetSum.
                    foreach(var kv in predecessors[state]) {
                        // symbol on the ingoing edge
                        var inSymbol = kv.Key;
                        // if localChange = true, i.e. follow set of the inSymbol
                        // has been enlarged, a predecessor state is enqueued. 
                        var localChange = false;
                        if(!follow.ContainsKey(inSymbol))
                            follow[inSymbol] = new HashSet<Symbol>();
                        foreach(var s in followSetSum)
                            if(!follow[inSymbol].Contains(s)) {
                                changes = true;
                                localChange = true;
                                follow[inSymbol].Add(s);
                            }
                        if(localChange)
                            queue.Enqueue(kv.Value);
                    }
                }//end while queue not empty
            }//end while there are no changes
            Follow = follow;
        }

        // FIRST(A) = { B \in Sigma : A ->* B\beta }
        private void ComputeFirst()
        {
            // preconditions (check if Nullable is already computed)
            if(Nullable == null)
                throw new ArgumentNullException("Cannot invoke 'First' method before 'Nullable'");

            // declare local vars
            var first = new Dictionary<Symbol, ISet<Symbol>>();

            // begin with FIRST(A) := { A } foreach A
            foreach(var A in Automatons.Keys) {
                first[A].Add(A);
            }

            // foreach production A -> E
            // where A - symbol, E - automata
            foreach(var symbol in Automatons.Keys) {
                var automata = Automatons[symbol];

                // perform BFS from automata startstate
                // using only edges labeled by symbols from nullable set
                var Q = new Queue<DfaState<Symbol>>();
                Q.Enqueue(automata.Start);

                while(Q.Count > 0) {
                    var state = Q.Dequeue();

                    // check all edges
                    foreach(var transition in state.Transitions) {

                        // add label to FIRST(symbol)
                        first[symbol].Add(transition.Key);

                        // if label is in NULLABLE set continue BFS using this edge
                        if(Nullable.Contains(transition.Key))
                            Q.Enqueue(transition.Value);
                    }
                }
            }

            // B \in FIRST(A) => FIRST(B) <= FIRST(A)
            var change = true;
            while(change) {
                change = false;

                foreach(var A in first.Keys) {
                    foreach(var B in first[A]) {

                        foreach(var x in first[B]) {
                            change |= !first[A].Contains(x);
                            first[A].Add(x);
                        }

                    }
                }
            }

            // set class immutables
            First = first;
        }

	}
}

