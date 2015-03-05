using System.Collections.Generic;

namespace Nicodem.Lexer
{
    internal interface IDfaState<T> where T : IDfaState<T>
    {
        uint Accepting { get; }
        KeyValuePair<char, T>[] Transitions { get; }
    }
}