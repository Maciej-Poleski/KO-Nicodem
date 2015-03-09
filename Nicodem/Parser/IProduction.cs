using System;
using Nicodem.Lexer;

namespace Nicodem.Parser
{
	public interface IProduction
	{
		Symbol Lhs { get; }
		// TODO: RegEx<Symbol>
		RegEx Rhs { get; }
	}
}

