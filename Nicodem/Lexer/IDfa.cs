using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public interface IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol>
        where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        TDfaState Start { get; }
        List<TDfaState> GetAllAcceptingStates();
    }
}