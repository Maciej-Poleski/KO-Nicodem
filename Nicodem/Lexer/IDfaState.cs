using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public interface IDfaState<TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        uint Accepting { get; }
        IReadOnlyList<KeyValuePair<TSymbol, IDfaState<TSymbol>>> Transitions { get; }
    }

    internal static partial class Extensions
    {
        internal static bool IsAccepting<TSymbol>(this IDfaState<TSymbol> state)
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            return state.Accepting != 0;
        }
    }
}