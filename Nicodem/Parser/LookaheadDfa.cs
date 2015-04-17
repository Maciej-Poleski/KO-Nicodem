using System;
using Nicodem.Lexer;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nicodem.Parser
{
	public class LookaheadDfa<TSymbol> : Dfa<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
    {
        // Decisions[AcceptingState.Accepting] is the result of lookahead.
		public ReadOnlyDictionary<uint, IEnumerable<TSymbol?>> Decisions { get; private set; }

		public LookaheadDfa(DfaState<TSymbol> start,
			ReadOnlyDictionary<uint, IEnumerable<TSymbol?>> decisions):
		base(start)
		{ 
			Decisions = decisions;
		}
	}
}

