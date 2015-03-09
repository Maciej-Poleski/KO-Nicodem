using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	// TODO: This class should be immutable. Find a way to use IImmutableDictionary and IImmutableSet.
	public class Grammar<TProduction> where TProduction:IProduction
	{
		public Symbol Start { get; private set; }
		public IDictionary<Symbol, TProduction[]> Productions { get; private set; } 
		// TODO(guspiel): to be implemented when the DFA is ready
		//internal/private IDictionary<Symbol, IDfa<Symbol>> Automatons { get; private set; }
		internal ISet<Symbol> Nullable { get; private set; }
		internal IDictionary<Symbol, ISet<Symbol>> First { get; private set; }
		internal IDictionary<Symbol, ISet<Symbol>> Follow { get; private set; }

		public Grammar()
		{
		}

		internal bool HasLeftRecursion()
		{
			throw new NotImplementedException();
		}
	}
}

