using System;
using Nicodem.Lexer;

namespace Nicodem.Parser
{
	public interface IProduction
	{
		Symbol Lhs { get; }
		
		RegEx<Symbol> Rhs { get; }
	}
}

