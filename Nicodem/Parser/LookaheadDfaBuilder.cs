using System;

namespace Nicodem.Parser
{
	public class LookaheadDfaBuilder<TSymbol> where TSymbol : class, ISymbol<TSymbol>
	{
		public LookaheadDfaBuilder()
		{
		}

		public LookaheadDfa<TSymbol> Build(Grammar<TSymbol> grammar)
		{
			throw new NotImplementedException();
		}
	}
}

