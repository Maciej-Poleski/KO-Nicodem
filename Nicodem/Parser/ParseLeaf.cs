using System;

using Nicodem.Source;

namespace Nicodem.Parser
{
	internal class ParseLeaf<TProduction> : IParseTree<TProduction> where TProduction:IProduction
	{
		public Symbol Symbol { get; private set; }
		public IFragment Fragment { get; private set; }

		public ParseLeaf(IFragment fragment, Symbol symbol)
		{
			Fragment = fragment;
			Symbol = symbol;
		}
	}
}

