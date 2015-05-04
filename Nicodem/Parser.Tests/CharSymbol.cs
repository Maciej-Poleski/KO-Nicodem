using System;

namespace Nicodem.Parser.Tests
{
	internal struct CharSymbol : ISymbol<CharSymbol>
	{
		public char C { get; private set; }
		public string Name { get; private set; }

		// Every non terminal symbol should be an uppercase letter
		// All others denote terminals
		internal CharSymbol(char c, string name = "")
			: this()
		{
			C = c;
			Name = name;
		}

        #region ISymbol implementation

        public string Description {
            get {
				return "CharSymbol-" + C.ToString();
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
			return C.Equals(other.C);
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

