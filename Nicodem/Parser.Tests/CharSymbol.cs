using System;

namespace Nicodem.Parser.Tests
{
	internal class CharSymbol : ISymbol
	{
		internal char C { get; private set; }

		internal CharSymbol(char c)
		{
			C = c;
		}

		#region IEquatable implementation

		public bool Equals(ISymbol other)
		{
			if(other is CharSymbol && C == (other as CharSymbol).C) {
				return true;
			}
			return false;
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(ISymbol other)
		{
			var other_c = other as CharSymbol;
			return other_c != null ? C.CompareTo(other_c.C) : -1;
		}

		#endregion

		#region ISymbol implementation

		public ISymbol EOF {
			get {
				return new CharSymbol('^');
			}
		}

		#endregion
	}
}

