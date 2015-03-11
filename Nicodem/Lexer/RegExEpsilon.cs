using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public class RegExEpsilon<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
    {
        internal static readonly RegExEpsilon<T> RegexEpsilon = new RegExEpsilon<T>();

        private RegExEpsilon()
        {
            TypeId = 0;
        }

        // 0 if other is RegExEpsilon
        // a < 0 otherwise
        // => RegExEpsilon is the 'smallest' one
        public override int CompareTo(RegEx<T> other)
        {
            return TypeId - other.TypeId;
        }

        public override bool HasEpsilon()
        {
            return true;
        }

        public override IEnumerable<T> DerivChanges()
        {
            return new T[] {};
        }

        public override RegEx<T> Derivative(T c)
        {
            return RegExFactory.Empty<T>();
        }
    }
}