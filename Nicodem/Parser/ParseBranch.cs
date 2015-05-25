using System;
using System.Collections.Generic;

using Nicodem.Source;

namespace Nicodem.Parser
{
	public class ParseBranch<TSymbol> : IParseTree<TSymbol>
		where TSymbol:ISymbol<TSymbol>
	{
		public IFragment Fragment { get; private set; }
		public TSymbol Symbol { get; private set; }
		public IProduction<TSymbol> Production { get; private set; }
		public IEnumerable<IParseTree<TSymbol>> Children { get; private set; }

		public ParseBranch(IFragment fragment, TSymbol symbol, IProduction<TSymbol> production, IEnumerable<IParseTree<TSymbol>> children)
		{
			Fragment = fragment;
			Symbol = symbol;
			Production = production;
			Children = children;
		}
			
		public bool Equals(IParseTree<TSymbol> other){
			var branch = other as ParseBranch<TSymbol>;
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

        public string ToStringIndented(string indent)
        {
            string r = indent + "Branch " + Symbol.ToString() + " : " + Production.ToString();
            foreach (var child in Children) r += "\n" + child.ToStringIndented(indent + "| ");
            return r;
        }

        public override string ToString() { return ToStringIndented("");  }
	}
}

