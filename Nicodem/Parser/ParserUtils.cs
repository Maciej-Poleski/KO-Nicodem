using System;

namespace Nicodem.Parser
{
	internal static class ParserUtils
	{
		// returns equivalent grammar with only single production for every non terminal symbol
		internal static Grammar<TProduction> SimplifyGrammar<TProduction>(Grammar<TProduction> grammar)
				where TProduction : IProduction
		{
			throw new NotImplementedException();
		}
	}
}

