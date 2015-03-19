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
        //1:0 --a--> 2
        //2:1 --a--> 2
        public void SimpleTest()
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
        //1:0 --a--> 2
        //2:1 --a--> 3
        //3:1 --a--> 2
        public void SimpleTest2()
        {
            var firstState = new DfaUtils. MinimizedDfaState<char>();
            firstState.Accepting = 0;
            var secondState = new DfaUtils. MinimizedDfaState<char>();
            secondState.Accepting = 1;
            var thirdState = new DfaUtils. MinimizedDfaState<char>();
            thirdState.Accepting = 1;
            firstState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            secondState.Transitions = new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState)};
            thirdState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            var dfa = new DfaUtils.MinimizedDfa<char>();
            dfa.Start = firstState;


            dfa = DfaUtils.Minimized<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char>(dfa);


            var stateList = DfaUtils.PrepareStateList<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char >(dfa);
            Assert.AreEqual(stateList.Count, 2);
        }

        [Test]
		//http://edu.pjwstk.edu.pl/wyklady/jfa/scb/jfa-main-node10.html
        //1:0 --a--> 2, --b--> 1
        //2:0 --a--> 3, --b--> 1
        //3:0 --a--> 3, --b--> 4
        //4:1 --a--> 5, --b--> 4
        //5:1 --a--> 6, --b--> 4
        //6:1 --a--> 6, --b--> 4
        public void SimpleTest3()
        {
            var firstState = new DfaUtils. MinimizedDfaState<char>();
            firstState.Accepting = 0;
            var secondState = new DfaUtils. MinimizedDfaState<char>();
            secondState.Accepting = 0;
            var thirdState = new DfaUtils. MinimizedDfaState<char>();
            thirdState.Accepting = 0;
            var fourthState = new DfaUtils. MinimizedDfaState<char>();
            fourthState.Accepting = 1;
            var fifthState = new DfaUtils. MinimizedDfaState<char>();
            fifthState.Accepting = 1;
            var sixthState = new DfaUtils. MinimizedDfaState<char>();
            sixthState.Accepting = 1;

            firstState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', firstState)};
            secondState.Transitions = new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', firstState)};
            thirdState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            fourthState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', fifthState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            fifthState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', sixthState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            sixthState.Transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', sixthState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            var dfa = new DfaUtils.MinimizedDfa<char>();
            dfa.Start = firstState;


            dfa = DfaUtils.Minimized<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char>(dfa);


            var stateList = DfaUtils.PrepareStateList<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char >(dfa);
            Assert.AreEqual(stateList.Count, 4);
        }

    }
}
