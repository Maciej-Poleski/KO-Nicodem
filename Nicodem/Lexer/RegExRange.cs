using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExRange : RegEx
	{
		public char Character { private set; get; }

		internal RegExRange ( char c )
		{
			this.Character = c;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(pmikos)
			throw new System.NotImplementedException ();
		}

		public override bool HasEpsilon()
		{
			return false;
		}

		public override IEnumerable<Char> DerivChanges()
		{
			return new char[] { Character };
		}

		public override RegEx Derivative(Char c)
		{
			if (Character <= c)
			{
				return RegExFactory.Epsilon();
			}
			else
			{
				return RegExFactory.Empty();
			}
		}
	}
}

