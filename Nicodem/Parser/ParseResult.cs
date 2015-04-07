using System;
using Nicodem.Core;

namespace Nicodem.Parser
{

	internal struct ParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
	{

		public IParseTree<TSymbol> Tree { get; private set; } 
		public MemoizedInput<IParseTree<TSymbol>>.Iterator Iterator { get; private set; }
		private bool _ok;

		public ParseResult(IParseTree<TSymbol> tree, MemoizedInput<IParseTree<TSymbol>>.Iterator iterator, bool ok = true)
			: this()
		{
			Tree = tree;
			Iterator = iterator;
			_ok = ok;
		}

		public static implicit operator bool(ParseResult<TSymbol> result)
		{
			return result._ok;
		}
	}
}

