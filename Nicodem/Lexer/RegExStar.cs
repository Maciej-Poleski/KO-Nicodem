using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExStar : RegEx
	{
		public RegEx Regex { private set; get; }

		internal RegExStar ( RegEx regex )
		{
			this.TypeId = 2;
			this.Regex = regex;
		}

		// typeid diff if other is not star
		// compareTo( Regex, other.Regex )  otherwise
		public override int CompareTo (RegEx other)
		{
			var diff = TypeId - other.TypeId;
			if (diff != 0) {
				Console.WriteLine (diff);
				return diff;
			}

			var star = other as RegExStar;
			return Regex.CompareTo (star.Regex);
		}

		public override bool HasEpsilon()
		{
			return true;
		}

		public override IEnumerable<Char> DerivChanges()
		{
			return Regex.DerivChanges();
		}

		public override RegEx Derivative(Char c)
		{
			return RegExFactory.Concat(Regex.Derivative(c), RegExFactory.Star(Regex));
		}
	}
}

