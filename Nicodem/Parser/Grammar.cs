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

		public Grammar()
		{
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
	}
}

