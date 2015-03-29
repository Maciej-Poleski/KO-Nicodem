using System;
using Nicodem.Core;

namespace Nicodem.Parser
{

	internal struct ParseResult<TProduction> where TProduction : IProduction
	{

		public IParseTree<TProduction> Tree { get; private set; } 
		public MemoizedInput<IParseTree<TProduction>>.Iterator Iterator { get; private set; }
		private bool _ok;

		public ParseResult(IParseTree<TProduction> tree, MemoizedInput<IParseTree<TProduction>>.Iterator iterator, bool ok = true)
			: this()
		{
			Tree = tree;
			Iterator = iterator;
			_ok = ok;
		}

		public static implicit operator bool(ParseResult<TProduction> result)
		{
			return result._ok;
		}
	}
}

