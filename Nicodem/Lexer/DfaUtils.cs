using System;
using System.Net;

namespace Nicodem.Lexer
{
    internal static class DfaUtils
    {
        internal static DFA Minimized<T,TU>(this T dfa) where T : IDfa<TU> where TU : IDfaState<TU>
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
    }
}