using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public abstract class RegEx : IComparable<RegEx>
	{
		#region IComparable implementation
		public virtual int CompareTo (RegEx other)
		{
			//Derived classes should override this method
			throw new NotImplementedException ();
		}
		#endregion

        public virtual bool HasEpsilon()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<Char> DerivChanges()
        {
            throw new NotImplementedException();
        }

        public virtual RegEx Derivative(Char c)
        {
            throw new NotImplementedException();
        }
	}
}

