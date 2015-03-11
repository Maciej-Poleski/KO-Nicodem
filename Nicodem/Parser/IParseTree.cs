using System;

using Nicodem.Source.Tmp;

namespace Nicodem.Parser
{
	public interface IParseTree<TProduction> where TProduction:IProduction
	{
		Symbol Symbol { get; }
		IFragment Fragment { get; }
	}
}
