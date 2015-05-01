using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Immutable;

namespace Nicodem.Parser.Tests
{
    [TestFixture]
    class LLConfigurationTests
    {
        Dictionary<string, DfaState<CharSymbol>> states;

        [TestFixtureSetUp]
        public void InitStates()
        {
            states = new Dictionary<string, DfaState<CharSymbol>>();

            states["A1"] = new DfaState<CharSymbol>("A1");
            states["A2"] = new DfaState<CharSymbol>("A2");
            states["A3"] = new DfaState<CharSymbol>("A3");

            states["B1"] = new DfaState<CharSymbol>("B1");
            states["B2"] = new DfaState<CharSymbol>("B2");
            states["B3"] = new DfaState<CharSymbol>("B3");
        }

        // ------ test constructor content ------
        [Test]
        public void LLConfSymbolConstructorTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var ll = new LlConfiguration<CharSymbol>(chSym);
            Console.WriteLine("LL -> " + ll);
            Assert.AreEqual(chSym, ll.label);
            Assert.IsEmpty(ll.stack);
        }

        [Test]
        public void LLConfStackConstructorTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var stack = new Stack<DfaState<CharSymbol>>();
            stack.Push(states["A1"]);
            stack.Push(states["A2"]);
            var imList = ImmutableList.Create(states["A1"], states["A2"]);

