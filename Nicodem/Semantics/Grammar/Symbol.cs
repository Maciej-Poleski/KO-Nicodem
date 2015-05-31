using System;
using System.Diagnostics;
using Nicodem.Parser;

namespace Nicodem.Semantics.Grammar
{
    public struct Symbol : ISymbol<Symbol>, IEquatable<Symbol>, IComparable<Symbol>
    {
        public static readonly Symbol MinValue = MakeNamedSymbol(int.MinValue, "MinValue");
        public static readonly Symbol EOF = MakeNamedSymbol(-1, "EOF");

        #region ISymbol implementation

        public string Description
        {
            get { return NicodemGrammarProductions.GetSymbolName(this); }
        }

        public bool IsTerminal
        {
            get { return !Production.IsNonterminalSymbol(this); }
        }

        #endregion

        private readonly int _category; // Symbol is in fact category of some Regular Expression in Lexer

        internal Symbol(int category)
        {
            _category = category;
        }

        private static Symbol MakeNamedSymbol(int category, string name)
        {
            var symbol = new Symbol(category);
            NicodemGrammarProductions.RegisterSymbolName(symbol, name);
            return symbol;
        }

        public int CompareTo(Symbol other)
        {
            return _category.CompareTo(other._category);
        }

        public bool Equals(Symbol other)
        {
            return _category == other._category;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", IsTerminal ? "T" : "N", _category, Description == "" ? "" : " (" + Description + ")");
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