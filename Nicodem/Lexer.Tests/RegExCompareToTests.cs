using Nicodem.Lexer;
using NUnit.Framework;

namespace Lexer.Tests
{
	[TestFixture]
	public class RegExCompareToTests
	{
		[Test]
		public void Test_Epsi()
		{
			var epsi = RegExFactory.Epsilon<char> ();
			var range = RegExFactory.Range ('a');
			var range2 = RegExFactory.Range ('b');

			Assert.AreEqual (0, epsi.CompareTo (epsi)); // epsi == epsi
			Assert.AreEqual (0, epsi.CompareTo (RegExFactory.Epsilon<char>())); // epsi == epsi

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Range ('a')) < 0); // epsi < RegExRange(...)
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Range ('a', 'b')) < 0); // epsi < RegexRange(...)

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Empty<char> ()) < 0); // epsi < empty
			Assert.IsTrue (epsi.CompareTo (RegExFactory.All<char> ()) < 0); // epsi < all

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Star (range)) < 0); // epsi < star
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Complement (range)) < 0); // epsi < complement

			Assert.IsTrue (epsi.CompareTo (RegExFactory.Concat (range, range)) < 0); // epsi < concat
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Union (range, range2)) < 0); // epsi < union
			Assert.IsTrue (epsi.CompareTo (RegExFactory.Intersection (range, range2)) < 0); // epsi < intersection
		}

        [Test]
        public void Test_Order()
        {
            var epsi = RegExFactory.Epsilon<char>(); // typeId = 0
            var range = RegExFactory.Range('a'); // typeId = 1
			var range2 = RegExFactory.Range ('b'); // typeId = 1
            var star = RegExFactory.Star(range); // typeId = 2
            var compl = RegExFactory.Complement(range); // typeId = 3
            var concat = RegExFactory.Concat(range, star); // typeId = 4
            var union = RegExFactory.Union(range, star, range2); // typeId = 5
            var inter = RegExFactory.Intersection(range, star, range2); // typeId = 6

            // a < b
            Assert.IsTrue(epsi.CompareTo(range) < 0);
            Assert.IsTrue(range.CompareTo(star) < 0);
            Assert.IsTrue(star.CompareTo(compl) < 0);
            Assert.IsTrue(compl.CompareTo(concat) < 0);
            Assert.IsTrue(concat.CompareTo(union) < 0);
            Assert.IsTrue(union.CompareTo(inter) < 0);

            // a > b
            Assert.IsTrue(range.CompareTo(epsi) > 0);
            Assert.IsTrue(star.CompareTo(range) > 0);
            Assert.IsTrue(compl.CompareTo(star) > 0);
            Assert.IsTrue(concat.CompareTo(compl) > 0);
            Assert.IsTrue(union.CompareTo(concat) > 0);
            Assert.IsTrue(inter.CompareTo(union) > 0);

            // a = b
            Assert.IsTrue(epsi.CompareTo(epsi) == 0);
            Assert.IsTrue(range.CompareTo(range) == 0);
            Assert.IsTrue(star.CompareTo(star) == 0);
            Assert.IsTrue(compl.CompareTo(compl) == 0);
            Assert.IsTrue(concat.CompareTo(concat) == 0);
            Assert.IsTrue(union.CompareTo(union) == 0);
            Assert.IsTrue(inter.CompareTo(inter) == 0);
        }

        [Test]
        public void Test_Empty()
        {
            var empty = RegExFactory.Empty<char>();
            Assert.IsTrue(empty is RegExUnion<char>);

            var emp = empty as RegExUnion<char>;
            Assert.AreEqual(0, emp.Regexes.Length);
        }

        [Test]
        public void Test_All()
        {
            var all = RegExFactory.All<char>();
            Assert.IsTrue(all is RegExIntersection<char>);

            var a = all as RegExIntersection<char>;
            Assert.AreEqual(0, a.Regexes.Length);
        }

        [Test]
        public void Test_Star()
        {
            var reg1 = RegExFactory.Epsilon<char>();
            var reg2 = RegExFactory.Range('a');
            var reg3 = RegExFactory.Range('a');

            var star1 = RegExFactory.Star(reg1);
            var star2 = RegExFactory.Star(reg2);
            var star3 = RegExFactory.Star(reg2);
            var star4 = RegExFactory.Star(reg3);

            Assert.IsTrue(star2.CompareTo(star3) == 0);
            Assert.IsTrue(star3.CompareTo(star2) == 0);

            Assert.IsTrue(star3.CompareTo(star4) == 0);
            Assert.IsTrue(star4.CompareTo(star3) == 0);

            Assert.IsTrue(star1.CompareTo(star2) < 0);
            Assert.IsTrue(star2.CompareTo(star1) > 0);
        }

        [Test]
        public void Test_Complement()
        {
            var reg1 = RegExFactory.Epsilon<char>();
            var reg2 = RegExFactory.Range('a');
            var reg3 = RegExFactory.Range('a');

            var comp1 = RegExFactory.Complement(reg1);
            var comp2 = RegExFactory.Complement(reg2);
            var comp3 = RegExFactory.Complement(reg2);
            var comp4 = RegExFactory.Complement(reg3);

            Assert.IsTrue(comp2.CompareTo(comp3) == 0);
            Assert.IsTrue(comp3.CompareTo(comp2) == 0);

            Assert.IsTrue(comp3.CompareTo(comp4) == 0);
            Assert.IsTrue(comp4.CompareTo(comp3) == 0);

            Assert.IsTrue(comp1.CompareTo(comp2) < 0);
            Assert.IsTrue(comp2.CompareTo(comp1) > 0);
        }

        [Test]
        public void Test_Concat()
        {
            var reg1 = RegExFactory.Epsilon<char>();
            var reg2 = RegExFactory.Range('a');
            var reg3 = RegExFactory.Range('a');
            var reg4 = RegExFactory.Range('b');

            var con1 = RegExFactory.Concat(reg1);
            var con2 = RegExFactory.Concat(reg2);
            var con3 = RegExFactory.Concat(reg3);
            var con4 = RegExFactory.Concat(reg4);

            var con5 = RegExFactory.Concat(reg2, reg3);
            var con6 = RegExFactory.Concat(reg3, reg2);
            var con7 = RegExFactory.Concat(reg3, reg4);

            Assert.AreNotEqual(con1, con2);
            Assert.AreNotEqual(con1, con3);
            Assert.AreNotEqual(con1, con4);
            Assert.AreEqual(con2, con3);
            Assert.AreNotEqual(con2, con4);
            Assert.AreNotEqual(con3, con4);

            Assert.AreEqual(con5, con6);
            Assert.AreNotEqual(con5, con7);
            Assert.AreNotEqual(con6, con7);
        }

        [Test]
        public void Test_Union()
        {
            var reg1 = RegExFactory.Epsilon<char>();
            var reg2 = RegExFactory.Range('a');
            var reg3 = RegExFactory.Range('a');
            var reg4 = RegExFactory.Range('b');

            var un1 = RegExFactory.Union(reg1);
            var un2 = RegExFactory.Union(reg2);
            var un3 = RegExFactory.Union(reg3);
            var un4 = RegExFactory.Union(reg4);

            var un5 = RegExFactory.Union(reg2, reg3);
            var un6 = RegExFactory.Union(reg3, reg4);
            var un7 = RegExFactory.Union(reg4, reg3);

            Assert.AreNotEqual(un1, un2);
            Assert.AreNotEqual(un1, un3);
            Assert.AreNotEqual(un1, un4);
            Assert.AreEqual(un2, un3);
            Assert.AreNotEqual(un2, un4);
            Assert.AreNotEqual(un3, un4);

            Assert.AreNotEqual(un5, un6);
            Assert.AreNotEqual(un5, un7);
            Assert.AreEqual(un6, un7);
        }

        [Test]
        public void Test_Intersection()
        {
            var reg1 = RegExFactory.Epsilon<char>();
            var reg2 = RegExFactory.Range('a');
            var reg3 = RegExFactory.Range('a');
            var reg4 = RegExFactory.Range('b');

            var un1 = RegExFactory.Intersection(reg1);
            var un2 = RegExFactory.Intersection(reg2);
            var un3 = RegExFactory.Intersection(reg3);
            var un4 = RegExFactory.Intersection(reg4);

            var un5 = RegExFactory.Intersection(reg2, reg3);
            var un6 = RegExFactory.Intersection(reg3, reg4);
            var un7 = RegExFactory.Intersection(reg4, reg3);

            Assert.AreNotEqual(un1, un2);
            Assert.AreNotEqual(un1, un3);
            Assert.AreNotEqual(un1, un4);
            Assert.AreEqual(un2, un3);
            Assert.AreNotEqual(un2, un4);
            Assert.AreNotEqual(un3, un4);

            Assert.AreNotEqual(un5, un6);
            Assert.AreNotEqual(un5, un7);
            Assert.AreEqual(un6, un7);
        }
	}
}

