using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public class RegExDfa<T> : AbstractDfa<DFAState<T>, T> where T : IComparable<T>, IEquatable<T>
    {
        private readonly uint accepting;

        private readonly SortedDictionary<RegEx<T>, DFAState<T>> dictionaryOfDfaStates =
            new SortedDictionary<RegEx<T>, DFAState<T>>();

        private DFAState<T> deadState;
        private readonly DFAState<T> _start;

        public RegExDfa(DFAState<T> start)
        {
            _start = start;
        }

        public RegExDfa(RegEx<T> regEx, uint acceptingStateMarker)
        {
            accepting = acceptingStateMarker;
            _start = CalculateDfaState(regEx);
        }

        public override DFAState<T> Start
        {
            get { return _start; }
        }

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
                new_state._accepting = accepting;
            else
                new_state._accepting = 0;

            if (listOfTransitions.Count == 0)
            {
                if (deadState == null)
                {
                    deadState = new DFAState<T>();
                    deadState._accepting = 0;
                    deadState._transitions = new KeyValuePair<T, DFAState<T>>[] { new KeyValuePair<T, DFAState<T>>(default(T), deadState)};
                }
                listOfTransitions.Add(new KeyValuePair<T, DFAState<T>>(default(T), deadState));
            }

            new_state._transitions = listOfTransitions.ToArray();

            return new_state;
        }
    }
}