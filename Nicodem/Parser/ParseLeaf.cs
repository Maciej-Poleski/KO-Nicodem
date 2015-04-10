using System;

using Nicodem.Source;

namespace Nicodem.Parser
{
	internal class ParseLeaf<TSymbol> : IParseTree<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		public TSymbol Symbol { get; private set; }
		public IFragment Fragment { get; private set; }

		public ParseLeaf(IFragment fragment, TSymbol symbol)
		{
			Fragment = fragment;
			Symbol = symbol;
		}
			
		public bool Equals(IParseTree<TSymbol> other){
			return (other is ParseLeaf<TSymbol> && Symbol.Equals(other.Symbol));
		}
	}
}

