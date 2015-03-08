using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExEpsilon : RegEx
	{
		internal RegExEpsilon ()
		{
		}

		public override int CompareTo (RegEx other)
		{
			throw new NotImplementedException ();
		}

		public override bool HasEpsilon()
		{
			return true;
		}

		public override IEnumerable<Char> DerivChanges()
		{
			return new char[] { };
		}

		public override RegEx Derivative(Char c)
		{
			return RegExFactory.Empty();
		}
	}
}

