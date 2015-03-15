using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
	public class DFAState<T> : IDfaState<DFAState<T>,T> where T : IComparable<T>, IEquatable<T>
    {
        public uint Accepting { get; internal set; }
        public KeyValuePair<T, DFAState<T>>[] Transitions { get; internal set; }
        public List<KeyValuePair<T, DFAState<T>>> Predecessors { get; internal set; }

        public DFAState()
        {
        }

        public DFAState(uint acceptingStateMaker, KeyValuePair<T, DFAState<T>>[] transitions)
        {
            Accepting = acceptingStateMaker;
            Transitions = transitions;
            Predecessors = new List<KeyValuePair<T, DFAState<T>>>();

            foreach(var entry in transitions)
            {
                entry.Value.Predecessors.Add(new KeyValuePair<T, DFAState<T>>(entry.Key, this));
            }

        }
    }
}
