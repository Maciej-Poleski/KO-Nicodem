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
}

