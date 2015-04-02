using NUnit.Framework;
using Nicodem.Lexer;
using System.Linq;
using System;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExDerivativeTests
	{
		private static RegEx<char> singleton(char c)
		{
			return RegExFactory.Range (c, (char)(c + 1));
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
				Assert.AreEqual (empty, deriv);
			}
		}

		// a^a = {epsi} = empty*
		[Test]
		public void Test_Range()
		{
			var regex = RegExFactory.Range ('b');
			var derivChanges = regex.DerivChanges ().ToArray ();

			Assert.AreEqual (1, derivChanges.Length);
			Assert.AreEqual ('b', derivChanges [0]);

			var derivA = regex.Derivative ('a');
			var derivB = regex.Derivative ('b');
			var derivC = regex.Derivative ('c');

			Assert.AreEqual (RegExFactory.Empty<char> (), derivA);
			Assert.AreEqual (RegExFactory.Epsilon<char> (), derivB);
			Assert.AreEqual (RegExFactory.Epsilon<char> (), derivC);
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
		}

		#region Star

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
		}

		// (((a*)^a)^a)^a = a*
		[Test]
		public void Test_StarTripleDerivative()
		{
			var star = RegExFactory.Star (singleton ('a'));
			var dStar = star.Derivative('a');
			var ddStar = dStar.Derivative('a');
			var dddStar = ddStar.Derivative ('a');

			Assert.AreEqual(star, dddStar);
		}

		// (a*b*)*^a = (a*b*)(a*b*)*
		// (a*b*)*^b = b*(a*b*)*
		[Test]
		public void Test_Star_AstarBstar()
		{
			var aStar = RegExFactory.Star (singleton ('a'));
			var bStar = RegExFactory.Star (singleton ('b'));

			var aStarbStar = RegExFactory.Concat (aStar, bStar);
			var aStarbStarSTAR = RegExFactory.Star (aStarbStar);

			var expectedA = RegExFactory.Concat (aStarbStar, aStarbStarSTAR);
			var expectedB = RegExFactory.Concat (bStar, aStarbStarSTAR);

			Assert.AreEqual(expectedA, aStarbStarSTAR.Derivative('a'));
			Assert.AreEqual(expectedB, aStarbStarSTAR.Derivative('b'));
		}

		#endregion

		#region Concat

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
			Assert.AreEqual (RegExFactory.Union (RegExFactory.Concat (derivX, Y), derivY), derivConcat);
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
			Assert.AreEqual (
				RegExFactory.Union (
					RegExFactory.Concat (derivX, Y, Z, W),
					RegExFactory.Concat (derivY, Z, W),
					RegExFactory.Concat (derivZ, W),
					derivW
				), derivConcat);
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
			Assert.AreEqual (
				RegExFactory.Union (
					RegExFactory.Concat (derivX, Y, Z),
					RegExFactory.Concat (derivY, Z),
					derivZ
				), derivConcat);
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
			Assert.AreEqual (
				RegExFactory.Union (
					RegExFactory.Concat (derivX, Y, Z),
					RegExFactory.Concat (derivY, Z)
				), derivConcat);
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
		}

		// (a*b)^b = {epsi}
		[Test]
		public void Test_Concat_AstarB()
		{
			var regAStar = RegExFactory.Star (singleton ('a'));
			var regB = singleton ('b');

			var regex = RegExFactory.Concat(regAStar, regB);
			var deriv = regex.Derivative ('b');

			Assert.AreEqual (RegExFactory.Epsilon<char> (), deriv);
		}

		// (a*b*c*d*)^a = (a*)^ab*c*d* + (b*)^ac*d* + (c*)^ad* + (d*)^a = (a^a)a*b*c*d* + (emp x 3) = a*b*c*d*
		// (a*b*c*d*)^b = (a*)^bb*c*d* + (b*)^bc*d* + (c*)^bd* + (d*)^b = emp + (b^b)b*c*d* + emp x 2 = b*c*d*
		// (a*b*c*d*)^c = ... = c*d*
		// (a*b*c*d*)^d = ... = d*
		// (a*b*c*d*)^x = ... = emp
		[Test]
		public void Test_Concat_AstarBstarCstarDstar()
		{
			var aStar = RegExFactory.Star (singleton ('a'));
			var bStar = RegExFactory.Star (singleton ('b'));
			var cStar = RegExFactory.Star (singleton ('c'));
			var dStar = RegExFactory.Star (singleton ('d'));

			var aSbScSdS = RegExFactory.Concat(aStar, bStar, cStar, dStar);
			var bScSdS = RegExFactory.Concat(bStar, cStar, dStar);
			var cSdS = RegExFactory.Concat(cStar, dStar);

			Assert.AreEqual(aSbScSdS, aSbScSdS.Derivative('a'));
			Assert.AreEqual(bScSdS, aSbScSdS.Derivative('b'));
			Assert.AreEqual(cSdS, aSbScSdS.Derivative('c'));
			Assert.AreEqual(dStar, aSbScSdS.Derivative('d'));
			Assert.AreEqual (RegExFactory.Empty<char> (), aSbScSdS.Derivative ('x'));
		}

		#endregion
	}
}

