using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
    class DFA : IDfa<DFAState>
    {
        public DFA(RegEx regEx, uint acceptingStateMarker)
        {
            throw new NotImplementedException();
        }

        public DFAState Start{get;}
    }
}
