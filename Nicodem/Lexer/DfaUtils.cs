using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    internal static class DfaUtils
    {
        internal static MinimizedDfa<TSymbol> Minimized<TDfa, TDfaState, TSymbol>(this TDfa dfa)
            where TDfa : IDfa<TDfaState, TSymbol> where TDfaState : IDfaState<TDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            // Funkcja minimalizująca DFA.
            // Można zmienić typ rezultatu z DFA na coś innego,
            // ale to coś innego powinno implementowac IDfa.
            // Nie zapomnij o tym, że istnieje potencjalnie wiele
            // rodzajów stanów akcpetujących (rozróżnianych różnymi
            // wartościami własności DFAState.Accepting: 0 - nieakceptujący,
            // coś innego niż 0 - jakiś rodzaj akceptacji)
            throw new NotImplementedException();
        }

        internal struct MinimizedDfa<TSymbol> : IDfa<MinimizedDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            public MinimizedDfaState<TSymbol> Start
            {
                get { throw new NotImplementedException(); }
            }
        }

        internal class MinimizedDfaState<TSymbol> : IDfaState<MinimizedDfaState<TSymbol>, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            public uint Accepting
            {
                get { throw new NotImplementedException(); }
            }

            public KeyValuePair<TSymbol, MinimizedDfaState<TSymbol>>[] Transitions
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}