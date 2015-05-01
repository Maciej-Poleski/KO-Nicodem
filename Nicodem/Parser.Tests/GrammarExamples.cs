using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal enum GrammarType {
		LL1, 
		LL2, 
		OTHER
	}

	internal interface GrammarExample<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		Grammar<TSymbol> Grammar { get; }

		GrammarType Type { get; }
		IEnumerable<ParseLeaf<TSymbol>> ValidPrograms { get; }
		IEnumerable<ParseLeaf<TSymbol>> InvalidPrograms { get; }
	}
}

