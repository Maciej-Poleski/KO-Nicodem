using System;
using System.Collections.Generic;

namespace Nicodem.Backend
{
	public class FindUnion<T>
	{
	
		private Dictionary<T,T> rep;

		public FindUnion (IEnumerable<T> initial)
		{
			rep = new Dictionary<T,T> ();
			foreach(T obj in initial) rep[obj] = obj;
		}

		public T Find(T obj){
			if(!rep.ContainsKey(obj)) throw new ArgumentException("The object is not belonging to the given universum.");
			if(rep[obj].Equals(obj)) return obj;
			var _rep = Find(rep[obj]);
			rep[obj] = _rep;
			return _rep;
		}

		public void Union(T a, T b){
			var repA = Find(a);
			var repB = Find(b);
			rep[repA] = repB;
		}
	}
}

