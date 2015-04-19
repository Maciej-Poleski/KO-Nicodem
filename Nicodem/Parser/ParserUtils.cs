using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nicodem.Core;

namespace Nicodem.Parser
{
	internal static class ParserUtils<TSymbol>
		where TSymbol : ISymbol<TSymbol>
	{
		public static TSymbol GetEOF()
		{
			if(typeof(TSymbol).GetField("EOF") != null) {

				return Expression.Lambda<Func<TSymbol>>(Expression.Field(null, typeof(TSymbol), "EOF")).Compile()();
			} else if(typeof(TSymbol).GetProperty("EOF") != null) {

				return Expression.Lambda<Func<TSymbol>>(Expression.Property(null, typeof(TSymbol), "EOF")).Compile()();
			} else {
            	throw new ArgumentException(String.Format("There is no implemented static field EOF in {0}", typeof(TSymbol)));
			}
		}
	}

    internal struct InputPosition<TSymbol> where TSymbol : ISymbol<TSymbol>
    {
        public MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator Iterator { get; private set; }
        public int InputOption { get; private set; }
        public bool BacktrackableInput { get; private set; }

        public InputPosition(MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator iterator, int inputOption, bool backtrackableInput = false)
            : this()
        {
            Iterator = iterator;
            InputOption = inputOption;
            BacktrackableInput = backtrackableInput;
        }
    }

    internal struct ParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
    {

        public IParseTree<TSymbol> Tree { get; private set; } 
        public InputPosition<TSymbol> Position { get; private set; }
        private bool _ok;

        public ParseResult(IParseTree<TSymbol> tree, InputPosition<TSymbol> position, bool ok = true)
            : this()
        {
            Tree = tree;
            Position = Position;
            _ok = ok;
        }

        public static implicit operator bool(ParseResult<TSymbol> result)
        {
            return result._ok;
        }
    }
}

