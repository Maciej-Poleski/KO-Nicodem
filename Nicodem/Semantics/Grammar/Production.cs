using Nicodem.Lexer;
using Nicodem.Parser;

namespace Nicodem.Semantics.Grammar
{
    internal class Production : IProduction
    {
        public Symbol Lhs { get; private set; }
        public RegEx<Symbol> Rhs { get; private set; }

        public Production(Symbol lhs, RegEx<Symbol> rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        ISymbol IProduction.Lhs
        {
            get { return Lhs; }
        }

        RegEx<ISymbol> IProduction.Rhs
        {
            get { return RegEx<Symbol>.Convert<ISymbol>(Rhs, symbol => symbol); }
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Lhs, Rhs);
        }
    }
}