using System;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nicodem.Core;
using Nicodem.Source;

namespace Nicodem.Parser
{
	public static class ParserUtils<TSymbol>
		where TSymbol : ISymbol<TSymbol>
	{
		internal static TSymbol GetEOF()
		{
			if(typeof(TSymbol).GetField("EOF") != null) {

				return Expression.Lambda<Func<TSymbol>>(Expression.Field(null, typeof(TSymbol), "EOF")).Compile()();
			} else if(typeof(TSymbol).GetProperty("EOF") != null) {

				return Expression.Lambda<Func<TSymbol>>(Expression.Property(null, typeof(TSymbol), "EOF")).Compile()();
			} else {
            	throw new ArgumentException(String.Format("There is no implemented static field EOF in {0}", typeof(TSymbol)));
			}
		}

		internal static ParseResult<TSymbol> Convert(ItParseResult<TSymbol> result) 
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

		public static string PrepareErrorMessage(Error<TSymbol> err)
		{
			var sb = new StringBuilder();
			// check if EOF
			if(err.Fragment == null) {
				sb.Append(String.Format("Could not parse symbol: {0} at the End of File\n", err.Symbol));
				return sb.ToString();
			}

			sb.Append(String.Format("Could not parse symbol: {0} in {1}\n", err.Symbol, err.Fragment.Origin));
			var orig = err.Fragment.Origin;

			var begLine = err.Fragment.GetBeginOriginPosition().LineNumber;
			var endLine = err.Fragment.GetBeginOriginPosition().LineNumber;

			int oneBefore = begLine - 1;
			int oneAfter = endLine + 1;

			if(oneBefore >= 1) {

				sb.Append("\t..........\n");
				var before = orig.GetLine(oneBefore);
				sb.Append(String.Format("{0}\t{1}\n", oneBefore, before));
			}

			sb.Append(String.Format("{0}\t{1}\n", begLine, orig.GetLine(begLine)));
			sb.Append('\t');
			sb.Append((new SourceDiagnostic()).GetFragmentInLine(err.Fragment));
			sb.Append('\n');

			try {
				var next = orig.GetLine(oneAfter);
				sb.Append(String.Format("{0}\t{1}\n", oneAfter, next));
				sb.Append("\t..........\n");
			} catch {
				// ...
			}

			return sb.ToString();
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

