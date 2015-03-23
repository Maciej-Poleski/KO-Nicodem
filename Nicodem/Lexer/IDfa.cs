using System;

namespace Nicodem.Lexer
{
    public interface IDfa<TSymbol> where TSymbol : IEquatable<TSymbol>, IComparable<TSymbol>
    {
        IDfaState<TSymbol> Start { get; }
    }
}