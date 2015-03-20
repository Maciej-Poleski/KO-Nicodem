using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
	public interface IDfaState<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol>
        where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        uint Accepting { get; set; }
        KeyValuePair<TSymbol, TDfaState>[] Transitions { get; set; }
    }

    internal static class Extensions
    {
        internal static bool IsDead<T>(this T state) where T : IDfaState<T, char>
        {
            var deadTransition = new[] {new KeyValuePair<char, T>('\0', state)};
            return state.Accepting == 0 && state.Transitions == deadTransition;
        }

        internal static bool IsAccepting<TDfaState, TSymbol>(this TDfaState state)
            where TDfaState : IDfaState<TDfaState, TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            return state.Accepting != 0;
        }
    }
}