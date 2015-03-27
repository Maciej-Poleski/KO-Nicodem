using System;

namespace Nicodem.Parser
{
	public class LookaheadDfaBuilder<TProduction> 
		where TProduction : IProduction
	{
		public LookaheadDfaBuilder()
		{
		}

		public LookaheadDfa Build(Grammar<TProduction> grammar)
		{
			throw new NotImplementedException();
		}
	}
}

