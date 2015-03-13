using NUnit.Framework;
using System;
using System.Text;
using Nicodem.Source;

namespace Nicodem.Source.Tests
{
    [TestFixture]
    public abstract class IOriginAbstractTests
    {
        public abstract IOrigin CreateOriginTest(string source);

        // --------- initialization tests ---------
        [Test]
        public void BeginLocationOriginTest()
        {
            string source = "";
            IOrigin sOrigin = CreateOriginTest(source);
            Assert.AreSame(sOrigin.Begin.Origin, sOrigin);
        }

        [Test]
        public void GetLocationOriginTest()
        {
            string source = "XyZ";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();

            sReader.MoveNext(); // point to first character
            ILocation loc1 = sReader.CurrentLocation;

            sReader.MoveNext();
            ILocation loc2 = sReader.CurrentLocation;

            Assert.AreSame(loc1.Origin, sOrigin);
            Assert.AreSame(loc2.Origin, sOrigin);
        }

        // --------- reading source tests ---------
        [Test]
        public void EmptySourceTest()
        {
            string source = "";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();
            Assert.IsFalse(sReader.MoveNext());
        }

        [Test]
        public void OneCharacterSourceTest()
        {
            string source = "X";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();
            Assert.IsTrue(sReader.MoveNext());
            Assert.AreEqual('X', sReader.CurrentCharacter);
            Assert.IsFalse(sReader.MoveNext());
        }

        [Test]
        public void ReadTheWholeSourceTest()
        {
            string source = "I like reading sources!\nEspecially in tests.";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();

            StringBuilder readString = new StringBuilder();
            while (sReader.MoveNext())
            {
                readString.Append(sReader.CurrentCharacter);
            }
            Assert.AreEqual(source, readString.ToString());
        }

        [Test]
        public void FalseMoveNextCallsTest()
        {
            string source = "I like reading sources!\nEspecially in tests.";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();

            while (sReader.MoveNext());
            Assert.IsFalse(sReader.MoveNext());
            Assert.IsFalse(sReader.MoveNext());
            Assert.IsFalse(sReader.MoveNext());

            // TODO - reading character after false MoveNext should throw an exception
            System.Console.WriteLine("after MoveNext: " + sReader.CurrentCharacter);
        }

        // --------- get/set location tests ---------

        [Test]
        public void SetBeginLocationTest()
        {
            string source = "XyZ";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();
            sReader.MoveNext();
            sReader.MoveNext();
            sReader.CurrentLocation = sOrigin.Begin; // set reader before first character
            sReader.MoveNext();
            Assert.AreEqual('X', sReader.CurrentCharacter);
        }

        [Test]
        public void SetOneLocationTest()
        {
            string source = "XyZ";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();

            sReader.MoveNext(); // point to first character
            ILocation loc1 = sReader.CurrentLocation;
            sReader.CurrentLocation = loc1;
            Assert.AreEqual('X', sReader.CurrentCharacter);
        }

        [Test]
        public void SettingLocationsTest()
        {
            string source = "012";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();

            sReader.MoveNext(); // point to first character
            char char1 = sReader.CurrentCharacter;
            ILocation loc1 = sReader.CurrentLocation;

            sReader.MoveNext(); // second character
            char char2 = sReader.CurrentCharacter;
            ILocation loc2 = sReader.CurrentLocation;

            sReader.MoveNext(); // third character
            char char3 = sReader.CurrentCharacter;
            ILocation loc3 = sReader.CurrentLocation;

            // check if setting location works

            sReader.CurrentLocation = loc1; // restore first character
            Assert.AreEqual(char1, sReader.CurrentCharacter);

            sReader.MoveNext(); // go to the second
            Assert.AreEqual(char2, sReader.CurrentCharacter);

            sReader.CurrentLocation = loc3; // restore third
            Assert.AreEqual(char3, sReader.CurrentCharacter);

            Assert.IsFalse(sReader.MoveNext());

            sReader.CurrentLocation = loc2; // restore second character
            Assert.AreEqual(char2, sReader.CurrentCharacter);
        }

        // --------- location equality tests ---------
        // We assume that two locations are equal if they point to the same place in origin.

        [Test]
        public void InitialReaderLocationTest()
        {
            string source = "";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();
            Assert.AreEqual(sOrigin.Begin, sReader.CurrentLocation);
        }

        [Test]
        public void NotEqualLocationsTest()
        {
            string source = "XXX";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();

            sReader.MoveNext(); // point to first character
            ILocation loc1 = sReader.CurrentLocation;
            sReader.MoveNext(); // second character
            Assert.AreNotEqual(loc1, sReader.CurrentLocation);
        }

        [Test]
        public void FalseMoveNextDoesntAffectLocationTest()
        {
            string source = "XXX";
            IOrigin sOrigin = CreateOriginTest(source);
            IOriginReader sReader = sOrigin.GetReader();

            while (sReader.MoveNext()) ; // move to the end of the origin

            ILocation endLocation = sReader.CurrentLocation;
            sReader.MoveNext();
            sReader.MoveNext();
            Assert.AreEqual(endLocation, sReader.CurrentLocation);
        }
    }
}

