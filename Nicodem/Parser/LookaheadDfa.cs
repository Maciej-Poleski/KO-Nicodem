using System;
using Nicodem.Lexer;
using Strilanc.Value;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nicodem.Parser
{
	public class LookaheadDfa : Dfa<ISymbol>
	{
		public ReadOnlyDictionary<uint, IEnumerable<May<ISymbol>>> Decisions { get; private set; }
		// Decisions[AcceptingState.Accepting] is the result of lookahead.

		public LookaheadDfa(DfaState<ISymbol> start,
			ReadOnlyDictionary<uint, IEnumerable<May<ISymbol>>> decisions):
		base(start)
		{ 
			Decisions = decisions;
		}
	}
}

