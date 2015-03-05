using System.Collections.Generic;
using Nicodem.Core;
using NUnit.Framework;

namespace Core.Tests
{
	[TestFixture]
	public class PartitionRefinementTests
	{
		bool ListEquals<T>(List<T> l1, List<T> l2)
		{
			if (l1.Count == l2.Count) {
				foreach (T t in l1)
					if (!l2.Contains (t))
						return false;
			}
			else return false;

			return true;
		}

		[Test]
		public void Basic_Partition_Test ()
		{
			var input = new List<int> { 1, 2, 3, 4, 5, 6 };
			var pr = new PartitionRefinement<int, List<int>> (input);
			Assert.AreEqual (1, pr.Partition.Count);

			var divided = pr.Refine (new List<int> { 1, 2, 3 });
			Assert.AreEqual (1, divided.Count);
			Assert.AreEqual (2, pr.Partition.Count);
			Assert.AreSame (input, divided [0].Difference);
			Assert.True(ListEquals(new List<int>{1,2,3}, divided[0].Intersection));
			Assert.True(ListEquals(new List<int>{4,5,6}, divided[0].Difference));

			var divided_2 = pr.Refine (new List<int> { 3, 4 });
			Assert.AreEqual (2, divided_2.Count);
			Assert.AreEqual (4, pr.Partition.Count);

			var divided_3 = pr.Refine (new List<int> { 1, 6 });
			Assert.AreEqual (2, divided_3.Count);
			Assert.AreEqual (6, pr.Partition.Count);
		}
	}
}

