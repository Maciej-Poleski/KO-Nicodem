using System.Collections.Generic;
using Nicodem.Core;
using NUnit.Framework;
using System.Linq;

namespace Core.Tests
{
	[TestFixture]
	public class PartitionRefinementTests
	{

		class LinkedListWithInit<T> : LinkedList<T>
		{
			public void Add( T item )
			{
				AddLast (item);
			}
		}

		static bool ListEquals<T>(LinkedList<T> l1, LinkedList<T> l2)
		{
			return (l1.Count == l2.Count && !l1.Except (l2).Any ());
		}

		[Test]
		public void Basic_Partition_Test ()
		{
			var input = new LinkedListWithInit<int> { 1, 2, 3, 4, 5, 6 };
			var pr = new PartitionRefinement<int> (new List<LinkedList<int>>{input});
			Assert.AreEqual (1, pr.Partition.Count);

			var divided = pr.Refine (new List<int> { 1, 2, 3 });
			Assert.AreEqual (1, divided.Count);
			Assert.AreEqual (2, pr.Partition.Count);
			Assert.AreSame (input, divided [0].Difference);
			Assert.IsTrue(ListEquals(new LinkedListWithInit<int>{1,2,3}, divided[0].Intersection));
			Assert.IsTrue(ListEquals(new LinkedListWithInit<int>{4,5,6}, divided[0].Difference));

			var divided_2 = pr.Refine (new List<int> { 3, 4 });
			Assert.AreEqual (2, divided_2.Count);
			Assert.AreEqual (4, pr.Partition.Count);

			var divided_3 = pr.Refine (new List<int> { 1, 6 });
			Assert.AreEqual (2, divided_3.Count);
			Assert.AreEqual (6, pr.Partition.Count);
		}

		[Test]
		public void Redundant_Partition_Test ()
		{
			var input = new LinkedListWithInit<int> { 1, 2, 3, 4, 5, 6 };
			var pr = new PartitionRefinement<int> (new List<LinkedList<int>>{input});
			Assert.AreEqual (1, pr.Partition.Count);

			var divided = pr.Refine (new List<int> { 1, 2, 3, 4, 5, 6 });
			Assert.IsFalse (divided.Any ());

			var divided_2 = pr.Refine (new List<int> { 2, 3, 4, 5, 6 });
			Assert.IsTrue (divided_2.Any ());
		}

		[Test]
		public void Access_Operator_Test ()
		{
			var input = new LinkedListWithInit<int> { 1, 2, 3, 4, 5, 6 };
			var pr = new PartitionRefinement<int> (new List<LinkedList<int>>{input});
			Assert.AreSame (input, pr [1]);
			Assert.AreSame (input, pr [6]);

			var divided = pr.Refine (new List<int> { 1, 2, 3 });
			var first = divided [0].Difference;
			var second = divided [0].Intersection;

			foreach (var el in first)
				Assert.AreSame (first, pr [el]);
			foreach (var el in second)
				Assert.AreSame (second, pr [el]);

			var devided_2 = pr.Refine (new List<int> { 3, 4 });
			Assert.AreEqual (2, devided_2.Count);
			var firstIntersect = devided_2 [0].Intersection;
			var firstDiff = devided_2 [0].Difference;
			var secondIntersect = devided_2 [1].Intersection;
			var secondDiff = devided_2 [1].Difference;

			foreach (var el in firstIntersect)
				Assert.AreSame (firstIntersect, pr [el]);
			foreach (var el in firstDiff)
				Assert.AreSame (firstDiff, pr [el]);
			foreach (var el in secondIntersect)
				Assert.AreSame (secondIntersect, pr [el]);
			foreach (var el in secondDiff)
				Assert.AreSame (secondDiff, pr [el]);
		}
	}
}

