using Nicodem.Lexer;
using NUnit.Framework;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExFactoryTests
	{
		#region Sum

		// sum() = empty
		[Test]
		public void Test_Sum_Empty() 
		{
			var empty = RegExFactory.Empty<char> ();
			var empty_sum = RegExFactory.Union<char> ();

			Assert.AreEqual (0, empty_sum.CompareTo (empty));
		}

		// sum(X, X) = X
		[Test]
		public void Test_Sum_XX()
		{
			var base_regex = RegExFactory.Range ('a');
			var sum = RegExFactory.Union (base_regex, base_regex);

			Assert.IsTrue (sum is RegExRange<char>);
			Assert.AreEqual (0, sum.CompareTo (base_regex));
		}

		// sum(X, Y) = sum(Y, X)
		[Test]
		public void Test_Sum_XY()
		{
			var regex1 = RegExFactory.Range ('a');
			var regex2 = RegExFactory.Range ('b');
			var union1 = RegExFactory.Union (regex1, regex2);
			var union2 = RegExFactory.Union (regex2, regex1);

			Assert.AreEqual (0, union1.CompareTo (union2));
		}

		// sum(X, Y, X) = sum(Y, X)
		[Test]
		public void Test_Sum_XYX()
		{
			var regex1 = RegExFactory.Range ('a');
			var regex2 = RegExFactory.Range ('b');
			var union1 = RegExFactory.Union (regex1, regex2, regex1);
			var union2 = RegExFactory.Union (regex2, regex1);

			Assert.AreEqual (0, union1.CompareTo (union2));
		}

        [Test]
        public void Test_Sum_Associativity()
        {
            var regex1 = RegExFactory.Range('a');
            var regex2 = RegExFactory.Range('b');
            var regex3 = RegExFactory.Range('c');
            var union1 = RegExFactory.Union(regex1, RegExFactory.Union(regex2, regex3));
            var union2 = RegExFactory.Union(RegExFactory.Union(regex1, regex2), regex3);

            Assert.AreEqual (0, union1.CompareTo (union2));
        }

		// union() = empty
		[Test]
		public void Test_Sum_EmptyList()
		{
			var empty = RegExFactory.Empty<char> ();
			var empty_sum = RegExFactory.Union (new RegEx<char>[]{ });

			Assert.AreEqual (empty, empty_sum);
		}

		// union(a) = a
		[Test]
		public void Test_Sum_SingleElement()
		{
			var regex = RegExFactory.Range ('a');
			var union = RegExFactory.Union (regex);

			Assert.AreEqual (regex, union);
		}

		#endregion

		#region Intersection

		// intersect() = universe
		[Test]
		public void Test_Intersect_Empty() 
		{
			var all = RegExFactory.All<char> ();
			var all_intersect = RegExFactory.Intersection<char> ();

			Assert.AreEqual (0, all_intersect.CompareTo (all));
		}

		// intersect(X, X) = X
		[Test]
		public void Test_Intersect_XX()
		{
			var base_regex = RegExFactory.Range ('a');
			var intersection = RegExFactory.Intersection (base_regex, base_regex);

			Assert.IsTrue (intersection is RegExRange<char>);
			Assert.AreEqual (0, intersection.CompareTo (base_regex));
		}

		// intersect(X, Y) = intersect(Y, X)
		[Test]
		public void Test_Intersect_XY()
		{
			var regex1 = RegExFactory.Range ('a');
			var regex2 = RegExFactory.Range ('b');
			var intersection1 = RegExFactory.Intersection (regex1, regex2);
			var intersection2 = RegExFactory.Intersection (regex2, regex1);

			Assert.AreEqual (0, intersection1.CompareTo (intersection2));
		}

		// intersect(X, Y, X) = intersect(Y, X)
		[Test]
		public void Test_Intersect_XYX()
		{
			var regex1 = RegExFactory.Range ('a');
			var regex2 = RegExFactory.Range ('b');
			var intersection1 = RegExFactory.Intersection (regex1, regex2, regex1);
			var intersection2 = RegExFactory.Intersection (regex2, regex1);

			Assert.AreEqual (0, intersection1.CompareTo (intersection2));
		}

        [Test]
        public void Test_Intersect_Associativity()
        {
            var regex1 = RegExFactory.Range('a');
            var regex2 = RegExFactory.Range('b');
            var regex3 = RegExFactory.Range('c');
            var int1 = RegExFactory.Intersection(regex1, RegExFactory.Intersection(regex2, regex3));
            var int2 = RegExFactory.Intersection(RegExFactory.Intersection(regex1, regex2), regex3);

            Assert.AreEqual (0, int1.CompareTo (int2));
        }

		// intersect() = all
		[Test]
		public void Test_Intersect_EmptyList()
		{
			var all = RegExFactory.All<char> ();
			var empty_intersect = RegExFactory.Intersection (new RegEx<char>[]{ });

			Assert.AreEqual (all, empty_intersect);
		}

		// intersect(a) = a
		[Test]
		public void Test_Intersect_SingleElement()
		{
			var regex = RegExFactory.Range ('a');
			var intersection = RegExFactory.Intersection (regex);

			Assert.AreEqual (regex, intersection);
		}

		#endregion

		#region Concat

		// X != Y -> concat(X, Y) != concat(Y, X)
		[Test]
		public void Test_Concat_XY_YX()
		{
			var regex1 = RegExFactory.Range ('a');
			var regex2 = RegExFactory.Range ('b');
			var concat1 = RegExFactory.Concat (regex1, regex2);
			var concat2 = RegExFactory.Concat (regex2, regex1);

			Assert.IsFalse (0 == concat1.CompareTo (concat2));
		}

		// concat(X, concat(Y, Z)) = concat(concat(X, Y), Z)
		[Test]
		public void Test_Concat_XYZ()
		{
			var regex1 = RegExFactory.Range ('a');
			var regex2 = RegExFactory.Range ('b');
			var regex3 = RegExFactory.Range ('c');
			var concat1 = RegExFactory.Concat (regex1, RegExFactory.Concat (regex2, regex3));
			var concat2 = RegExFactory.Concat (RegExFactory.Concat (regex1, regex2), regex3);
			var concat3 = RegExFactory.Concat (regex1, regex2, regex3);

			Assert.AreEqual (0, concat1.CompareTo (concat2));
			Assert.AreEqual (0, concat2.CompareTo (concat3));
			Assert.AreEqual (0, concat3.CompareTo (concat1));
		}

		// concat(empty, X) = concat(X, empty) = empty
		[Test]
		public void Test_Concat_Empty()
		{
			var regex = RegExFactory.Range ('a');
			var empty = RegExFactory.Empty<char> ();
			var concat1 = RegExFactory.Concat (empty, regex);
			var concat2 = RegExFactory.Concat (regex, empty);

			Assert.AreEqual (0, empty.CompareTo (concat1));
			Assert.AreEqual (0, empty.CompareTo (concat2));
		}

		// concat(epsi, X) = concat(X, epsi) = X
		[Test]
		public void Test_Concat_Epsi()
		{
			var epsi = RegExFactory.Epsilon<char> ();
			var regex = RegExFactory.Range ('a');

			var concat1 = RegExFactory.Concat (epsi, regex);
			var concat2 = RegExFactory.Concat (regex, epsi);

			Assert.IsTrue (concat1 is RegExRange<char>);
			Assert.IsTrue (concat2 is RegExRange<char>);
			Assert.AreEqual (0, regex.CompareTo (concat1));
			Assert.AreEqual (0, regex.CompareTo (concat2));
		}

		// concat() = epsi
		[Test]
		public void Test_Concat_EmptyList()
		{
			var epsi = RegExFactory.Epsilon<char> ();
			var empty_concat = RegExFactory.Concat (new RegEx<char>[]{ });

			Assert.AreEqual (epsi, empty_concat);
		}

		// concat(a) = a
		[Test]
		public void Test_Concat_SingleElement()
		{
			var regex = RegExFactory.Range ('a');
			var concat = RegExFactory.Concat (regex);

			Assert.AreEqual (regex, concat);
		}

		#endregion

		#region Star

		// star(star(X)) = star(X)
		[Test]
		public void Test_Star_Star()
		{
			var regex = RegExFactory.Range ('c');
			var star = RegExFactory.Star (regex);
			var starstar = RegExFactory.Star (star);

			Assert.AreEqual (0, star.CompareTo (starstar));
		}

		// star(epsi) = star(empty) = epsi
		[Test]
		public void Test_Star_Empty()
		{
			var epsi = RegExFactory.Epsilon<char> ();
			var empty = RegExFactory.Empty<char> ();

			var star_epsi = RegExFactory.Star (epsi);
			var star_empty = RegExFactory.Star (empty);

			Assert.IsTrue (star_empty is RegExEpsilon<char>);
			Assert.IsTrue (star_epsi is RegExEpsilon<char>);
			Assert.AreEqual (0, epsi.CompareTo (star_epsi));
			Assert.AreEqual (0, epsi.CompareTo (star_empty));
		}

		// star(all) = all
		[Test]
		public void Test_Star_All()
		{
			var all = RegExFactory.All<char> ();
			var star = RegExFactory.Star (all);

			Assert.AreEqual (all, star);
		}

		#endregion

		#region Complement

		[Test]
		// complement(complement(X)) = X
		public void Test_Complement_Double()
		{
			var regex = RegExFactory.Range ('a');
			var compl = RegExFactory.Complement (regex);
			var compl2 = RegExFactory.Complement (compl);

			Assert.IsTrue (compl2 is RegExRange<char>);
			Assert.AreEqual (0, regex.CompareTo (compl2));
		}

		[Test]
		// complement(all) = empty
		public void Test_Complement_All() 
		{
			var all = RegExFactory.All<char> ();
			var compl_all = RegExFactory.Complement (all);
			var empty = RegExFactory.Empty<char> ();

			Assert.AreEqual (empty, compl_all);
		}

		[Test]
		// complement(empty) = all
		public void Test_Complement_Empty() 
		{
			var all = RegExFactory.All<char> ();
			var empty = RegExFactory.Empty<char> ();
			var compl_empty = RegExFactory.Complement (empty);

			Assert.AreEqual (all, compl_empty);
		}

		#endregion
	}
}

