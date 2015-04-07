using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nicodem.Lexer
{
    public class RegExDfa<T> : AbstractDfa<DFAState<T>, T> where T : IComparable<T>, IEquatable<T>
    {
        private readonly uint accepting;

        private readonly SortedDictionary<RegEx<T>, DFAState<T>> dictionaryOfDfaStates =
            new SortedDictionary<RegEx<T>, DFAState<T>>();

        private DFAState<T> deadState;
        private readonly DFAState<T> _start;

        public static TSymbol MinSymbol<TSymbol>()
        {
            try
            {
				return Expression.Lambda<Func<TSymbol>>(Expression.Property(null, typeof(TSymbol), "MinValue")).Compile()();
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException(String.Format("There is no implemented static field MinValue in {0}", typeof(TSymbol)), e);
            }
        }

        public override DFAState<T> Start
        {
            get { return _start; }
        }

        /// <summary>
        /// Create RegExDfa with start state
        /// </summary>
        /// <param name="start">Start state</param>
        public RegExDfa(DFAState<T> start)
        {
            _start = start;
        }

        /// <summary>
        /// Create RegExDfa from RegEx
        /// </summary>
        /// <param name="regEx">Regular Expression from which is made automata</param>
        /// <param name="acceptingStateMarker">Number for accepting states</param>
        public RegExDfa(RegEx<T> regEx, uint acceptingStateMarker)
        {
            accepting = acceptingStateMarker;
            _start = CalculateDfaState(regEx);
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

            if (listOfTransitions.Count == 0 || !listOfTransitions[0].Key.Equals(MinSymbol<T>()))
            {
                if (deadState == null)
                {
                    deadState = new DFAState<T>();
                    deadState._accepting = 0;
                    deadState._transitions = new KeyValuePair<T, DFAState<T>>[] { new KeyValuePair<T, DFAState<T>>(MinSymbol<T>(), deadState) };
                }
                listOfTransitions.Insert(0, new KeyValuePair<T, DFAState<T>>(MinSymbol<T>(), deadState));
            }

            new_state._transitions = listOfTransitions.ToArray();

            return new_state;
        }
    }
}