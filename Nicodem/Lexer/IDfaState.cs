using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public interface IDfaState<TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        uint Accepting { get; }
        IReadOnlyList<KeyValuePair<TSymbol, IDfaState<TSymbol>>> Transitions { get; }
    }
}