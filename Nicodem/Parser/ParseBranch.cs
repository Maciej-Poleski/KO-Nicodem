using System;
using System.Collections.Generic;

using Nicodem.Source;

namespace Nicodem.Parser
{
	internal class ParseBranch<TProduction> : IParseTree<TProduction>
		where TProduction:IProduction
	{
		public IFragment Fragment { get; private set; }
		public ISymbol Symbol { get; private set; }
		public TProduction Production { get; private set; }
		public IEnumerable<IParseTree<TProduction>> Children { get; private set; }

		public ParseBranch(IFragment fragment, ISymbol symbol, TProduction production, IEnumerable<IParseTree<TProduction>> children)
		{
			Fragment = fragment;
			Symbol = symbol;
			Production = production;
			Children = children;
		}
			
		public bool Equals(IParseTree<TProduction> other){
			var branch = other as ParseBranch<TProduction>;
			if (branch == null)
				return false;

			if (!Symbol.Equals (other.Symbol))
				return false;

			var childs1 = Children.GetEnumerator ();
			var childs2 = branch.Children.GetEnumerator ();

			while (childs1.MoveNext () && childs2.MoveNext ()) {
				var tree1 = childs1.Current;
				var tree2 = childs2.Current;
				if (!tree1.Equals (tree2))
					return false;
			}

			return !(childs1.MoveNext () || childs2.MoveNext ());
		}

	}
}

