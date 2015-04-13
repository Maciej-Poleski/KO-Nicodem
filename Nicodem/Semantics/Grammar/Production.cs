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
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Lhs, Rhs);
        }
    }
}