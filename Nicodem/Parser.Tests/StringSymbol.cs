using System;

namespace Nicodem.Parser.Tests
{
	public class StringSymbol : ISymbol
	{
		internal String S { get; private set; }
		internal StringSymbol (String s)
		{
			S = s;
		}
			
		public bool Equals(ISymbol other)
		{
			if (other is StringSymbol && String.Equals(S, (other as StringSymbol).S)) {
				return true;
			}
			return false;
		}
			
		public int CompareTo(ISymbol other)
		{
			if (other is StringSymbol) {
				return String.Compare (S, (other as StringSymbol).S);
			}
			return -1;
		}

		public ISymbol EOF {
			get {
				return new StringSymbol ("EOF");
			}
		}
	
		public ISymbol MinValue {
			get {
				return new StringSymbol (Char.MinValue.ToString());
			}
		}
	}
}

