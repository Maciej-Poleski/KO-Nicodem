using System;
using NUnit.Framework;
using Nicodem.Core;
using System.Collections.Generic;

namespace Core.Tests
{
    [TestFixture]
    public class MemoizedInputTests
    {

        [Test]
        public void Empty_Input_Test()
        {
            var input = new List<int>();
            var mi = new MemoizedInput<int>(input);

            var it_0 = mi.Begin;
            Assert.AreEqual(it_0, mi.End);

            Assert.AreEqual(it_0.Next(), mi.End);
            Assert.AreEqual(it_0.Next().Next(), mi.End);

            Assert.AreEqual(mi.At(10), mi.End);
            Assert.AreEqual(mi.At(0), mi.End);
            Assert.AreEqual(mi.At(-1), mi.End);
        }

        [Test]
        public void Iterator_Advance_Test()
        {
            var input = new List<int>{ 0, 1, 2, 3 };
            var mi = new MemoizedInput<int>(input);

            var it = mi.Begin;
            Assert.AreNotEqual(it, mi.End);

            Assert.AreEqual(it.Current, input[0]);
            Assert.AreEqual(mi.At(3), mi.End);

            Assert.AreEqual(it.Next().Current, input[1]);
            Assert.AreEqual(it.Next().Next().Current, input[2]);
            Assert.AreEqual(it.Next().Next().Next().Current, input[3]);
            Assert.AreEqual(it.Next().Next().Next().Next(), mi.End);

            var it2 = it;
            it2 = it2.Next().Next();
            Assert.AreNotEqual(it, it2);
            Assert.AreEqual(it, mi.Begin);
            Assert.AreEqual(mi.At(2), it2);
        }

        [Test]
        public void Iterator_Comparison_Test()
        {
            var input = new List<int>{ 0, 1, 2, 3 };
            var mi = new MemoizedInput<int>(input);

            var it = mi.Begin;
            Assert.AreEqual(it.Next().Next().Next().Next(), mi.End);
            Assert.IsTrue(mi.Begin < mi.End);
            Assert.IsTrue(mi.Begin <= mi.End);
            Assert.IsTrue(mi.End > mi.Begin);
            Assert.IsTrue(mi.End >= mi.Begin);
            Assert.IsTrue(mi.At(0) >= mi.Begin);
            Assert.IsTrue(mi.At(0) <= mi.Begin);
            Assert.IsTrue(mi.At(0) == mi.Begin);

            it = it.Next();
            Assert.IsTrue(it > mi.Begin);
            Assert.IsTrue(it >= mi.Begin);
            Assert.IsTrue(it.Next() >= it);
            Assert.IsTrue(mi.End > it);
            Assert.IsTrue(mi.Begin < it);
        }
    }
}

