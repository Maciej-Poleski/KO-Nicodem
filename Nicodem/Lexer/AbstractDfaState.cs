using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
    public abstract class AbstractDfaState<TDfaState, TSymbol> : IDfaState<TSymbol>
        where TDfaState : AbstractDfaState<TDfaState, TSymbol>
        where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        public abstract KeyValuePair<TSymbol, TDfaState>[] Transitions { get; }
        public abstract uint Accepting { get; }

        IReadOnlyList<KeyValuePair<TSymbol, IDfaState<TSymbol>>> IDfaState<TSymbol>.Transitions
        {
            get { return new TransitionsType(Transitions); }
        }

        private class TransitionsType : IReadOnlyList<KeyValuePair<TSymbol, IDfaState<TSymbol>>>
        {
            private readonly KeyValuePair<TSymbol, TDfaState>[] Transitions;

            public TransitionsType(KeyValuePair<TSymbol, TDfaState>[] transitions)
            {
                Transitions = transitions;
            }

            public int Count
            {
                get { return Transitions.Length; }
            }

            public KeyValuePair<TSymbol, IDfaState<TSymbol>> this[int index]
            {
                get
                {
                    var kv = Transitions[index];
                    return new KeyValuePair<TSymbol, IDfaState<TSymbol>>(kv.Key, kv.Value);
                }
            }

            public IEnumerator<KeyValuePair<TSymbol, IDfaState<TSymbol>>> GetEnumerator()
            {
                return
                    Transitions.Select(kv => new KeyValuePair<TSymbol, IDfaState<TSymbol>>(kv.Key, kv.Value))
                        .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

    internal static class Extensions
    {
        internal static bool IsDead<T>(this T state) where T : AbstractDfaState<T, char>
        {
            var deadTransition = new[] {new KeyValuePair<char, T>('\0', state)};
            return state.Accepting == 0 && deadTransition.SequenceEqual(state.Transitions);
        }

        internal static bool IsAccepting<TDfaState, TSymbol>(this TDfaState state)
            where TDfaState : AbstractDfaState<TDfaState, TSymbol>
            where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
        {
            return state.Accepting != 0;
        }
    }
}