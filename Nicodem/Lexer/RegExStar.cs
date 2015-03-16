using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public class RegExStar<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
	{
		public RegEx<T> Regex { private set; get; }

		internal RegExStar ( RegEx<T> regex )
		{
			this.TypeId = 2;
			this.Regex = regex;
		}

		// typeid diff if other is not star
		// compareTo( Regex, other.Regex )  otherwise
		public override int CompareTo (RegEx<T> other)
		{
			var diff = TypeId - other.TypeId;
			if (diff != 0)
				return diff;

			var star = other as RegExStar<T>;
			return Regex.CompareTo (star.Regex);
		}

		public override bool HasEpsilon()
		{
			return true;
		}

		public override IEnumerable<T> DerivChanges()
		{
			return Regex.DerivChanges();
		}

		public override RegEx<T> Derivative(T c)
		{
			return RegExFactory.Concat(Regex.Derivative(c), RegExFactory.Star(Regex));
		}
	}
}

