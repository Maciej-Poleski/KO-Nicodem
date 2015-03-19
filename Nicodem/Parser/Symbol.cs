using System;

namespace Nicodem.Parser
{
	public class Symbol : IComparable<Symbol>, IEquatable<Symbol>
	{
		public Symbol()
		{
		}

        public static Symbol EOF
        {
            get {
                throw new NotImplementedException();
            }
        }

		public int CompareTo(Symbol other)
		{
			throw new NotImplementedException();
		}

	    public bool Equals(Symbol other)
	    {
	        throw new NotImplementedException();
	    }
	}
}

