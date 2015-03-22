using System;
using Nicodem.Lexer;

namespace Nicodem.Parser.Tests
{
	internal class StringProduction : IProduction
	{
		public StringProduction(char symbol, RegEx<char> production)
		{
			Lhs = new CharSymbol(symbol);
			Rhs = RegEx<char>.Convert<ISymbol>(production, c => new CharSymbol(c));
		}

		public StringProduction(char symbol, string production)
		{
			Lhs = new CharSymbol(symbol);
			Rhs = RegEx<char>.Convert<ISymbol>(RegExParser.Parse(production), c => new CharSymbol(c));
		}

		#region IProduction implementation

		public ISymbol Lhs { get; private set; }

		public RegEx<ISymbol> Rhs { get; private set; }

		#endregion
	}
}

