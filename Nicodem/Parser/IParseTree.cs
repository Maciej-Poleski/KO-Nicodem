using System;

using Nicodem.Source;

namespace Nicodem.Parser
{
	public interface IParseTree<TProduction> where TProduction:IProduction
	{
		ISymbol Symbol { get; }
		IFragment Fragment { get; }
	}
}
