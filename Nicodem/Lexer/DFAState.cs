using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
    class DFAState
    {
        uint Accepting {get;}
        KeyValuePair<char,DFAState>[] Transitions;
    }
}
