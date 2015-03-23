using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public class DFAState<T> : AbstractDfaState<DFAState<T>, T> where T : IComparable<T>, IEquatable<T>
    {
        internal uint _accepting;
        internal KeyValuePair<T, DFAState<T>>[] _transitions;

        public DFAState()
        {
        }

        public DFAState(uint accepting, KeyValuePair<T, DFAState<T>>[] transitions)
        {
            _accepting = accepting;
            _transitions = transitions;
        }

        public override uint Accepting
        {
            get { return _accepting; }
        }

        public override KeyValuePair<T, DFAState<T>>[] Transitions
        {
            get { return _transitions; }
        }
    }
}