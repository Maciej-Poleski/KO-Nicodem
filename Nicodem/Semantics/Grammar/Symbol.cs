using System;
using Nicodem.Parser;

namespace Nicodem.Semantics.Grammar
{
    internal struct Symbol : ISymbol<Symbol>
    {
        public static readonly Symbol MinValue = new Symbol(-1);
        private readonly int _category; // Symbol is in fact category of some Regular Expression in Lexer

        public Symbol(int category)
        {
            _category = category;
        }

        public int CompareTo(Symbol other)
        {
            return _category - other._category;
        }

        public bool Equals(Symbol other)
        {
            return _category == other._category;
        }

        public static Symbol EOF
        {
            get { throw new NotImplementedException(); }
        }

        bool IEquatable<Symbol>.Equals(Symbol other)
        {
            throw new NotImplementedException();
        }

        int IComparable<Symbol>.CompareTo(Symbol other)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0}", _category);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Symbol && Equals((Symbol) obj);
        }

        public override int GetHashCode()
        {
            return _category.GetHashCode();
        }

        public static explicit operator Symbol(int category)
        {
            return new Symbol(category);
        }

        internal Symbol Next()
        {
            return new Symbol(_category + 1);
        }

        public static bool operator ==(Symbol left, Symbol right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Symbol left, Symbol right)
        {
            return !left.Equals(right);
        }

        public static bool operator <(Symbol left, Symbol right)
        {
            return left._category < right._category;
        }

        public static bool operator >(Symbol left, Symbol right)
        {
            return right < left;
        }

        public static bool operator <=(Symbol left, Symbol right)
        {
            return left._category <= right._category;
        }

        public static bool operator >=(Symbol left, Symbol right)
        {
            return right <= left;
        }
    }
}