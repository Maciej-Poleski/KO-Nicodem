using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExEpsilon : RegEx
	{
		internal RegExEpsilon ()
		{
			this.TypeId = 0;
		}

		// 0 if other is RegExEpsilon
		// a < 0 otherwise
		// => RegExEpsilon is the 'smallest' one
		public override int CompareTo (RegEx other)
		{
			return TypeId - other.TypeId;
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

