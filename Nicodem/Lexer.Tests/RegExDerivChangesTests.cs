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
			throw new NotImplementedException ();
		}

		[Test]
		public void Test_Union()
		{
			throw new NotImplementedException ();
		}

		[Test]
		public void Test_Intersection()
		{
			throw new NotImplementedException ();
		}

		[Test]
		public void Test_Concat()
		{
			throw new NotImplementedException ();
		}
	}
}

