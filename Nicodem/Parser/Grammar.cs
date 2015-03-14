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

        public Grammar(IDictionary<Symbol, TProduction[]> productions)
		{
            Productions = productions;
			// Here begins the computation of Automatons and ProductionMarkers.
			uint productionMarker = 1;
			foreach (var symbolProductions in Productions)
			{
				// TODO(guspiel): create a product DFA for each symbol.
				foreach (var production in symbolProductions.Value)
				{
					productionMarker++;
				}
			}
		}

		internal bool HasLeftRecursion()
		{
			throw new NotImplementedException();
		}

        internal List<DFAState<Symbol>> GetAllAcceptingStates(IDfa<DFAState<Symbol>, Symbol> automaton)
        {
            throw new NotImplementedException();
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
                    var predecessors = state.Predecessors;
                    foreach(var kv in predecessors)
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

	}
}

