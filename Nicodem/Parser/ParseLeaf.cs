using System;

using Nicodem.Source;

namespace Nicodem.Parser
{
	internal class ParseLeaf<TProduction> : IParseTree<TProduction> where TProduction:IProduction
	{
		public ISymbol Symbol { get; private set; }
		public IFragment Fragment { get; private set; }

		public ParseLeaf(IFragment fragment, ISymbol symbol)
		{
			Fragment = fragment;
			Symbol = symbol;
		}
	}
}

