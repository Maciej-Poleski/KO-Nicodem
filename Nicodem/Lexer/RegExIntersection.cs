using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nicodem.Lexer
{
	public class RegExIntersection : RegEx
	{
		public RegEx[] Regexes { private set; get; }

		internal RegExIntersection ( params RegEx[] regexes )
		{
			Debug.Assert(regexes.Length > 0); // set intersection is defined for nonempty sets only
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			throw new NotImplementedException ();
		}

		public override bool HasEpsilon()
		{
			foreach (var i in Regexes)
			{
				if (!i.HasEpsilon())
				{
					return false;
				}
			}
			Debug.Assert(Regexes.Length > 0); // set intersection is defined for nonempty sets only
			return true;
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
			return RegExFactory.Intersection(derivatives.ToArray());
		}
	}
}

