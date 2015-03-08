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
            accepting = acceptingStateMarker;
            Start = CalculateDfaState(regEx);
        }

        private DFAState CalculateDfaState(RegEx regEx){
            if(dictionaryOfDfaStates.ContainsKey(regEx))
                return dictionaryOfDfaStates[regEx];//return this state

            
            List<KeyValuePair<char, DFAState>> listOfTransitions = new List<KeyValuePair<char,DFAState>>();

            foreach (char c in regEx.DerivChanges())
                listOfTransitions.Add(new KeyValuePair<char,DFAState>(c, CalculateDfaState(regEx.Derivative(c))));

            //If epsilon is in Language this is accepting state
            if (regEx.HasEpsilon())
                return new DFAState(accepting, listOfTransitions.ToArray());

            DFAState new_state = new DFAState(0, listOfTransitions.ToArray());
            dictionaryOfDfaStates.Add(regEx, new_state);
            return new_state;
        }

        public DFAState Start { get; private set; }

        private Dictionary<RegEx, DFAState> dictionaryOfDfaStates = new Dictionary<RegEx,DFAState>();

        private uint accepting;
    }
}
