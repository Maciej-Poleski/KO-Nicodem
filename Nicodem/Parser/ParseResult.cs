using Nicodem.Source;

namespace Nicodem.Parser
{

	public abstract class ParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
	{
		public static implicit operator bool(ParseResult<TSymbol> result)
        {
			return result is OK<TSymbol>;
        }
	}

	public class OK<TSymbol> : ParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
    {
        public IParseTree<TSymbol> Tree { get; private set; } 

		public OK(IParseTree<TSymbol> tree)
        {
			Tree = tree;
        }
    }

	public class Error<TSymbol> : ParseResult<TSymbol> where TSymbol : ISymbol<TSymbol>
    {
		public IFragment Fragment { get; private set; }
		public TSymbol Symbol { get; private set; }

		public Error(IFragment fragment, TSymbol symbol)
        {
            Fragment = fragment;
			Symbol = symbol;
        }
    }
}

