using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public class RegExRange<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
    {
        public T Character { private set; get; }

        internal RegExRange(T c)
        {
            TypeId = 1;
            Character = c;
        }

        // typeid diff if other is not star
        // c - other.c  otherwise
        public override int CompareTo(RegEx<T> other)
        {
            var diff = TypeId - other.TypeId;
            if (diff != 0)
                return diff;

            var range = other as RegExRange<T>;
            return Character.CompareTo(range.Character);
        }

		public override string ToString ()
		{
            return string.Format("[{0}...]", Character);
		}

        public override bool HasEpsilon()
        {
            return false;
        }

        public override IEnumerable<T> DerivChanges()
        {
            return new[] {Character};
        }

        public override RegEx<T> Derivative(T c)
        {
            return Character.CompareTo(c) <= 0 ? RegExFactory.Epsilon<T>() : RegExFactory.Empty<T>();
        }
    }
}