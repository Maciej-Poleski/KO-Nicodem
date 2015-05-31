using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Core
{
	public class SetPartition <T>
	{
		public LinkedList<T> Difference { get; private set; } 
		public LinkedList<T> Intersection { get; private set; } 

		public SetPartition(LinkedList<T> difference, LinkedList<T> intersection = null)
		{
			Difference = difference;
			Intersection = intersection;
		}

		internal bool HasIntersection ()
		{
			return Intersection != null;
		}

		internal void InitIntersection ()
		{
			Intersection = new LinkedList<T> ();
		}

		internal void CloseIntersection ()
		{
			Intersection = null;
		}

		internal LinkedListNode<T> Intersect(LinkedListNode<T> el) 
		{
			Difference.Remove (el);
			return Intersection.AddLast (el.Value);
		}
	}

	public class PartitionRefinement <T>
	{

		private class Pointer
		{
			public LinkedListNode<SetPartition<T>> ToSet { get; set; }
			public LinkedListNode<T> ToIterator { get; set; }

			public Pointer(LinkedListNode<SetPartition<T>> toSet, LinkedListNode<T> toIterator)
			{
				ToSet = toSet;
				ToIterator = toIterator;
			}
		}

		private LinkedList<SetPartition<T>> partition = new LinkedList<SetPartition<T>>();
		private Dictionary<T, Pointer> pointers = new Dictionary<T, Pointer>();

		// list of sets depicting initial partition
		public PartitionRefinement (IList<LinkedList<T>> sets)
		{
			foreach (var elements in sets) {
				var newSet = new SetPartition<T> (elements);
				var node = partition.AddLast (newSet);

				var iterator = elements.First;
				while (iterator != null) {
					pointers [iterator.Value] = new Pointer (node, iterator);
					iterator = iterator.Next;
				}
			}
		}

		// Returns a list of partitions represented by SetPartition<T>
		// An old reference to the set is hold by SetPartition<T>::Difference
		// An intersection is given by SetPartition<T>::Intersection which can 
		// be invalidaded with further actions on the instance of ProductRefinement
		public List<SetPartition<T>> Refine (ICollection<T> elements)
		{
			var changedNodes = new List<LinkedListNode<SetPartition<T>>> ();

			foreach (var el in elements)
				pointers [el].ToSet.Value.CloseIntersection ();

			foreach (var el in elements) 
			{
				var p = pointers [el];
				if (!p.ToSet.Value.HasIntersection ()) {
					p.ToSet.Value.InitIntersection ();
					changedNodes.Add (p.ToSet);
				}
				var newIt = p.ToSet.Value.Intersect (p.ToIterator);
				p.ToIterator = newIt;
			}

			foreach (var partNode in changedNodes) {
				if (!partNode.Value.Difference.Any ()) {
                    foreach(T el in partNode.Value.Intersection) {
                        var backIt = partNode.Value.Difference.AddLast(el);
                        pointers[el].ToIterator = backIt;
                    }
                    partNode.Value.Intersection.Clear();
					continue;
				}

				var intersectionPart = new SetPartition<T> (partNode.Value.Intersection);
				var node = partition.AddLast (intersectionPart);

				foreach (T el in partNode.Value.Intersection)
					pointers [el].ToSet = node;
			}

			var changes = new List<SetPartition<T>> ();
			foreach (var node in changedNodes) {
				if(node.Value.Intersection.Any())
					changes.Add (node.Value);
			}

			return changes;
		}

		public List<LinkedList<T>> Partition
		{
			get {
				var results = new List<LinkedList<T>> ();

				foreach(SetPartition<T> part in partition) {
					results.Add (part.Difference);
					if(!part.Difference.Any()) {
						throw new InvalidOperationException("Should not happen!");
					}	
				}

				return results;
			}
		}

		// returns the set el belongs to
		public LinkedList<T> this[T el]
		{
			get {
				return pointers [el].ToSet.Value.Difference;
			}
		}
	}
}
