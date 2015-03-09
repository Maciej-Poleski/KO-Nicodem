using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExComplement : RegEx
	{
		public RegEx Regex { private set; get; }

		internal RegExComplement ( RegEx regex )
		{
			this.TypeId = 3;
			this.Regex = regex;
		}

		// typeid diff if other is not complement
		// compareTo( Regex, other.Regex ) otherwise
		public override int CompareTo (RegEx other)
		{
			var diff = TypeId - other.TypeId;
			if (diff != 0)
				return diff;

			var complement = other as RegExComplement;
			return Regex.CompareTo (complement.Regex);
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

