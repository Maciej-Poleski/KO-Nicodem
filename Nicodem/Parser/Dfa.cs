using System;
using Nicodem.Lexer;

namespace Nicodem.Parser
{
	internal class Dfa<TSymbol> 
		where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
	{
		private DfaUtils.MinimizedDfa<TSymbol> wrappedDfa;

		public DfaState<TSymbol> Start
		{
			get {
				throw new NotImplementedException();
			}
		}

		private static uint maxAmbiguityHandler(uint leftState, uint rightState)
		{
			if (leftState < rightState) {
				return rightState;
			} else {
				return leftState;
			}
		}

		private Dfa(DfaUtils.MinimizedDfa<TSymbol> wrappedDfa)
		{
			this.wrappedDfa = wrappedDfa;
		}

		public static Dfa<TSymbol> RegexDfa(RegEx<TSymbol> RegEx, uint acceptingStateMarker)
		{
			if (acceptingStateMarker == 0) {
				throw new ArgumentOutOfRangeException();
			}
			IDfa<DFAState<TSymbol>, TSymbol> factorDfa = 
				(IDfa<DFAState<TSymbol>, TSymbol>) new RegExDfa<TSymbol>(RegEx, acceptingStateMarker);
			return new Dfa<TSymbol>(DfaUtils.MakeMinimizedProductDfa<
					IDfa<DFAState<TSymbol>, TSymbol>,
					DFAState<TSymbol>,	
					IDfa<DFAState<TSymbol>, TSymbol>,
					DFAState<TSymbol>,
					TSymbol
				>(factorDfa, factorDfa, new DfaUtils.AmbiguityHandler(maxAmbiguityHandler)));
		}

		public static Dfa<TSymbol> ProductDfa(Dfa<TSymbol>[] dfas)
		{
			if (dfas.Length == 0) {
				throw new ArgumentException();
			} else if (dfas.Length == 1) {
				return dfas[0];
			} else {
				DfaUtils.MinimizedDfa<TSymbol> cur = dfas[0].wrappedDfa;
				for (int i = 1; i < dfas.Length; i++) {
					cur = DfaUtils.MakeMinimizedProductDfa<
						DfaUtils.MinimizedDfa<TSymbol>,
					DfaUtils.MinimizedDfaState<TSymbol>,	
					DfaUtils.MinimizedDfa<TSymbol>,
					DfaUtils.MinimizedDfaState<TSymbol>,
					TSymbol
					>(cur, dfas[i].wrappedDfa, new DfaUtils.AmbiguityHandler(maxAmbiguityHandler));
				}
				return new Dfa<TSymbol>(cur);
			}
		}
	}
}

