using System;

namespace Nicodem.Parser.Tests
{
	public struct StringSymbol : ISymbol<StringSymbol>
	{
		internal String S { get; private set; }
		internal StringSymbol (String s)
			: this()
		{
			S = s;
		}

        #region ISymbol implementation

        public string Description {
            get {
                throw new NotImplementedException();
            }
        }

        public bool IsTerminal {
            get {
                throw new NotImplementedException();
            }
        }

        #endregion
			
		public bool Equals(StringSymbol other)
		{
			return String.Equals(S, other.S);
		}
			
		public int CompareTo(StringSymbol other)
		{
			return String.Compare (S, other.S);
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

