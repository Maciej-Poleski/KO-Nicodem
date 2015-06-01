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
            firstState._accepting = 0;
            var secondState = new DfaUtils. MinimizedDfaState<char>();
            secondState._accepting = 1;
            firstState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            secondState._transitions = new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            var dfa = new DfaUtils.MinimizedDfa<char>();
            dfa._start = firstState;


            dfa = DfaUtils.Minimized<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char>(dfa);


            var stateList = DfaUtils.PrepareStateList<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char >(dfa);
            Assert.AreEqual(stateList.Count, 2);
        }

        [Test]
        //1:0 --a--> 2
        //2:1 --a--> 3
		//3:2 --a--> 2
        public void SimpleTest2()
        {
            var firstState = new DfaUtils. MinimizedDfaState<char>();
            firstState._accepting = 0;
            var secondState = new DfaUtils. MinimizedDfaState<char>();
            secondState._accepting = 1;
            var thirdState = new DfaUtils. MinimizedDfaState<char>();
			thirdState._accepting = 2;
            firstState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            secondState._transitions = new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState)};
            thirdState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[1] {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState)};
            var dfa = new DfaUtils.MinimizedDfa<char>();
            dfa._start = firstState;


            dfa = DfaUtils.Minimized<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char>(dfa);


            var stateList = DfaUtils.PrepareStateList<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char >(dfa);
			Assert.AreEqual(stateList.Count, 3);
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
            firstState._accepting = 0;
            var secondState = new DfaUtils. MinimizedDfaState<char>();
            secondState._accepting = 0;
            var thirdState = new DfaUtils. MinimizedDfaState<char>();
            thirdState._accepting = 0;
            var fourthState = new DfaUtils. MinimizedDfaState<char>();
            fourthState._accepting = 1;
            var fifthState = new DfaUtils. MinimizedDfaState<char>();
            fifthState._accepting = 1;
            var sixthState = new DfaUtils. MinimizedDfaState<char>();
            sixthState._accepting = 1;

            firstState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', firstState)};
            secondState._transitions = new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', firstState)};
            thirdState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            fourthState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', fifthState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            fifthState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', sixthState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            sixthState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
            {new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', sixthState),
                new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
            var dfa = new DfaUtils.MinimizedDfa<char>();
            dfa._start = firstState;


            dfa = DfaUtils.Minimized<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char>(dfa);


            var stateList = DfaUtils.PrepareStateList<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char >(dfa);
			Assert.AreEqual(stateList.Count, 4);
        }

		[Test]
		//http://edu.pjwstk.edu.pl/wyklady/jfa/scb/jfa-main-node10.html
		//1:0 --a--> 2, --b--> 1
		//2:0 --a--> 3, --b--> 1
		//3:0 --a--> 3, --b--> 4
		//4:1 --a--> 5, --b--> 4
		//5:1 --a--> 6, --b--> 4
		//6:2 --a--> 6, --b--> 4
		public void SimpleTest4()
		{
			var firstState = new DfaUtils. MinimizedDfaState<char>();
			firstState._accepting = 0;
			var secondState = new DfaUtils. MinimizedDfaState<char>();
			secondState._accepting = 0;
			var thirdState = new DfaUtils. MinimizedDfaState<char>();
			thirdState._accepting = 0;
			var fourthState = new DfaUtils. MinimizedDfaState<char>();
			fourthState._accepting = 1;
			var fifthState = new DfaUtils. MinimizedDfaState<char>();
			fifthState._accepting = 1;
			var sixthState = new DfaUtils. MinimizedDfaState<char>();
			sixthState._accepting = 2;

			firstState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
			{new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', secondState),
				new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', firstState)};
			secondState._transitions = new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>[2]
			{new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState),
				new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', firstState)};
			thirdState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
			{new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', thirdState),
				new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
			fourthState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
			{new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', fifthState),
				new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
			fifthState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
			{new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', sixthState),
				new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
			sixthState._transitions = new KeyValuePair<char, DfaUtils.MinimizedDfaState<char>>[2]
			{new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('a', sixthState),
				new KeyValuePair<char, DfaUtils. MinimizedDfaState<char>>('b', fourthState)};
			var dfa = new DfaUtils.MinimizedDfa<char>();
			dfa._start = firstState;


			dfa = DfaUtils.Minimized<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char>(dfa);


			var stateList = DfaUtils.PrepareStateList<DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, char >(dfa);

			Assert.AreEqual(stateList.Count, 6);
		}

        [Test]
        public void MinimizedEmpty()
        {
            DfaUtils.MakeEmptyLanguageDfa<char>();
        }

        [Test]
        public void MakeMinimizedProductDfaTypePunnedInvocation()
        {
            IDfa<char> someDfa = DfaUtils.MakeEmptyLanguageDfa<char>();
            var someOtherDfa = DfaUtils.MakeMinimizedProductDfa(someDfa, someDfa,
                (a, b) =>
                {
                    if (a != 0 && b != 0)
                    {
                        throw new ArgumentException();
                    }
                    return a + b;
                });
            Assert.NotNull(someOtherDfa);
            Assert.NotNull(someOtherDfa.Start);
            Assert.AreEqual(someOtherDfa.Start.Accepting, 0);
            Assert.AreEqual(someOtherDfa.Start.Transitions.Count, 1);
            Assert.AreEqual(someOtherDfa.Start.Transitions[0].Key, '\0');
            Assert.AreSame(someOtherDfa.Start.Transitions[0].Value, someOtherDfa.Start);
        }

    }
}
