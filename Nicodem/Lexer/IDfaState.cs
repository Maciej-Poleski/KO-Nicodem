using System.Collections.Generic;

namespace Nicodem.Lexer
{
    internal interface IDfaState<T> where T : IDfaState<T>
    {
        uint Accepting { get; }
        KeyValuePair<char, T>[] Transitions { get; }
    }

    internal static partial class Extensions
    {
        internal static bool IsDead<T>(this T state) where T : IDfaState<T>
        {
            var deadTransition=new[]{new KeyValuePair<char, T>('\0',state) };
            return state.Accepting == 0 && state.Transitions == deadTransition;
        }

        internal static bool IsAccepting<T>(this T state) where T : IDfaState<T>
        {
            return state.Accepting != 0;
        }
    }
}