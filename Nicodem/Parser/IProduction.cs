using System;
using Nicodem.Lexer;

namespace Nicodem.Parser
{
	public interface IProduction
	{
		ISymbol Lhs { get; }
		
		RegEx<ISymbol> Rhs { get; }
	}
}

