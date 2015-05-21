using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public abstract class AbstractDfa<TDfaState, TSymbol> : IDfa<TSymbol>
        where TDfaState : AbstractDfaState<TDfaState, TSymbol>
        where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
    {
        public abstract TDfaState Start { get; }

        IDfaState<TSymbol> IDfa<TSymbol>.Start
        {
            get { return Start; }
        }
    }

    internal static class Extension
    {
        internal static int NumberOfPseudoDeadStates(this IDfa<char> dfa)
        {
            int number = 0;
            Queue<IDfaState<char>> Q = new Queue<IDfaState<char>>();
            Dictionary<IDfaState<char>, int> color = new Dictionary<IDfaState<char>, int>();
            while (Q.Count > 0)
            {
                IDfaState<char> current_state = Q.Dequeue();
                if (color.ContainsKey(current_state))
                {
                    color[current_state] = 1;
                    if (current_state.IsPseudoDead())
                        number++;
                    foreach (var transition in current_state.Transitions)
                        if (!color.ContainsKey(transition.Value))
                            Q.Enqueue(transition.Value);
                }
            }
            return number;
        }
    }
}