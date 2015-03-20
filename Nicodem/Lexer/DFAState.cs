using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
	public class DFAState<T> : IDfaState<DFAState<T>,T> where T : IComparable<T>, IEquatable<T>
    {
        public uint Accepting { get; set; }
        public KeyValuePair<T, DFAState<T>>[] Transitions { get; set; }

        public DFAState(){}
        public DFAState(uint accepting, KeyValuePair<T, DFAState<T>>[] transitions)
        {
            Accepting = accepting;
            Transitions = transitions;
        }
    }
}
