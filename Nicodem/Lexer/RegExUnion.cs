using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
	public class RegExUnion : RegEx
	{
		public RegEx[] Regexes { private set; get; }

		internal RegExUnion ( params RegEx[] regexes )
		{
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(?)
			throw new System.NotImplementedException ();
		}

		public override bool HasEpsilon()
		{
			foreach (var i in Regexes)
			{
				if (i.HasEpsilon())
				{
					return true;
				}
			}
			return false;
		}

		public override IEnumerable<Char> DerivChanges()
		{
			IEnumerable<char> changes = new char[] { };
			foreach (var i in Regexes)
			{
				changes = Enumerable.Union(changes, i.DerivChanges());
			}
			return changes;
		}

		public override RegEx Derivative(Char c)
		{
			var derivatives = new List<RegEx>();
			foreach (var i in Regexes) 
			{
				derivatives.Add(i.Derivative(c));
			}
			return RegExFactory.Union(derivatives.ToArray());
		}
	}
}

