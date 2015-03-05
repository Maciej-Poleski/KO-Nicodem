using System;
using System.Collections.Generic;

namespace Nicodem.Core
{
	public class DoubleLinkedList <T>
	{
		class Node <W>
		{
			private W elem;
			private Node<W> _next, _prev;

			public Node<W> Next 
			{ 
				get { return _next; }
				set { _next = value; }
			}
			public Node<W> Prev 
			{ 
				get { return _prev; }
				set { _prev = value; }
			}

			public Node(W elem) 
			{
				this.elem = elem;
				_next = _prev = null;
			}
				
				
		}

		private Node<T> head;
		private Dictionary<T, Node<T> > positions = new Dictionary<T, Node<T> >();

		public DoubleLinkedList ()
		{
			head = null;
		}

		public DoubleLinkedList (ICollection<T> collection)
		{
			foreach (var elem in collection)
				Add (elem);
		}

		public bool IsEmpty () 
		{
			return head == null;
		}

		public int Count
		{
			get { return positions.Count; }
		}

		public void Add (T elem)
		{
			Node<T> newNode = new Node<T> (elem);
			positions [elem] = newNode;

			if (this.IsEmpty () == true) 
			{
				head = newNode;
				head.Prev = head;
				head.Next = head;
			} 
			else 
			{
				newNode.Next = head.Next;
				head.Next.Prev = newNode;
				newNode.Prev = head;
				head.Next = newNode;
			}
		}

		public void Remove (T elem) {
			var node = positions [elem];
			positions.Remove (elem);

			if (head.Next == head)
			{
				head = null;
			} 
			else
			{
				if (head == node)
					head = head.Prev;

				node.Prev.Next = node.Next;
				node.Next.Prev = node.Prev;
			}

		}
	}
}

