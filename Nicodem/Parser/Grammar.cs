using System;
using System.Collections.Generic;
using Nicodem.Lexer;

namespace Nicodem.Parser
{
	// TODO: This class should be immutable. Find a way to use IImmutableDictionary and IImmutableSet.
	public class Grammar<TProduction> where TProduction:IProduction
	{
		public Symbol Start { get; private set; }
		public IDictionary<Symbol, TProduction[]> Productions { get; private set; } 
		internal IDictionary<Symbol, IDfa<DFAState<Symbol>, Symbol>> Automatons { get; private set; }
		internal IDictionary<uint, TProduction> WhichProduction { get; private set; } // accepting state marker -> production
		internal ISet<Symbol> Nullable { get; private set; }
		internal IDictionary<Symbol, ISet<Symbol>> First { get; private set; }
		internal IDictionary<Symbol, ISet<Symbol>> Follow { get; private set; }

		// returns true if word belongs to the FIRST+ set for given term
		internal bool InFirstPlus(Symbol term, Symbol word)
		{
			return First[term].Contains(word) || (Nullable.Contains(term) && Follow[term].Contains(word));
		}

		internal bool IsTerminal(Symbol term)
		{
			return Productions[term].Length == 0;
		}

        private IDfa<DFAState<Symbol>, Symbol> ProductAutomaton(IDfa<DFAState<Symbol>, Symbol>[] automatons) {
            // TODO: It is implemented in Lexer but needs to be made public.
            throw new NotImplementedException();
        }

        public Grammar(IDictionary<Symbol, TProduction[]> productions)
		{
            Productions = productions;
			// Here we prepare automatons for each symbol.
			uint productionMarker = 1;
			foreach (var symbolProductions in Productions)
			{
                List<IDfa<DFAState<Symbol>, Symbol>> automatons = new List<IDfa<DFAState<Symbol>, Symbol>>();
				foreach (var production in symbolProductions.Value)
				{
                    automatons.Add(new RegExDfa<Symbol>(production.Rhs, productionMarker));
                    WhichProduction[productionMarker] = production;
					productionMarker++;
				}
                Automatons[symbolProductions.Key] = ProductAutomaton(automatons.ToArray());
			}
		}

        private bool DepthFirstSearchOnSymbol(Symbol symbol, Dictionary<DFAState<Symbol>,int> color)
        {
            var automata = Automatons[symbol];
            return DepthFirstSearchOnState(automata.Start, color);
        }

        private bool DepthFirstSearchOnState(DFAState<Symbol> state, Dictionary<DFAState<Symbol>, int> color)
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
            var color = new Dictionary<DFAState<Symbol>, int>();

            //try to search on every symbol
            foreach (var symbol in Automatons.Keys)
            {
                if (DepthFirstSearchOnSymbol(symbol, color))
                    return true;
            }

            return false;
		}

        // Returns the list of all accepting states of an automaton.
        private List<DFAState<Symbol>> GetAllAcceptingStates(IDfa<DFAState<Symbol>, Symbol> automaton)
        {
            var result = new List<DFAState<Symbol>>();
            var queue = new Queue<DFAState<Symbol>>();
            var visited = new HashSet<DFAState<Symbol>>();
            queue.Enqueue(automaton.Start);

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
        private Dictionary<DFAState<Symbol>,List<KeyValuePair<Symbol, DFAState<Symbol>>>> ComputePredecessors()
        {
            var predecessors = new Dictionary<DFAState<Symbol>, List<KeyValuePair<Symbol, DFAState<Symbol>>>>();
            foreach(var dfa in Automatons.Values) {
                var queue = new Queue<DFAState<Symbol>>();
                var visited = new HashSet<DFAState<Symbol>>();
                queue.Enqueue(dfa.Start);
                while(queue.Count > 0) {
                    var state = queue.Dequeue();
                    foreach(var kv in state.Transitions) {
                        var nextState = kv.Value;
                        predecessors[nextState].Add(new KeyValuePair<Symbol, DFAState<Symbol>>(kv.Key,state));
                        if(!visited.Contains(nextState)) {
                            visited.Add(nextState);
                            queue.Enqueue(nextState);
                        }
                    }
                }
            }
            return predecessors;
        }

        internal void ComputeNullable()
        {
            // For productions A -> E startStatesLhs maps start state of automaton of E to A
            var startStatesLhs = new Dictionary<DFAState<Symbol>, Symbol>();
            // conditional stores states which will be enqueued as soon as Symbol turns out to be nullable.
            var conditional = new Dictionary<Symbol, List<DFAState<Symbol>>>();
            var queue = new Queue<DFAState<Symbol>>();

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

            var predecessors = ComputePredecessors();

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
                                conditional.Add(kv.Key, new List<DFAState<Symbol>>());
                            conditional[kv.Key].Add(kv.Value);
                        }
                    }
                }
            }
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
                var Q = new Queue<DFAState<Symbol>>();
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

