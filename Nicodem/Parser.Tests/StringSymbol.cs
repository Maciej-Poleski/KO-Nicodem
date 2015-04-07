using System;

namespace Nicodem.Parser.Tests
{
	public class StringSymbol : ISymbol<StringSymbol>
	{
		internal String S { get; private set; }
		internal StringSymbol (String s)
		{
			S = s;
		}
			
		public bool Equals(StringSymbol other)
		{
			return (other != null && String.Equals(S, other.S));
		}
			
		public int CompareTo(StringSymbol other)
		{
			if (other != null) {
				return String.Compare (S, other.S);
			}
			return -1;
		}

		public static StringSymbol EOF {
			get {
				return new StringSymbol ("EOF");
			}
		}
	
		public static StringSymbol MinValue {
			get {
				return new StringSymbol (Char.MinValue.ToString());
			}
		}

		public override string ToString ()
		{
			return "'" + S + "'";
		}
	}
}

