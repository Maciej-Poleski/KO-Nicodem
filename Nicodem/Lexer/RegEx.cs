using System;

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
	}
}