            var ll = new LlConfiguration<CharSymbol>(chSym, stack);
            Console.WriteLine("LL -> " + ll);
            Assert.AreEqual(chSym, ll.label);
            Assert.AreEqual(imList, ll.stack);
        }

        [Test]
        public void LLConfListConstructorTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);

            var ll = new LlConfiguration<CharSymbol>(chSym, imList);
            Console.WriteLine("LL -> " + ll);
            Assert.AreEqual(chSym, ll.label);
            Assert.AreEqual(imList, ll.stack);
        }

        // ------ test constructor count ------
        [Test]
        public void LLConfEmptyCountTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var ll = new LlConfiguration<CharSymbol>(chSym);
            Assert.AreEqual(ll.Count(), 0);
        }

        [Test]
        public void LLConfThreeCountTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var stack = new Stack<DfaState<CharSymbol>>();
            stack.Push(new DfaState<CharSymbol>("A1"));
            stack.Push(new DfaState<CharSymbol>("A2"));
            stack.Push(new DfaState<CharSymbol>("A3"));
            var ll = new LlConfiguration<CharSymbol>(chSym, stack);
            Assert.AreEqual(ll.Count(), 3);
        }

        // ------ test constructor stack copy ------
        [Test]
        public void LLConfSymbolConstructorStackTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var stack = new Stack<DfaState<CharSymbol>>();
            var ll = new LlConfiguration<CharSymbol>(chSym);
            Assert.AreEqual(stack, ll.copyOfStack());
        }

        [Test]
        public void LLConfStackConstructorStackTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var stack = new Stack<DfaState<CharSymbol>>();
            stack.Push(states["A1"]);
            stack.Push(states["A2"]);
            stack.Push(states["A3"]);
            var ll = new LlConfiguration<CharSymbol>(chSym, stack);
            Assert.AreEqual(stack, ll.copyOfStack());
        }

        [Test]
        public void LLConfListConstructorStackTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var stack = new Stack<DfaState<CharSymbol>>();
            stack.Push(states["A1"]);
            stack.Push(states["A2"]);
            stack.Push(states["A3"]);
            var imList = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);
            var ll = new LlConfiguration<CharSymbol>(chSym, imList);
            Assert.AreEqual(stack, ll.copyOfStack());
        }

        // ------ test equals ------
        [Test]
        public void LLConfEmptySameLabelsEqualTest()
        {
            var ll1 = new LlConfiguration<CharSymbol>(new CharSymbol('A'));
            var ll2 = new LlConfiguration<CharSymbol>(new CharSymbol('A'));
            Assert.AreEqual(ll1, ll2);
        }

        [Test]
        public void LLConfEmptyDiffLabelsNotEqualTest()
        {
            var ll1 = new LlConfiguration<CharSymbol>(new CharSymbol('A'));
            var ll2 = new LlConfiguration<CharSymbol>(new CharSymbol('B'));
            Assert.AreNotEqual(ll1, ll2);
        }

        [Test]
        public void LLConfSameListEqualTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);

            var ll1 = new LlConfiguration<CharSymbol>(chSym, imList);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, imList);
            Assert.AreEqual(ll1, ll2);
        }

        [Test]
        public void LLConfSameListDiffLabelNotEqualTest()
        {
            CharSymbol chSym1 = new CharSymbol('A');
            CharSymbol chSym2 = new CharSymbol('B');
            var imList = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);

            var ll1 = new LlConfiguration<CharSymbol>(chSym1, imList);
            var ll2 = new LlConfiguration<CharSymbol>(chSym2, imList);
            Assert.AreNotEqual(ll1, ll2);
        }

        [Test]
        public void LLConfDiffListNotEqualTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var aaaList = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);
            var abaList = ImmutableList.Create(states["A1"], states["B2"], states["A3"]);

            var ll1 = new LlConfiguration<CharSymbol>(chSym, aaaList);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, abaList);
            Assert.AreNotEqual(ll1, ll2);
        }

        [Test]
        public void LLConfDiffCountNotEqualTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var aaaList = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);
            var aaList = ImmutableList.Create(states["A1"], states["A2"]);

            var ll1 = new LlConfiguration<CharSymbol>(chSym, aaaList);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, aaList);
            Assert.AreNotEqual(ll1, ll2);
        }

        // ------ test peek, push, pop ------
        [Test]
        public void LLConfPeekTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A3"]);
            var ll = new LlConfiguration<CharSymbol>(chSym, imList);
            Assert.AreEqual(states["A3"], ll.Peek());
        }

        // --- push ---
        [Test]
        public void LLConfEmptyPushTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A1"]);
            var ll = new LlConfiguration<CharSymbol>(chSym);
            Assert.AreEqual(ll.Push(states["A1"]).stack, imList);
        }
        [Test]
        public void LLConfEmptyPushPeekTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var ll = new LlConfiguration<CharSymbol>(chSym);
            Assert.AreEqual(ll.Push(states["A1"]).Peek(), states["A1"]);
        }
        [Test]
        public void LLConfEmptyPushEqualTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A3"]);
            var ll1 = new LlConfiguration<CharSymbol>(chSym);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, imList);
            Assert.AreEqual(ll1.Push(states["A3"]), ll2);
        }
        [Test]
        public void LLConfOnePushEqualTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList1 = ImmutableList.Create(states["A1"], states["A2"]);
            var imList2 = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);
            var ll1 = new LlConfiguration<CharSymbol>(chSym, imList1);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, imList2);
            Assert.AreEqual(ll1.Push(states["A3"]), ll2);
        }
        [Test]
        public void LLConfThreePushEqualTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var targetList = ImmutableList.Create(states["A1"], states["B2"], states["A3"]);
            var ll1 = new LlConfiguration<CharSymbol>(chSym);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, targetList);
            Assert.AreEqual(ll1.Push(states["A1"]).Push(states["B2"]).Push(states["A3"]), ll2);
        }

        // --- pop ---
        [Test]
        public void LLConfPopToEmptyTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A1"]);
            var begLL = new LlConfiguration<CharSymbol>(chSym, imList);
            var resLL = new LlConfiguration<CharSymbol>(chSym);
            Assert.AreEqual(begLL.Pop(), resLL);
        }
        [Test]
        public void LLConfPopPeekTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A1"], states["A2"]);
            var begLL = new LlConfiguration<CharSymbol>(chSym, imList);
            Assert.AreEqual(begLL.Pop().Peek(), states["A1"]);
        }
        [Test]
        public void LLConfOnePopEqualTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList1 = ImmutableList.Create(states["A1"], states["A2"]);
            var imList2 = ImmutableList.Create(states["A1"], states["A2"], states["A3"]);
            var ll1 = new LlConfiguration<CharSymbol>(chSym, imList1);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, imList2);
            Assert.AreEqual(ll1, ll2.Pop());
        }
        [Test]
        public void LLConfThreePopToEmptyTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var imList = ImmutableList.Create(states["A1"], states["B2"], states["A3"]);
            var ll1 = new LlConfiguration<CharSymbol>(chSym);
            var ll2 = new LlConfiguration<CharSymbol>(chSym, imList);
            Assert.AreEqual(ll1, ll2.Pop().Pop().Pop());
        }

        // ------ test push/pop count ------
        [Test]
        public void LLConfPushCountTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var stack = new Stack<DfaState<CharSymbol>>();
            stack.Push(states["A1"]);
            stack.Push(states["A2"]);
            var ll = new LlConfiguration<CharSymbol>(chSym, stack);
            Assert.AreEqual(ll.Push(states["A3"]).Count(), 3);
        }
        [Test]
        public void LLConfPopCountTest()
        {
            CharSymbol chSym = new CharSymbol('A');
            var stack = new Stack<DfaState<CharSymbol>>();
            stack.Push(states["A1"]);
            stack.Push(states["A2"]);
            var ll = new LlConfiguration<CharSymbol>(chSym, stack);
            Assert.AreEqual(ll.Pop().Count(), 1);
        }
    }
}
