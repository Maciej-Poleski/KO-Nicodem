using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public class RegExDfa<T> : IDfa<DFAState<T>, T> where T : IComparable<T>, IEquatable<T>
    {
        private readonly uint accepting;

        private readonly SortedDictionary<RegEx<T>, DFAState<T>> dictionaryOfDfaStates =
            new SortedDictionary<RegEx<T>, DFAState<T>>();

        public RegExDfa(RegEx<T> regEx, uint acceptingStateMarker)
        {
            accepting = acceptingStateMarker;
            Start = CalculateDfaState(regEx);
        }

        public DFAState<T> Start { get; set; }

        private DFAState<T> CalculateDfaState(RegEx<T> regEx)
        {
            if (dictionaryOfDfaStates.ContainsKey(regEx))
                return dictionaryOfDfaStates[regEx]; //return this state

            var new_state = new DFAState<T>();
            dictionaryOfDfaStates.Add(regEx, new_state);

            var listOfTransitions = new List<KeyValuePair<T, DFAState<T>>>();

            foreach (var c in regEx.DerivChanges())
            {
                listOfTransitions.Add(new KeyValuePair<T, DFAState<T>>(c, CalculateDfaState(regEx.Derivative(c))));
            }

            if (regEx.HasEpsilon())
                new_state.Accepting = accepting;
            else
                new_state.Accepting = 0;

            new_state.Transitions = listOfTransitions.ToArray();

            return new_state;
        }
    }
}