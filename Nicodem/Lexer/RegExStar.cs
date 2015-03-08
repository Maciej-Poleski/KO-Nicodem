using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExStar : RegEx
	{
		public RegEx Regex { private set; get; }

		internal RegExStar ( RegEx regex )
		{
			this.Regex = regex;
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
			return Regex.DerivChanges();
		}

		public override RegEx Derivative(Char c)
		{
			return RegExFactory.Concat(Regex.Derivative(c), RegExFactory.Star(Regex));
		}
	}
}

