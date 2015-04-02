using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Lexer;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExDerivChangesTests
	{
		private static RegEx<char> singleton(char c)
		{
			return RegExFactory.Range (c, (char)(c + 1));
		}

		/// <summary>
		/// Check if given list contains unique elements and is sorted
		/// </summary>
		/// <param name="l">L.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		static bool isUniqueAndSorted<T>(IEnumerable<T> l) where T : IComparable<T>
		{
			var arr = l.ToArray ();

			// empty list is sorted and unique
			if (arr.Length == 0)
				return true;

			var previous = arr [0];

			var s = new HashSet<T> ();
			s.Add (previous);

			for (var i = 1; i < arr.Length; ++i) {

				// found duplicate
				if (s.Contains (arr [i]))
					return false;

				// found not sorted elements
				if (previous.CompareTo (arr [i]) > 0)
					return false;

				s.Add (arr [i]);
				previous = arr [i];
			}

			return true;
		}

		static IEnumerable<T> joinEnumerables<T>( params IEnumerable<T>[] lists )
		{
			var S = new HashSet<T> ();

			foreach (var list in lists)
				foreach (var elem in list)
					S.Add (elem);

			return S;
		}

		[Test]
		public void Test_Range()
		{
			var regex = RegExFactory.Range ('a');
			var changes = regex.DerivChanges ();

			Assert.AreEqual (1, changes.Count ());
			Assert.IsTrue (isUniqueAndSorted (changes));
		}

		[Test]
		public void Test_Star()
		{
			var regex = RegExFactory.Range ('a');
			var star = RegExFactory.Star (regex);

			var regexChanges = regex.DerivChanges ();
			var starChanges = star.DerivChanges ();

			Assert.AreEqual (regexChanges, starChanges);
			Assert.IsTrue (isUniqueAndSorted (starChanges));
		}

		[Test]
		public void Test_Complement()
		{
			var regex = RegExFactory.Range ('a');
			var compl = RegExFactory.Complement (regex);

			var regexChanges = regex.DerivChanges ();
			var complChanges = compl.DerivChanges ();

			Assert.AreEqual (regexChanges, complChanges);
			Assert.IsTrue (isUniqueAndSorted (complChanges));
		}

		[Test]
		public void Test_Union()
		{
			var regA = RegExFactory.Range ('a');
			var regB = RegExFactory.Range ('b');
			var regC = RegExFactory.Range ('c');
			var union = RegExFactory.Union (regA, regB, regC);

			var unionChanges = union.DerivChanges ();
			var expectedChanges = joinEnumerables (regA.DerivChanges (), regB.DerivChanges (), regC.DerivChanges ());

			Assert.IsTrue (isUniqueAndSorted (unionChanges));
			Assert.AreEqual (expectedChanges, unionChanges);
		}

		[Test]
		public void Test_Intersection()
		{
			var regA = RegExFactory.Range ('a');
			var regB = RegExFactory.Range ('b');
			var regC = RegExFactory.Range ('c');
			var inter = RegExFactory.Intersection (regA, regB, regC);

			var interChanges = inter.DerivChanges ();
			var expectedChanges = joinEnumerables (regA.DerivChanges (), regB.DerivChanges (), regC.DerivChanges ());

			Assert.IsTrue (isUniqueAndSorted (interChanges));
			Assert.AreEqual (expectedChanges, interChanges);
		}

		[Test]
		public void Test_Concat_XYZ_withoutEpsi()
		{
			var regA = RegExFactory.Range ('a');
			var regB = RegExFactory.Range ('b');
			var regC = RegExFactory.Range ('c');
			var concat = RegExFactory.Concat (regA, regB, regC);

			var concatChanges = concat.DerivChanges ();
			var expectedChanges = regA.DerivChanges ();

			Assert.IsTrue (isUniqueAndSorted (concatChanges));
			Assert.AreEqual (expectedChanges, concatChanges);
		}

		[Test]
		public void Test_Concat_XYZ_withEpsiX()
		{
			var regA = RegExFactory.Star (RegExFactory.Range ('a'));
			var regB = RegExFactory.Range ('b');
			var regC = RegExFactory.Range ('c');
			var concat = RegExFactory.Concat (regA, regB, regC);

			var concatChanges = concat.DerivChanges ();
			var expectedChanges = joinEnumerables (regA.DerivChanges (), regB.DerivChanges ());

			Assert.IsTrue (isUniqueAndSorted (concatChanges));
			Assert.AreEqual (expectedChanges, concatChanges);
		}

		[Test]
		public void Test_Concat_XYZ_withEpsiXZ()
		{
			var regA = RegExFactory.Star (RegExFactory.Range ('a'));
			var regB = RegExFactory.Range ('b');
			var regC = RegExFactory.Star (RegExFactory.Range ('c'));
			var concat = RegExFactory.Concat (regA, regB, regC);

			var concatChanges = concat.DerivChanges ();
			var expectedChanges = joinEnumerables (regA.DerivChanges (), regB.DerivChanges ());

			Assert.IsTrue (isUniqueAndSorted (concatChanges));
			Assert.AreEqual (expectedChanges, concatChanges);
		}

		[Test]
		public void Test_Concat_XYZ_withEpsiXY()
		{
			var regA = RegExFactory.Star (RegExFactory.Range ('a'));
			var regB = RegExFactory.Star (RegExFactory.Range ('b'));
			var regC = RegExFactory.Range ('c');
			var concat = RegExFactory.Concat (regA, regB, regC);

			var concatChanges = concat.DerivChanges ();
			var expectedChanges = joinEnumerables (regA.DerivChanges (), regB.DerivChanges (), regC.DerivChanges ());

			Assert.IsTrue (isUniqueAndSorted (concatChanges));
			Assert.AreEqual (expectedChanges, concatChanges);
		}

		[Test]
		public void Test_Concat_XYZ_withEpsiXYZ()
		{
			var regA = RegExFactory.Star (RegExFactory.Range ('a'));
			var regB = RegExFactory.Star (RegExFactory.Range ('b'));
			var regC = RegExFactory.Star (RegExFactory.Range ('c'));
			var concat = RegExFactory.Concat (regA, regB, regC);

			var concatChanges = concat.DerivChanges ();
			var expectedChanges = joinEnumerables (regA.DerivChanges (), regB.DerivChanges (), regC.DerivChanges ());

			Assert.IsTrue (isUniqueAndSorted (concatChanges));
			Assert.AreEqual (expectedChanges, concatChanges);
		}

		// (a*b)^a = (a*)^ab + b^a = (a^a)a*b + emp = a*b
		// (a*b)^b = (a*)^bb + b^b = (a^b)a*b + epsi = empa*b+ epsi = epsi
		// (a*b)^c = emp
		[Test]
		public void Test_Concat_AstarB()
		{
			var regAStar = RegExFactory.Star (singleton ('a'));
			var regB = singleton ('b');
			var regex = RegExFactory.Concat(regAStar, regB);

			var changes = regex.DerivChanges ();
			var expectedChanges = new char[] { 'a', 'b', 'c' };

			Assert.IsTrue (isUniqueAndSorted (changes));
			Assert.AreEqual (expectedChanges, changes);
		}

		// (a*b*c*d*)^a = (a*)^ab*c*d* + (b*)^ac*d* + (c*)^ad* + (d*)^a = (a^a)a*b*c*d* + (emp x 3) = a*b*c*d*
		// (a*b*c*d*)^b = (a*)^bb*c*d* + (b*)^bc*d* + (c*)^bd* + (d*)^b = emp + (b^b)b*c*d* + emp x 2 = b*c*d*
		// (a*b*c*d*)^c = ... = c*d*
		// (a*b*c*d*)^d = ... = d*
		// (a*b*c*d*)^e = ... = emp = (a*b*c*d*)^0
		[Test]
		public void Test_Concat_AstarBstarCstarDstar_derivD()
		{
			var aStar = RegExFactory.Star (singleton ('a'));
			var bStar = RegExFactory.Star (singleton ('b'));
			var cStar = RegExFactory.Star (singleton ('c'));
			var dStar = RegExFactory.Star (singleton ('d'));
			var aSbScSdS = RegExFactory.Concat(aStar, bStar, cStar, dStar);

			var changes = aSbScSdS.DerivChanges ();
			var expectedChanges = new char[] { 'a', 'b', 'c', 'd', 'e' };

			Assert.IsTrue (isUniqueAndSorted (changes));
			Assert.AreEqual (expectedChanges, changes);
		}

		// (a*b*)*^a = (a*b*)(a*b*)*
		// (a*b*)*^b = b*(a*b*)*
		// (a*b*)*^c = emp
		[Test]
		public void Test_Star_AstarBstar()
		{
			var aStar = RegExFactory.Star (singleton ('a'));
			var bStar = RegExFactory.Star (singleton ('b'));

			var aSbS = RegExFactory.Concat (aStar, bStar);
			var aSbS_Star = RegExFactory.Star (aSbS);

			var changes = aSbS_Star.DerivChanges ();
			var expectedChanges = new char[]{ 'a', 'b', 'c' };

			Assert.IsTrue (isUniqueAndSorted (changes));
			Assert.AreEqual (expectedChanges, changes);
		}
	}
}

