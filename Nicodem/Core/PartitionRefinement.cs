using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Core
{

	public class SetPartition <T, E> where E : class, ICollection<T>, new()
	{
		private E difference, intersection;

		public SetPartition(E difference, E intersection = null)
		{
			this.difference = difference;
			this.intersection = intersection;
		}

		public E Difference
		{
			get { return difference; }
		}

		public E Intersection
		{
			get { return intersection; }
		}

		public bool HasIntersection ()
		{
			return intersection != null;
		}

		public void InitIntersection ()
		{
			intersection = new E ();
		}

		public void CloseIntersection ()
		{
			intersection = null;
		}

		public void Intersect(T el) 
		{
			difference.Remove (el);
			intersection.Add (el);
		}
	}

	public class PartitionRefinement <T, E> where E : class, ICollection<T>, new()
	{

		private LinkedList<SetPartition<T, E>> partition = new LinkedList<SetPartition<T, E>>();
		private Dictionary<T, LinkedListNode<SetPartition<T, E>>> pointers = new Dictionary<T, LinkedListNode<SetPartition<T, E>>>();

		public PartitionRefinement (E setElements)
		{
			SetPartition<T, E> initialSet = new SetPartition<T, E> (setElements);
			var node = partition.AddLast (initialSet);

			foreach (var el in setElements)
				pointers [el] = node;
		}

		// Returns a list of partitions represented by SetPartition<T, E>
		// An old reference to the set is hold by SetPartition<T, E>::Difference
		// An intersection is given by SetPartition<T, E>::Intersection which can 
		// be invalidaded with further actions on the instance of ProductRefinement
		public List<SetPartition<T, E>> Refine (ICollection<T> elements)
		{
			List<LinkedListNode<SetPartition<T, E>>> changedNodes = new List<LinkedListNode<SetPartition<T, E>>> ();

			foreach (var el in elements)
				pointers [el].Value.CloseIntersection ();

			foreach (var el in elements) 
			{
				var partNode = pointers [el];
				if (!partNode.Value.HasIntersection ()) {
					partNode.Value.InitIntersection ();
					changedNodes.Add (partNode);
				}
				partNode.Value.Intersect (el);
			}

			foreach (var partNode in changedNodes) {
				SetPartition<T, E> intersectionPart = new SetPartition<T, E> (partNode.Value.Intersection);
				var node = partition.AddLast (intersectionPart);
				if (!partNode.Value.Difference.Any())
					partition.Remove (partNode);

				foreach (T el in partNode.Value.Intersection)
					pointers [el] = node;
			}

			List<SetPartition<T, E>> changes = new List<SetPartition<T, E>> ();
			foreach (var node in changedNodes)
				changes.Add (node.Value);

			return changes;
		}

		public ICollection<E> Partition
		{
			get {
				List<E> results = new List<E> ();

				foreach (SetPartition<T, E> part in partition)
					results.Add (part.Difference);

				return results;
			}
		}
	}
}
