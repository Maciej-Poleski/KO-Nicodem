using Nicodem.Lexer;
using NUnit.Framework;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExCompareToTests
	{
		[Test]
		public void Test_Epsi_Smallest()
		{
			var epsi = RegExFactory.Epsilon ();
			var range = RegExFactory.Range ('a');
			var range2 = RegExFactory.Range ('b');

			Assert.AreEqual (0, epsi.CompareTo (epsi)); // epsi == epsi
			Assert.AreEqual (0, epsi.CompareTo (RegExFactory.Epsilon())); // epsi == epsi

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Range ('a')) < 0); // epsi < RegExRange(...)
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Range ('a', 'b')) < 0); // epsi < RegexRange(...)

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Empty ()) < 0); // epsi < empty
			Assert.IsTrue (epsi.CompareTo (RegExFactory.All ()) < 0); // epsi < all

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Star (range)) < 0); // epsi < star
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Complement (range)) < 0); // epsi < complement

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Concat (range, range)) < 0); // epsi < concat
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Union (range, range2)) < 0); // epsi < union
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Intersection (range, range2)) < 0); // epsi < intersection
		}
	}
}

