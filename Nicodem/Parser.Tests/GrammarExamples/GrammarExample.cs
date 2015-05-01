﻿using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal enum GrammarType {
		LL1, 
		LL2, 
		OTHER
	}

	internal interface GrammarExample<TSymbol> where TSymbol : struct, ISymbol<TSymbol> 
	{
		Grammar<TSymbol> Grammar { get; }

		GrammarType Type { get; }
		// Tuple<Description, Code>
		Tuple<String, IEnumerable<ParseLeaf<TSymbol>>>[] ValidPrograms { get; }
		Tuple<String, IEnumerable<ParseLeaf<TSymbol>>>[] InvalidPrograms { get; }
	}
}

