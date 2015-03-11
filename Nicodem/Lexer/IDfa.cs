using System;

namespace Nicodem.Lexer
{
    internal interface IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol>
        where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        TDfaState Start { get; }
    }
}