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
        internal bool HasLeftRecursion { get; private set; }
		
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
            HasLeftRecursion = GrammarUtils.HasLeftRecursion(Automatons, Nullable);
		}

	}
}

