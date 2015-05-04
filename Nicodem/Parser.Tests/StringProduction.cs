using System;
using Nicodem.Lexer;

namespace Nicodem.Parser.Tests
{
	internal class StringProduction : IProduction<CharSymbol>
	{

		public CharSymbol Lhs { get; private set; }
		public RegEx<CharSymbol> Rhs { get; private set; }

		public StringProduction(char symbol, RegEx<char> production)
		{
			Lhs = new CharSymbol(symbol);
			Rhs = RegEx<char>.Convert<CharSymbol>(production, c => new CharSymbol(c));
		}

		public StringProduction(char symbol, string production)
		{
			Lhs = new CharSymbol(symbol);
			Rhs = RegEx<char>.Convert<CharSymbol>(RegExParser.Parse(production), c => new CharSymbol(c));
		}

		public StringProduction(CharSymbol symbol, string production)
		{
			Lhs = symbol;
			Rhs = RegEx<char>.Convert<CharSymbol>(RegExParser.Parse(production), c => new CharSymbol(c));
		}
	}
}

