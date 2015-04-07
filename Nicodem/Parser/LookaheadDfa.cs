using System;
using Nicodem.Lexer;
using Strilanc.Value;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nicodem.Parser
{
	public class LookaheadDfa<TSymbol> : Dfa<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		public ReadOnlyDictionary<uint, IEnumerable<May<TSymbol>>> Decisions { get; private set; }
		// Decisions[AcceptingState.Accepting] is the result of lookahead.

		public LookaheadDfa(DfaState<TSymbol> start,
			ReadOnlyDictionary<uint, IEnumerable<May<TSymbol>>> decisions):
		base(start)
		{ 
			Decisions = decisions;
		}
	}
}

