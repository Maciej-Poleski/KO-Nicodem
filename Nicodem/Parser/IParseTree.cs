using System;

using Nicodem.Source;

namespace Nicodem.Parser
{
	public interface IParseTree<TSymbol> : IEquatable<IParseTree<TSymbol>> where TSymbol:ISymbol<TSymbol>
	{
		TSymbol Symbol { get; }
		IFragment Fragment { get; }
        string ToStringIndented(string indent);
	}
}
