using System;
using Nicodem.Lexer;

namespace Nicodem.Parser
{
	public class Production<TSymbol> where TSymbol : ISymbol<TSymbol>
	{

		public Production(TSymbol lhs, RegEx<TSymbol> rhs)
		{
			Lhs = lhs;
			Rhs = rhs;
		}

		public TSymbol Lhs { get; private set; }
		
		public RegEx<TSymbol> Rhs { get; private set; }
	}
}

