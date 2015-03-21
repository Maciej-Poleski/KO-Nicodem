using System;
using Nicodem.Lexer;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	public class DfaState<TSymbol> 
		where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
	{
		private readonly DFAState<TSymbol> wrappedState;

		public DfaState(DFAState<TSymbol> wrappedState)
		{
			this.wrappedState = wrappedState;
			Transitions = new KeyValuePair<TSymbol, DfaState<TSymbol>>[wrappedState.Transitions.Length];
			for (int i = 0; i < wrappedState.Transitions.Length; i++) {
				Transitions[i] = new KeyValuePair<TSymbol, DfaState<TSymbol>>(
					wrappedState.Transitions[i].Key,
					new DfaState<TSymbol>(wrappedState.Transitions[i].Value)
				);
			}
		}

		public uint Accepting
		{
			get {
				return wrappedState.Accepting;
			}
		}

		public KeyValuePair<TSymbol, DfaState<TSymbol>>[] Transitions { get; private set; }
	}
}

