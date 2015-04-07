using System;

namespace Nicodem.Parser.Tests
{
	internal class CharSymbol : ISymbol<CharSymbol>
	{
		internal char C { get; private set; }

		internal CharSymbol(char c)
		{
			C = c;
		}

		#region IEquatable implementation

		public bool Equals(CharSymbol other)
		{
			return (other != null && C == other.C);
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(CharSymbol other)
		{
			return other != null ? C.CompareTo(other.C) : -1;
		}

		#endregion

		#region ISymbol implementation

		public static CharSymbol EOF {
			get {
				return new CharSymbol('$');
			}
		}

		public static CharSymbol MinValue {
			get {
				return new CharSymbol (Char.MinValue);
			}
		}
		#endregion
	}
}

