using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	// TODO: This class should be immutable. Find a way to use IImmutableDictionary and IImmutableSet.
	public class Grammar<TProduction> where TProduction:IProduction
	{
		public ISymbol Start { get; private set; }
		public IDictionary<ISymbol, TProduction[]> Productions { get; private set; } 
		internal IDictionary<ISymbol, Dfa<ISymbol>> Automatons { get; private set; }
		internal IDictionary<uint, TProduction> WhichProduction { get; private set; } // accepting state marker -> production
		internal ISet<ISymbol> Nullable { get; private set; }
		internal IDictionary<ISymbol, ISet<ISymbol>> First { get; private set; }
		internal IDictionary<ISymbol, ISet<ISymbol>> Follow { get; private set; }
		
        // returns true if word belongs to the FIRST+ set for given term
		internal bool InFirstPlus(ISymbol term, ISymbol word)
		{
			return First[term].Contains(word) || (Nullable.Contains(term) && Follow[term].Contains(word));
		}

		internal bool IsTerminal(ISymbol term)
		{
			return Productions[term].Length == 0;
		}
			
        public Grammar(IDictionary<ISymbol, TProduction[]> productions)
		{
            Productions = productions;
			// Here we prepare automatons for each symbol.
			uint productionMarker = 1;
			foreach (var symbolProductions in Productions)
			{
				var automatons = new List<Lexer.DfaUtils.MinimizedDfa<ISymbol>>();
				foreach (var production in symbolProductions.Value)
				{
					automatons.Add(Dfa<ISymbol>.RegexDfa(production.Rhs, productionMarker));
                    WhichProduction[productionMarker] = production;
					productionMarker++;
				}
				Automatons[symbolProductions.Key] = Dfa<ISymbol>.ProductDfa(automatons.ToArray());
			}

			// set Nullable, First and Follow
			Nullable = GrammarUtils.ComputeNullable (Automatons);
			First = GrammarUtils.ComputeFirst (Automatons, Nullable);
			Follow = GrammarUtils.ComputeFollow (Automatons, Nullable, First);
		}

        private bool DepthFirstSearchOnSymbol(ISymbol symbol, Dictionary<DfaState<ISymbol>,int> color)
        {
            var automata = Automatons[symbol];
            return DepthFirstSearchOnState(automata.Start, color);
        }

        private bool DepthFirstSearchOnState(DfaState<ISymbol> state, Dictionary<DfaState<ISymbol>, int> color)
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
            var color = new Dictionary<DfaState<ISymbol>, int>();

            //try to search on every symbol
            foreach (var symbol in Automatons.Keys)
            {
                if (DepthFirstSearchOnSymbol(symbol, color))
                    return true;
            }

            return false;
		}
	}
}

