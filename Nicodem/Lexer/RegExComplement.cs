using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExComplement : RegEx
	{
		public RegEx Regex { private set; get; }

		internal RegExComplement ( RegEx regex )
		{
			this.Regex = regex;
		}

		public override int CompareTo (RegEx other)
		{
			throw new NotImplementedException ();
		}

		public override bool HasEpsilon()
		{
			return !Regex.HasEpsilon();
		}

		public override IEnumerable<Char> DerivChanges()
		{
			return Regex.DerivChanges();
		}

		public override RegEx Derivative(Char c)
		{
			return RegExFactory.Complement(Regex.Derivative(c));
		}
	}
}

