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

		public static ParseResult<TSymbol> Convert(ItParseResult<TSymbol> result) 
		{
			var okRes = result as ItOK<TSymbol>;
			if(okRes != null) {
				return new OK<TSymbol>(okRes.Tree);
			} else {
				var err = result as ItError<TSymbol>;
				var fragment = err.Iterator.Pos >= 0 ? err.Iterator.Current.Fragment : null;
				return new Error<TSymbol>(fragment, err.Symbol);
			}
		}
	}

	internal abstract class ItParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		public static implicit operator bool(ItParseResult<TSymbol> result)
        {
			return result is ItOK<TSymbol>;
        }
	}

	internal class ItOK<TSymbol> : ItParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
    {
        public MemoizedInput<ParseLeaf<TSymbol>>.Iterator Iterator { get; private set; }
        public IParseTree<TSymbol> Tree { get; private set; } 

		public ItOK(IParseTree<TSymbol> tree, MemoizedInput<ParseLeaf<TSymbol>>.Iterator iterator)
        {
            Tree = tree;
            Iterator = iterator;
        }
    }

	internal class ItError<TSymbol> : ItParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
    {
        public MemoizedInput<ParseLeaf<TSymbol>>.Iterator Iterator { get; private set; }
		public TSymbol Symbol { get; private set; }

		public ItError(MemoizedInput<ParseLeaf<TSymbol>>.Iterator iterator, TSymbol symbol)
        {
            Iterator = iterator;
			Symbol = symbol;
        }
    }
}

