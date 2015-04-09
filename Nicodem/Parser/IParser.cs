using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	public interface IParser<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		IParseTree<TSymbol> Parse(IEnumerable<IParseTree<TSymbol>> word);
	}
}

