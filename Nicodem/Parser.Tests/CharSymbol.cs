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

        #region ISymbol implementation

        public string Description {
            get {
				return "test_symbol: " + C.ToString();
            }
        }

        public bool IsTerminal {
            get {
				return !char.IsUpper(C);
            }
        }

        #endregion

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

        public override string ToString()
        {
            if (Char.IsSymbol(C) || Char.IsLetterOrDigit(C)) { 
                return "" + C;
            }
            else {
                return "_" + (int)C;
            }
        }
	}
}

