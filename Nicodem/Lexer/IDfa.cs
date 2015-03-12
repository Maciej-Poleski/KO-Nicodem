﻿using System;

namespace Nicodem.Lexer
{
    public interface IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol>
        where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        TDfaState Start { get; }
    }
}