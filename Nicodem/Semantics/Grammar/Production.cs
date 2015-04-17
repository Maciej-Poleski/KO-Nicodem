using System.Collections.Generic;
using Nicodem.Lexer;
using Nicodem.Parser;

namespace Nicodem.Semantics.Grammar
{
    internal class Production : IProduction<Symbol>
    {
        public Symbol Lhs { get; private set; }
        public RegEx<Symbol> Rhs { get; private set; }

        public Production(Symbol lhs, RegEx<Symbol> rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
            NonterminalSymbols.Add(lhs);
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Lhs, Rhs);
        }

        internal static bool IsNonterminalSymbol(Symbol symbol)
        {
            return NonterminalSymbols.Contains(symbol);
        }

        private static readonly HashSet<Symbol> NonterminalSymbols = new HashSet<Symbol>(); // FIXME memory leak
        // When last production with symbol s is reclaimed, s.Lhs is to be forgotten.
    }
}