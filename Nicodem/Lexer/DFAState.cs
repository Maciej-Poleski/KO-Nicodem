using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
    class DFAState : IDfaState<DFAState>
    {
        public uint Accepting { get; private set; }
        public KeyValuePair<char, DFAState>[] Transitions { get; private set; }

        public DFAState(uint acceptingStateMaker, KeyValuePair<char, DFAState>[] transitions)
        {
            Accepting = acceptingStateMaker;
            Transitions = transitions;
        }
    }
}
