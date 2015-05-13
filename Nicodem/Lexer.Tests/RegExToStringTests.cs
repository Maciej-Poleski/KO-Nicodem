using NUnit.Framework;
using Nicodem.Lexer;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExToStringTests
	{
		#region simple 

		[Test]
		public void Test_UnionSimple() {
			var ra = RegExFactory.Range<char> ('a');
			var rb = RegExFactory.Range<char> ('b');
			var rc = RegExFactory.Range<char> ('c');

			var r1 = RegExFactory.Union (ra);
			Assert.AreEqual (ra.ToString(), r1.ToString ());

			var r2 = RegExFactory.Union (ra, rb);
			Assert.AreEqual (ra + " | " + rb, r2.ToString ());

			var r3 = RegExFactory.Union (ra, rb, rc);
			Assert.AreEqual (ra + " | " + rb + " | " + rc, r3.ToString ());
		}

		[Test]
		public void Test_IntersectionSimple() {
			var ra = RegExFactory.Range<char> ('a');
			var rb = RegExFactory.Range<char> ('b');
			var rc = RegExFactory.Range<char> ('c');

			var r1 = RegExFactory.Intersection (ra);
			Assert.AreEqual (ra.ToString(), r1.ToString ());

			var r2 = RegExFactory.Intersection (ra, rb);
			Assert.AreEqual (ra + " & " + rb, r2.ToString ());

			var r3 = RegExFactory.Intersection (ra, rb, rc);
			Assert.AreEqual (ra + " & " + rb + " & " + rc, r3.ToString ());
		}

		[Test]
		public void Test_ConcatSimple() {
			var ra = RegExFactory.Range<char> ('a');
			var rb = RegExFactory.Range<char> ('b');
			var rc = RegExFactory.Range<char> ('c');

			var r1 = RegExFactory.Concat (ra);
			Assert.AreEqual (ra.ToString(), r1.ToString ());

			var r2 = RegExFactory.Concat (ra, rb);
			Assert.AreEqual (ra.ToString() + rb, r2.ToString ());

			var r3 = RegExFactory.Concat (ra, rb, rc);
			Assert.AreEqual (ra.ToString() + rb + rc, r3.ToString ());
		}

		[Test]
		public void Test_StarSimple() {
			var ra = RegExFactory.Range<char> ('a');
			var r1 = RegExFactory.Star (ra);
			Assert.AreEqual (ra + "*", r1.ToString ());
		}

		[Test]
		public void Test_ComplementSimple() {
			var ra = RegExFactory.Range<char> ('a');
			var r1 = RegExFactory.Complement (ra);
			Assert.AreEqual ("~" + ra, r1.ToString ());
		}

		#endregion

		#region complex

		[Test]
		public void Test_UnionComplex() {
			var ra = RegExFactory.Intersection(RegExFactory.Range<char> ('a'), RegExFactory.Range<char> ('z'));
			var rb = RegExFactory.Range<char> ('b');
			var rc = RegExFactory.Range<char> ('c');

			var r1 = RegExFactory.Union (ra);
			Assert.AreEqual (ra.ToString(), r1.ToString ());

			var r2 = RegExFactory.Union (ra, rb);
			Assert.AreEqual (rb + " | (" + ra + ")", r2.ToString ());

			var r3 = RegExFactory.Union (ra, rb, rc);
			Assert.AreEqual (rb + " | " + rc + " | (" + ra + ")", r3.ToString ());
		}

		[Test]
		public void Test_IntersectionComplex() {
			var ra = RegExFactory.Union(RegExFactory.Range<char> ('a'), RegExFactory.Range<char> ('z'));
			var rb = RegExFactory.Range<char> ('b');
			var rc = RegExFactory.Range<char> ('c');

			var r1 = RegExFactory.Intersection (ra);
			Assert.AreEqual (ra.ToString(), r1.ToString ());

			var r2 = RegExFactory.Intersection (ra, rb);
			Assert.AreEqual (rb + " & (" + ra + ")", r2.ToString ());

			var r3 = RegExFactory.Intersection (ra, rb, rc);
			Assert.AreEqual (rb + " & " + rc + " & (" + ra + ")", r3.ToString ());
		}

		[Test]
		public void Test_ConcatComplex() {
			var ra = RegExFactory.Union(RegExFactory.Range<char> ('a'), RegExFactory.Range<char> ('z'));
			var rb = RegExFactory.Range<char> ('b');
			var rc = RegExFactory.Range<char> ('c');

			var r1 = RegExFactory.Concat (ra);
			Assert.AreEqual (ra.ToString(), r1.ToString ());

			var r2 = RegExFactory.Concat (ra, rb);
			Assert.AreEqual ("(" + ra + ")" + rb, r2.ToString ());

			var r3 = RegExFactory.Concat (ra, rb, rc);
			Assert.AreEqual ("(" + ra + ")" + rb + rc, r3.ToString ());
		}

		[Test]
		public void Test_StarComplex() {
			var ra = RegExFactory.Union(RegExFactory.Range<char> ('a'), RegExFactory.Range<char> ('z'));
			var r1 = RegExFactory.Star (ra);
			Assert.AreEqual ("(" + ra + ")*", r1.ToString ());
		}

		[Test]
		public void Test_ComplementComplex() {
			var ra = RegExFactory.Union(RegExFactory.Range<char> ('a'), RegExFactory.Range<char> ('z'));
			var r1 = RegExFactory.Complement (ra);
			Assert.AreEqual ("~(" + ra + ")", r1.ToString ());
		}

		#endregion
	}
}

