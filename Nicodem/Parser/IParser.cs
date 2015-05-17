using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	public interface IParser<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		ParseResult<TSymbol> Parse(IEnumerable<ParseLeaf<TSymbol>> word);
	}
}

