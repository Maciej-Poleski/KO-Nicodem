using System;

namespace Nicodem.Parser
{
    public interface ISymbol : IComparable<ISymbol>, IEquatable<ISymbol>
    {
        ISymbol EOF { get; }
        public static ISymbol MinValue { get; }
    }
}