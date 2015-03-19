using System;
using System.Linq;
using Nicodem.Lexer;
using Nicodem.Source;
using NUnit.Framework;
using System.Collections.Generic;

namespace Lexer.Tests
{
    [TestFixture]
    internal class DfaUtilsTests
    {
        [Test]
        public void SimpleTest() //0 --a--> 1, 1 --a--> 1
        {
            var firstState = new DfaUtils. MinimizedDfaState<char>();
            firstState.Accepting = 0;
            var secondState = new DfaUtils. MinimizedDfaState<char>();
            secondState.Accepting = 1;
            firstState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            secondState.Transitions = new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            var dfa = new DfaUtils.MinimizedDfa<char>();
            dfa.Start = firstState;


            dfa = DfaUtils.Minimized<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char>(dfa);


            var stateList = DfaUtils.PrepareStateList<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char >(dfa);
            Assert.AreEqual(stateList.Count, 2);
        }

        [Test]
        public void MinimizedEmpty()
        {
            DfaUtils.MakeEmptyLanguageDfa<char>();
        }

    }
}
