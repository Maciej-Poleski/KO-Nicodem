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
        // --------- FIELDS ---------
        private IOrigin iOrigin;
        private IOriginReader iReader;

        // --------- initialization tests ---------
        [Test]
        public void BeginLocationOriginTest()
        {
            string source = "";
            iOrigin = CreateOriginTest(source);
            Assert.AreSame(iOrigin.Begin.Origin, iOrigin);
        }

        [Test]
        public void GetLocationOriginTest()
        {
            string source = "XyZ";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext(); // point to first character
            ILocation loc1 = iReader.CurrentLocation;

            iReader.MoveNext();
            ILocation loc2 = iReader.CurrentLocation;

            Assert.AreSame(loc1.Origin, iOrigin);
            Assert.AreSame(loc2.Origin, iOrigin);
        }

        // --------- reading source tests ---------
        [Test]
        public void EmptySourceTest()
        {
            string source = "";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();
            Assert.IsFalse(iReader.MoveNext());
        }

        [Test]
        public void OneCharacterSourceTest()
        {
            string source = "X";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();
            Assert.IsTrue(iReader.MoveNext());
            Assert.AreEqual('X', iReader.CurrentCharacter);
            Assert.IsFalse(iReader.MoveNext());
        }

        [Test]
        public void ReadTheWholeSourceTest()
        {
            string source = "I like reading sources!\nEspecially in tests.";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            StringBuilder readString = new StringBuilder();
            while (iReader.MoveNext())
            {
                readString.Append(iReader.CurrentCharacter);
            }
            Assert.AreEqual(source, readString.ToString());
        }

		[Test]
		public void ReadSourceWithEmptyLinesTest()
		{
			string source = "I \n\nlike reading \nsources!\n\n\nEspecially\nin\n tests.";
			iOrigin = CreateOriginTest(source);
			iReader = iOrigin.GetReader();

			StringBuilder readString = new StringBuilder();
			while (iReader.MoveNext())
			{
				readString.Append(iReader.CurrentCharacter);
			}
			Assert.AreEqual(source, readString.ToString());
		}

        [Test]
        public void FalseMoveNextCallsTest()
        {
            string source = "I like reading sources!\nEspecially in tests.";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            while (iReader.MoveNext());
            Assert.IsFalse(iReader.MoveNext());
            Assert.IsFalse(iReader.MoveNext());
            Assert.IsFalse(iReader.MoveNext());
        }

        // --------- get/set location tests ---------

        [Test]
        public void SetBeginLocationTest()
        {
            string source = "XyZ";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();
            iReader.MoveNext();
            iReader.MoveNext();
            iReader.CurrentLocation = iOrigin.Begin; // set reader before first character
            iReader.MoveNext();
            Assert.AreEqual('X', iReader.CurrentCharacter);
        }

        [Test]
        public void SetOneLocationTest()
        {
            string source = "XyZ";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext(); // point to first character
            Assert.AreEqual('X', iReader.CurrentCharacter);
            ILocation loc1 = iReader.CurrentLocation;
            iReader.CurrentLocation = loc1;
            Assert.AreEqual('X', iReader.CurrentCharacter);
        }

        [Test]
        public void SettingLocationsTest()
        {
            string source = "012";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext(); // point to first character
            char char1 = iReader.CurrentCharacter;
            ILocation loc1 = iReader.CurrentLocation;

            iReader.MoveNext(); // second character
            char char2 = iReader.CurrentCharacter;
            ILocation loc2 = iReader.CurrentLocation;

            iReader.MoveNext(); // third character
            char char3 = iReader.CurrentCharacter;
            ILocation loc3 = iReader.CurrentLocation;

            // check if setting location works

            iReader.CurrentLocation = loc1; // restore first character
            Assert.AreEqual(char1, iReader.CurrentCharacter);

            iReader.MoveNext(); // go to the second
            Assert.AreEqual(char2, iReader.CurrentCharacter);

            iReader.CurrentLocation = loc3; // restore third
            Assert.AreEqual(char3, iReader.CurrentCharacter);

            Assert.IsFalse(iReader.MoveNext());

            iReader.CurrentLocation = loc2; // restore second character
            Assert.AreEqual(char2, iReader.CurrentCharacter);
        }

        // --------- location equality tests ---------
        // We assume that two locations are equal if they point to the same place in origin.

        [Test]
        public void InitialReaderLocationTest()
        {
            string source = "";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();
            Assert.AreEqual(iOrigin.Begin, iReader.CurrentLocation);
        }

        [Test]
        public void NotEqualLocationsTest()
        {
            string source = "XXX";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext(); // point to first character
            ILocation loc1 = iReader.CurrentLocation;
            iReader.MoveNext(); // second character
            Assert.AreNotEqual(loc1, iReader.CurrentLocation);
        }

        [Test]
        public void FalseMoveNextDoesntAffectLocationTest()
        {
            string source = "XXX";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            while (iReader.MoveNext()) ; // move to the end of the origin

            ILocation endLocation = iReader.CurrentLocation;
            iReader.MoveNext();
            iReader.MoveNext();
            Assert.AreEqual(endLocation, iReader.CurrentLocation);
        }

        // --------- tear down method ---------
        [TearDown]
        public void DisposeReader()
        {
            if (iReader != null)
            {
                iReader.Dispose();
            }
        }
    }
}

