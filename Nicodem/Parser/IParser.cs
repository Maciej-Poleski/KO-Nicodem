using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	public interface IParser<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		IParseTree<TSymbol> Parse(IEnumerable<IEnumerable<IParseTree<TSymbol>>> word);
	}
}

