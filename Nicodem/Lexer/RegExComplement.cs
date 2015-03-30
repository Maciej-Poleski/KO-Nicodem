using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public class RegExComplement<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
    {
        internal RegExComplement(RegEx<T> regex)
        {
            TypeId = 3;
            Regex = regex;
        }

        public RegEx<T> Regex { private set; get; }
        // typeid diff if other is not complement
        // compareTo( Regex, other.Regex ) otherwise
        public override int CompareTo(RegEx<T> other)
        {
            var diff = TypeId - other.TypeId;
            if (diff != 0)
                return diff;

            var complement = other as RegExComplement<T>;
            return Regex.CompareTo(complement.Regex);
        }

		public override string ToString ()
		{
			return string.Format ("~({0})", Regex);
		}

        public override bool HasEpsilon()
        {
            return !Regex.HasEpsilon();
        }

        public override IEnumerable<T> DerivChanges()
        {
            return Regex.DerivChanges();
        }

        public override RegEx<T> Derivative(T c)
        {
            return RegExFactory.Complement(Regex.Derivative(c));
        }
    }
}