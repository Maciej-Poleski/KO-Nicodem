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
            while (iReader.MoveNext()) {
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
            while (iReader.MoveNext()) {
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

            while (iReader.MoveNext()) ;
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

        // --------- location position tests ---------

        [Test]
        public void InitialLocationPositionTest()
        {
            string source = "";
            iOrigin = CreateOriginTest(source);

            OriginPosition pos = iOrigin.Begin.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 0));
        }

        [Test]
        public void EmptySourcePositionTest()
        {
            string source = "";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            OriginPosition pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 0));

            Assert.IsFalse(iReader.MoveNext());
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 0));
        }

        [Test]
        public void OneLinePositionsTest()
        {
            string source = "1234";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();
            OriginPosition pos;

            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 0));

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 1));

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 2));

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 3));
        }

        [Test]
        public void OneLineEndPositionTest()
        {
            string source = "1234";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            while(iReader.MoveNext());

            OriginPosition pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 4));
        }

        [Test]
        public void FewLinesPositionTest()
        {
            string source = "1\n22\n\n4";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();
            OriginPosition pos;

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 1) && (pos.CharNumber == 1));

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 2) && (pos.CharNumber == 0));

            iReader.MoveNext();
            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 2) && (pos.CharNumber == 2));

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 3) && (pos.CharNumber == 0));

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 4) && (pos.CharNumber == 0));

            iReader.MoveNext();
            pos = iReader.CurrentLocation.GetOriginPosition();
            Assert.IsTrue((pos.LineNumber == 4) && (pos.CharNumber == 1));
        }

        // --------- fragment positions tests ---------

        [Test]
        public void EmptySourceFragmentPositionsTest()
        {
            string source = "";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            ILocation loc1 = iReader.CurrentLocation;
            iReader.MoveNext();
            ILocation loc2 = iReader.CurrentLocation;

            IFragment fr = iOrigin.MakeFragment(loc1, loc2);

            Assert.AreEqual(fr.GetBeginOriginPosition(), loc1.GetOriginPosition());
            Assert.AreEqual(fr.GetEndOriginPosition(), loc2.GetOriginPosition());
        }


        [Test]
        public void OneLineFragmentPositionsTest()
        {
            string source = "1234";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext();
            ILocation loc1 = iReader.CurrentLocation;
            iReader.MoveNext();
            iReader.MoveNext();
            ILocation loc2 = iReader.CurrentLocation;
            iReader.MoveNext();

            IFragment fr = iOrigin.MakeFragment(loc1, loc2);

            Assert.AreEqual(fr.GetBeginOriginPosition(), loc1.GetOriginPosition());
            Assert.AreEqual(fr.GetEndOriginPosition(), loc2.GetOriginPosition());
        }

        [Test]
        public void FewLinesFragmentPositionsTest()
        {
            string source = "1\n22\n\n4";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext();
            ILocation loc1 = iReader.CurrentLocation;
            iReader.MoveNext();
            iReader.MoveNext();
            ILocation loc2 = iReader.CurrentLocation;
            iReader.MoveNext();

            IFragment fr = iOrigin.MakeFragment(loc1, loc2);
            Assert.AreEqual(fr.GetBeginOriginPosition(), loc1.GetOriginPosition());
            Assert.AreEqual(fr.GetEndOriginPosition(), loc2.GetOriginPosition());

            iReader.MoveNext();
            iReader.MoveNext();
            ILocation loc3 = iReader.CurrentLocation;

            IFragment fr2 = iOrigin.MakeFragment(loc2, loc3);
            Assert.AreEqual(fr2.GetBeginOriginPosition(), loc2.GetOriginPosition());
            Assert.AreEqual(fr2.GetEndOriginPosition(), loc3.GetOriginPosition());
        }

        // --------- fragment get text tests ---------

        [Test]
        public void EmptySourceFragmentTextTest()
        {
            string source = "";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            ILocation loc1 = iReader.CurrentLocation;
            iReader.MoveNext();
            ILocation loc2 = iReader.CurrentLocation;

            IFragment fr = iOrigin.MakeFragment(loc1, loc2);
            Assert.AreEqual(fr.GetOriginText(), "");
        }

        [Test]
        public void OneLineFragmentTextTest()
        {
            string source = "1234";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext();
            ILocation loc1 = iReader.CurrentLocation;
            iReader.MoveNext();
            iReader.MoveNext();
            ILocation loc2 = iReader.CurrentLocation;
            iReader.MoveNext();

            IFragment fr = iOrigin.MakeFragment(loc1, loc2);
            Assert.AreEqual(fr.GetOriginText(), "23");
        }

        [Test]
        public void FewLinesFragmentTextTest()
        {
            string source = "1\n22\n\n4";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            iReader.MoveNext(); // after '1'
            ILocation loc1 = iReader.CurrentLocation;
            iReader.MoveNext(); // after '\n'
            iReader.MoveNext(); // after '2'
            ILocation loc2 = iReader.CurrentLocation;
            iReader.MoveNext(); // after '2'

            Assert.AreEqual(iOrigin.MakeFragment(loc1, loc2).GetOriginText(), "\n2");

            ILocation loc3 = iReader.CurrentLocation;
            iReader.MoveNext(); // after '\n'
            iReader.MoveNext(); // after '\n'
            ILocation loc4 = iReader.CurrentLocation;

            Assert.AreEqual(iOrigin.MakeFragment(loc3, loc4).GetOriginText(), "\n\n");

            iReader.MoveNext(); // after '4'
            ILocation loc5 = iReader.CurrentLocation;

            Assert.AreEqual(iOrigin.MakeFragment(loc4, loc5).GetOriginText(), "4");
        }

        [Test]
        public void WholeSourceFragmentTextTest()
        {
            string source = "I like reading sources!\nEspecially in tests.";
            iOrigin = CreateOriginTest(source);
            iReader = iOrigin.GetReader();

            ILocation locBeg = iReader.CurrentLocation;
            while (iReader.MoveNext()) ; // move to the end of source
            ILocation locEnd = iReader.CurrentLocation;
            
            IFragment frAll = iOrigin.MakeFragment(locBeg, locEnd);
            Assert.AreEqual(frAll.GetOriginText(), source);
        }

        // --------- tear down method ---------
        [TearDown]
        public void DisposeReader()
        {
            if (iReader != null) {
                iReader.Dispose();
            }
        }
    }
}

