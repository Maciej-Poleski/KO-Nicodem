﻿using System;

namespace Nicodem.Parser
{
    public interface ISymbol<TSymbol> : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        string Description { get; }
        bool IsTerminal { get; }
    }
}