using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public abstract class RegEx : IComparable<RegEx>, IEquatable<RegEx>
	{
		internal int TypeId { set; get; }

		#region IComparable implementation

		public virtual int CompareTo (RegEx other)
		{
			//Derived classes should override this method
			throw new NotImplementedException ();
		}

		#endregion

		#region IEquatable implementation

		public bool Equals (RegEx other)
		{
			// should be correlated with compareTo
			return CompareTo (other) == 0;
		}

		#endregion

		#region standard methods

		public override bool Equals (object obj)
		{
			var regex = obj as RegEx;
			return regex != null && Equals (regex);
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
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

