using System;
using Nicodem.Lexer;

namespace Nicodem.Parser.Tests
{
	internal class StringProduction : Production<CharSymbol>
	{
		public StringProduction(char symbol, RegEx<char> production)
			: base(
				new CharSymbol(symbol),
				RegEx<char>.Convert<CharSymbol>(production, c => new CharSymbol(c)))
		{
		}

		public StringProduction(char symbol, string production)
			: base(
				new CharSymbol(symbol),
				RegEx<char>.Convert<CharSymbol>(RegExParser.Parse(production), c => new CharSymbol(c)))
		{
		}
	}
}

