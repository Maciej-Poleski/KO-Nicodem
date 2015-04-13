using System;
using Nicodem.Lexer;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Nicodem.Parser
{
	public class DfaState<TSymbol>
		where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
	{
		public uint Accepting { get; private set; }
		public IReadOnlyList<KeyValuePair<TSymbol, DfaState<TSymbol>>> Transitions { get; private set; }

		public DfaState() { }
		public void Initialize(uint accepting, IReadOnlyList<KeyValuePair<TSymbol, DfaState<TSymbol>>> transitions)
		{
			if (Transitions != null) {
				throw new InvalidOperationException("DfaState may be initialized only once.");
			}
			Accepting = accepting;
			Transitions = transitions;
		}

		public DfaState<TSymbol> Transition(TSymbol symbol) {
			//return Transitions.FirstOrDefault(kv => kv.Key.Equals(symbol)).Value;
			throw new NotImplementedException();
		}
	}
}

