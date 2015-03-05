using System;
using System.Collections.Generic;

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
			return intersection == null;
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

		private List<SetPartition<T, E>> partition = new List<SetPartition<T, E>>();
		private Dictionary<T, SetPartition<T, E>> pointers = new Dictionary<T, SetPartition<T, E>>();

		public PartitionRefinement (E setElements)
		{
			SetPartition<T, E> initialSet = new SetPartition<T, E> (setElements);
			partition.Add (initialSet);

			foreach (var el in setElements)
				pointers [el] = initialSet;
		}

		// Returns a list of partitions represented by SetPartition<T, E>
		// An old reference to the set is hold by SetPartition<T, E>::Difference
		// An intersection is given by SetPartition<T, E>::Intersection which can 
		// be invalidaded with further actions on the instance of ProductRefinement
		public List<SetPartition<T, E>> Refine (ICollection<T> elements)
		{
			List<SetPartition<T, E>> changes = new List<SetPartition<T, E>> ();

			foreach (var el in elements)
				pointers [el].CloseIntersection ();

			foreach (var el in elements) 
			{
				SetPartition<T, E> part = pointers [el];
				if (part.HasIntersection ()) {
					part.InitIntersection ();
					changes.Add (part);
				}
				part.Intersect (el);
			}

			foreach (SetPartition<T, E> part in changes) {
				SetPartition<T, E> intersectionPart = new SetPartition<T, E> (part.Intersection);
				partition.Add (intersectionPart);
				if (part.Difference.Count == 0)
					partition.Remove (part);

				foreach (T el in part.Intersection)
					pointers [el] = intersectionPart;
			}

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
