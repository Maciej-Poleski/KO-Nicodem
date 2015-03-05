using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
    class DFAState : IDfaState<DFAState>
    {
        public uint Accepting {get;}
        public KeyValuePair<char, DFAState>[] Transitions { get; }
    }
}
