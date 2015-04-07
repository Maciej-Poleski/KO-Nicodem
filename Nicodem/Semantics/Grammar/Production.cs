using Nicodem.Lexer;
using Nicodem.Parser;

namespace Nicodem.Semantics.Grammar
{
    internal class Production : Production<Symbol>
    {
        public Production(Symbol lhs, RegEx<Symbol> rhs)
			: base (lhs, rhs)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Lhs, Rhs);
        }
    }
}