using System;
using System.Linq.Expressions;

namespace Nicodem.Parser
{
	public static class ParserUtils<TSymbol>
		where TSymbol : ISymbol<TSymbol>
	{
		public static TSymbol GetEOF()
		{
            try
            {
				return Expression.Lambda<Func<TSymbol>>(Expression.Property(null, typeof(TSymbol), "EOF")).Compile()();
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(String.Format("There is no implemented static field MinValue in {0}", typeof(TSymbol)), e);
            }
		}
	}
}

