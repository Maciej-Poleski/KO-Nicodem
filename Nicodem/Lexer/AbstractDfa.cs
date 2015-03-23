using System;

namespace Nicodem.Lexer
{
    public abstract class AbstractDfa<TDfaState, TSymbol> : IDfa<TSymbol>
        where TDfaState : AbstractDfaState<TDfaState, TSymbol>
        where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        public abstract TDfaState Start { get; }

        IDfaState<TSymbol> IDfa<TSymbol>.Start
        {
            get { return Start; }
        }
    }
}