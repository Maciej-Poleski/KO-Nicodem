using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExRange : RegEx
	{
		public char Character { private set; get; }

		internal RegExRange ( char c )
		{
			this.TypeId = 1;
			this.Character = c;
		}

		// typeid diff if other is not star
		// c - other.c  otherwise
		public override int CompareTo (RegEx other)
		{
			var diff = TypeId - other.TypeId;
			if (diff != 0)
				return diff;

			var range = other as RegExRange;
			return Character - range.Character;
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

