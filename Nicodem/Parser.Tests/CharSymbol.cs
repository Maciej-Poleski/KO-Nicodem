using System;

namespace Nicodem.Parser.Tests
{
	internal struct CharSymbol : ISymbol<CharSymbol>
	{
		internal char C { get; private set; }

		internal CharSymbol(char c)
			: this()
		{
			C = c;
		}

		#region IEquatable implementation

		public bool Equals(CharSymbol other)
		{
			return C == other.C;
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(CharSymbol other)
		{
			return C.CompareTo(other.C);
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

