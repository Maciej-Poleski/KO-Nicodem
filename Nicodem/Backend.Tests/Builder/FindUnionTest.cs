using NUnit.Framework;
using Nicodem.Backend.Builder;
using System.Collections.Generic;
using System;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class FindUnionTest
	{
		[Test]
		public void OneGroupTest ()
		{
			var univ = new string[4]{ "a", "b", "c", "f" };
			var fu = new FindUnion<string> (univ);
			fu.Union ("a", "c");
			fu.Union ("b", "f");
			fu.Union ("a", "b");

			var keys = new HashSet<string> ();
			foreach (string s in univ)
				keys.Add (fu.Find (s));
			Assert.AreEqual (keys.Count, 1);
		}

		[Test]
		public void ManyGroupsTest ()
		{
			var univ = new int[8]{ 1, 2, 3, 4, 5, 6, 7, 8 };
			var fu = new FindUnion<int> (univ);
			fu.Union (1, 2);
			fu.Union (1, 3);
			fu.Union (7, 8);
			fu.Union (1, 8);

			var keys = new HashSet<int> ();
			foreach (int s in univ)
				keys.Add (fu.Find (s));
			Assert.AreEqual (keys.Count, 4);
		}
	}
}

