using NUnit.Framework;
using Nicodem.Lexer;
using System.Linq;
using System;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExDerivativeTests
	{
        /// <summary>
        /// Function which check if every of given regEx'es is unique.
        /// </summary>
        /// <param name="regExes">Any number of Regular Expresions</param>
        /// <returns>True if every RegEx is unique</returns>
        public bool derivativeChangesOfRegexesAreUnique(params RegEx<char>[] regExes)
        {
            for(int i=0; i<regExes.Length; i++)
                if (!derivativeChangesIsUnique(regExes[i]))
                    return false;
            return true;
        }

        /// <summary>
        /// Function which check if regEx is unique
        /// </summary>
        /// <param name="regEx">RegularExpresion</param>
        /// <returns>True if RegEx is unique</returns>
        public bool derivativeChangesIsUnique(RegEx<char> regEx)
        {
            char prev = (char)0xffff;
            foreach(var c in regEx.DerivChanges())
                if(c.Equals(prev))
                    return false;
            return true;
        }

		// empty^a = empty
		[Test]
		public void Test_Empty()
		{
			var empty = RegExFactory.Empty<char> ();
			var derivChanges = empty.DerivChanges ().ToList ();
			Assert.AreEqual (0, derivChanges.Count);

			for (var c = 'a'; c <= 'z'; c++) {
                var deriv = empty.Derivative(c);
                Assert.IsTrue(derivativeChangesIsUnique(deriv));
				Assert.AreEqual (empty, deriv);
			}
		}

		// a^a = {epsi} = empty*
		[Test]
		public void Test_SingleLetterMatch()
		{
			var regex = RegExFactory.Range ('a');
			var derivChanges = regex.DerivChanges ().ToArray ();

			Assert.AreEqual (1, derivChanges.Length);
			Assert.AreEqual ('a', derivChanges [0]);

			var deriv = regex.Derivative ('a');
			var singl_empty = RegExFactory.Epsilon<char> ();

			Assert.AreEqual (singl_empty, deriv);
		}

		// a^b = empty  when b != a
		[Test]
		public void Test_SingleLetterMismatch()
		{
            var regex = RegExFactory.Range('a');
            Assert.IsTrue(derivativeChangesIsUnique(regex));
			var derivChanges = regex.DerivChanges ().ToArray ();

			Assert.AreEqual (1, derivChanges.Length);
			Assert.AreEqual ('a', derivChanges [0]);

			var deriv = regex.Derivative ('b');
			Assert.AreEqual (RegExFactory.Empty<char> (), deriv);
		}

		// sum(X, Y)^a = sum(X^a, Y^a)
		[Test]
		public void Test_Sum()
		{
			var regA = RegExFactory.Range ('a');
			var regB = RegExFactory.Range ('b');
            var regC = RegExFactory.Range('c');

			var regAB = RegExFactory.Union (regA, regB);
			var regBC = RegExFactory.Union (regB, regC);
			var regABC = RegExFactory.Union (regA, regB, regC);

			var derivA = regA.Derivative ('a');
			var derivB = regB.Derivative ('a');
			var derivC = regC.Derivative ('a');

			var derivAB = regAB.Derivative ('a');
			var derivBC = regBC.Derivative ('a');
			var derivABC = regABC.Derivative ('a');

			Assert.AreEqual (derivAB, RegExFactory.Union (derivA, derivB));
			Assert.AreEqual (derivBC, RegExFactory.Union (derivB, derivC));
			Assert.AreEqual (derivABC, RegExFactory.Union (derivA, derivB, derivC));

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(regA, regAB, regABC, regB, regBC, regC, derivA, derivAB, derivABC, derivB, derivBC, derivC));
		}

		// intersect(X, Y)^a = intersect(X^a, Y^a)
		[Test]
		public void Test_Intersect()
		{
			var regA = RegExFactory.Range ('a');
			var regB = RegExFactory.Range ('b');
			var regC = RegExFactory.Range ('c');

			var regAB = RegExFactory.Intersection (regA, regB);
			var regBC = RegExFactory.Intersection (regB, regC);
			var regABC = RegExFactory.Intersection (regA, regB, regC);

			var derivA = regA.Derivative ('a');
			var derivB = regB.Derivative ('a');
			var derivC = regC.Derivative ('a');

			var derivAB = regAB.Derivative ('a');
			var derivBC = regBC.Derivative ('a');
			var derivABC = regABC.Derivative ('a');

			Assert.AreEqual (derivAB, RegExFactory.Intersection (derivA, derivB));
			Assert.AreEqual (derivBC, RegExFactory.Intersection (derivB, derivC));
			Assert.AreEqual (derivABC, RegExFactory.Intersection (derivA, derivB, derivC));

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(regA, regAB, regABC, regB, regBC, regC, derivA, derivAB, derivABC, derivB, derivBC, derivC));
		}

		// complement(X)^a = complement(X^a)
		[Test]
		public void Test_Complement()
		{
			var regA = RegExFactory.Range ('a');
			var regB = RegExFactory.Range ('b');
			var derivA = regA.Derivative ('a');
			var derivB = regB.Derivative ('b');

			var complA = RegExFactory.Complement (regA);
			var complB = RegExFactory.Complement (regB);
			var derivComplA = complA.Derivative ('a');
			var derivComplB = complB.Derivative ('b');

			Assert.AreEqual (derivComplA, RegExFactory.Complement (derivA));
			Assert.AreEqual (derivComplB, RegExFactory.Complement (derivB));

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(RegExFactory.Complement(derivA), RegExFactory.Complement(derivB)));
		}

		// star(X)^a = concat(X^a, star(X))
		[Test]
		public void Test_Star()
		{
			var regA = RegExFactory.Range ('a');
			var regB = RegExFactory.Range ('b');
			var derivA = regA.Derivative ('a');
			var derivB = regB.Derivative ('b');

			var starA = RegExFactory.Star (regA);
			var starB = RegExFactory.Star (regB);
			var derivStarA = starA.Derivative ('a');
			var derivStarB = starB.Derivative ('b');

			Assert.AreEqual (derivStarA, RegExFactory.Concat (derivA, starA));
			Assert.AreEqual (derivStarB, RegExFactory.Concat (derivB, starB));

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(starA, starB, derivStarA, derivStarB, RegExFactory.Concat(derivA, starA), RegExFactory.Concat(derivB, starB)));
		}

		// concat(XY)^a = X^aY  if epsi not in X
		[Test]
		public void Test_Concat_XY_withoutEpsi()
		{
			var X = RegExFactory.Range ('a'); // does not contain epsi
			var Y = RegExFactory.Range ('b');
			var derivX = X.Derivative ('a');

			var concat = RegExFactory.Concat (X, Y);
			var derivConcat = concat.Derivative ('a');

			Assert.IsFalse (X.HasEpsilon ());
			Assert.AreEqual (derivConcat, RegExFactory.Concat (derivX, Y));

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(concat, derivConcat, RegExFactory.Concat(derivX, Y)));
		}

		// concat(XY)^a = sum(X^aY, Y^a)  if epsi in X
		[Test]
		public void Test_Concat_XY_withEpsi()
		{
			var reg = RegExFactory.Range ('a');
			var X = RegExFactory.Star (reg); // contains epsi
			var Y = RegExFactory.Range ('b');

			var derivX = X.Derivative ('a');
			var derivY = Y.Derivative ('a');

			var concat = RegExFactory.Concat (X, Y);
			var derivConcat = concat.Derivative ('a');

			Assert.IsTrue (X.HasEpsilon ());
			Assert.AreEqual (derivConcat, RegExFactory.Union (RegExFactory.Concat (derivX, Y), derivY));
            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(reg, X, Y, derivX, derivY, concat, derivConcat, RegExFactory.Union(RegExFactory.Concat(derivX, Y), derivY)));
		}

		// concat(XYZW)^a = sum(X^aYZW, Y^aZW, Z^aW, W^a)  if epsi in X, Y, Z
		[Test]
		public void Test_Concat_XYZW_withEpsiXYZ()
		{
			var regX = RegExFactory.Range ('a');
			var regY = RegExFactory.Range ('b');
			var regZ = RegExFactory.Range ('c');
			var X = RegExFactory.Star (regX); // contains epsi
			var Y = RegExFactory.Star (regY); // contains epsi
			var Z = RegExFactory.Star (regZ); // contains epsi
			var W = RegExFactory.Range ('d');

			var derivX = X.Derivative ('a');
			var derivY = Y.Derivative ('a');
			var derivZ = Z.Derivative ('a');
			var derivW = W.Derivative ('a');

			var concat = RegExFactory.Concat (X, Y, Z, W);
			var derivConcat = concat.Derivative ('a');

			Assert.IsTrue (X.HasEpsilon ());
			Assert.IsTrue (Y.HasEpsilon ());
			Assert.IsTrue (Z.HasEpsilon ());
			Assert.IsFalse (W.HasEpsilon ());
			Assert.AreEqual (derivConcat,
				RegExFactory.Union (
					RegExFactory.Concat (derivX, Y, Z, W),
					RegExFactory.Concat (derivY, Z, W),
					RegExFactory.Concat (derivZ, W),
					derivW
				));

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(concat, derivConcat, RegExFactory.Union(RegExFactory.Concat (derivX, Y, Z, W), RegExFactory.Concat (derivY, Z, W), RegExFactory.Concat (derivZ, W), derivW) ));
		}

		// concat(XYZ)^a = sum(X^aYZ, Y^aZ, Z^a)  if epsi in X, Y
		[Test]
		public void Test_Concat_XYZ_withEpsiXY()
		{
			var regX = RegExFactory.Range ('a');
			var regY = RegExFactory.Range ('b');
			var X = RegExFactory.Star (regX); // contains epsi
			var Y = RegExFactory.Star (regY); // contains epsi
			var Z = RegExFactory.Range ('c');

			var derivX = X.Derivative ('a');
			var derivY = Y.Derivative ('a');
			var derivZ = Z.Derivative ('a');

			var concat = RegExFactory.Concat (X, Y, Z);
			var derivConcat = concat.Derivative ('a');

			Assert.IsTrue (X.HasEpsilon ());
			Assert.IsTrue (Y.HasEpsilon ());
			Assert.IsFalse (Z.HasEpsilon ());
			Assert.AreEqual (derivConcat,
				RegExFactory.Union (
					RegExFactory.Concat (derivX, Y, Z),
					RegExFactory.Concat (derivY, Z),
					derivZ
				));

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(concat, derivConcat, RegExFactory.Union(RegExFactory.Concat(derivX, Y, Z), RegExFactory.Concat(derivY, Z), derivZ) ));
		}

		// concat(XYZ)^a = sum(X^aYZ, Y^aZ)  if epsi in X
		[Test]
		public void Test_Concat_XYZ_withEpsiX()
		{
			var regX = RegExFactory.Range ('a');
			var X = RegExFactory.Star (regX); // contains epsi
			var Y = RegExFactory.Range ('b');
			var Z = RegExFactory.Range ('c');

			var derivX = X.Derivative ('a');
			var derivY = Y.Derivative ('a');

			var concat = RegExFactory.Concat (X, Y, Z);
			var derivConcat = concat.Derivative ('a');

			Assert.IsTrue (X.HasEpsilon ());
			Assert.IsFalse (Y.HasEpsilon ());
			Assert.IsFalse (Z.HasEpsilon ());
			Assert.AreEqual (derivConcat,
				RegExFactory.Union (
					RegExFactory.Concat (derivX, Y, Z),
					RegExFactory.Concat (derivY, Z)
				));
            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(derivConcat, RegExFactory.Union(RegExFactory.Concat(derivX, Y, Z), RegExFactory.Concat(derivY, Z))));
		}

		// concat(XYZ)^a = X^aYZ  if epsi not in X
		[Test]
		public void Test_Concat_XYZ_withoutEpsi()
		{
			var X = RegExFactory.Range ('a');
			var Y = RegExFactory.Range ('b');
			var Z = RegExFactory.Range ('c');

			var derivX = X.Derivative ('a');

			var concat = RegExFactory.Concat (X, Y, Z);
			var derivConcat = concat.Derivative ('a');

			Assert.IsFalse (X.HasEpsilon ());
			Assert.IsFalse (Y.HasEpsilon ());
			Assert.IsFalse (Z.HasEpsilon ());
			Assert.AreEqual (derivConcat, RegExFactory.Concat (derivX, Y, Z));
            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(concat, derivConcat, RegExFactory.Concat(derivX, Y, Z)));
		}

        // (a*b)^b = {epsi}
        [Test]
        public void RegExStarSimpleDerivativeTest()
        {
            var aStar = RegExFactory.Star(RegExFactory.Range('a', 'b'));
            var b = RegExFactory.Range('b', 'c');
            var aStarb = RegExFactory.Concat(aStar, b);

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(aStar, b, aStarb, aStarb.Derivative('b')));
            Assert.AreEqual(RegExFactory.Epsilon<char>(), aStarb.Derivative('b'));
        }

        // (a*b*c*d*)^d = d*
		[Test]
		public void RegExStarComplexDerivativeTest()
		{
			var aStar = RegExFactory.Star (RegExFactory.Range ('a', 'b'));
			var bStar = RegExFactory.Star (RegExFactory.Range ('b', 'c'));
			var cStar = RegExFactory.Star (RegExFactory.Range ('c', 'd'));
			var dStar = RegExFactory.Star (RegExFactory.Range ('d', 'e'));
			var aStarbStarcStardStar = RegExFactory.Concat(aStar, bStar, cStar, dStar);

            Assert.IsTrue(derivativeChangesOfRegexesAreUnique(aStar, bStar, cStar, dStar, aStarbStarcStardStar, aStarbStarcStardStar.Derivative('d')));
			Assert.AreEqual(dStar, aStarbStarcStardStar.Derivative('d'));
		}
	}
}

