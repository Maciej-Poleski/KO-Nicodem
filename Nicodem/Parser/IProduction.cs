using System;
using Nicodem.Lexer;

namespace Nicodem.Parser
{
	public interface IProduction<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		TSymbol Lhs { get; }
		
		RegEx<TSymbol> Rhs { get; }
	}
}

