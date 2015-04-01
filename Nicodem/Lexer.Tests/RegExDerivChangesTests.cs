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
	}
}

