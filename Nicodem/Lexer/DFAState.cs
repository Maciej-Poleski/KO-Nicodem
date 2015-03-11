using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
    class DFAState : IDfaState<DFAState>
    {
        public uint Accepting { get; internal set; }
        public KeyValuePair<char, DFAState>[] Transitions { get; internal set; }

        public DFAState()
        {
        }

        public DFAState(uint acceptingStateMaker, KeyValuePair<char, DFAState>[] transitions)
        {
            Accepting = acceptingStateMaker;
            Transitions = transitions;
        }
    }
}
