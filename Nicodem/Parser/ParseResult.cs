using System;
using System.Collections.Generic;
using Nicodem.Core;

namespace Nicodem.Parser
{

	internal struct ParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
	{

		public IParseTree<TSymbol> Tree { get; private set; } 
		public MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator Iterator { get; private set; }
        public int InputOption { get; private set; }
        public bool CanBacktrackOnInput { get; private set; }
		private bool _ok;

		public ParseResult(IParseTree<TSymbol> tree, MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator iterator, int inputOption, bool canBacktrackOnInput, bool ok = true)
			: this()
		{
			Tree = tree;
			Iterator = iterator;
            InputOption = inputOption;
            CanBacktrackOnInput = canBacktrackOnInput;
			_ok = ok;
		}

		public static implicit operator bool(ParseResult<TSymbol> result)
		{
			return result._ok;
		}
	}
}

