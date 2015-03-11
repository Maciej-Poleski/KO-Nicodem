using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Lexer
{
    class RegexDfa : IDfa<DFAState>
    {
        public RegexDfa(RegEx regEx, uint acceptingStateMarker)
        {
            accepting = acceptingStateMarker;
            Start = CalculateDfaState(regEx);
        }

        private DFAState CalculateDfaState(RegEx regEx){
            if(dictionaryOfDfaStates.ContainsKey(regEx))
                return dictionaryOfDfaStates[regEx];//return this state

            DFAState new_state = new DFAState();
            dictionaryOfDfaStates.Add(regEx, new_state);
            
            List<KeyValuePair<char, DFAState>> listOfTransitions = new List<KeyValuePair<char,DFAState>>();

            foreach (char c in regEx.DerivChanges())
            {
                RegEx deriv = regEx.Derivative(c);
                if(deriv == regEx)
                    listOfTransitions.Add(new KeyValuePair<char,DFAState>(c, null));
                else
                    listOfTransitions.Add(new KeyValuePair<char, DFAState>(c, CalculateDfaState(regEx.Derivative(c))));
            }

            if (regEx.HasEpsilon())
                new_state.Accepting = accepting;
            else
                new_state.Accepting = 0;

            new_state.Transitions = listOfTransitions.ToArray();

            return new_state;
        }

        public DFAState Start { get; private set; }

        private SortedDictionary<RegEx, DFAState> dictionaryOfDfaStates = new SortedDictionary<RegEx,DFAState>();

        private uint accepting;
    }
}
