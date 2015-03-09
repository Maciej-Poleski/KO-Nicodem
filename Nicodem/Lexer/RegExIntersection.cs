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
			this.TypeId = 6;
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			var diff = TypeId - other.TypeId;
			if (diff != 0)
				return diff;

			var intersection = other as RegExIntersection;
			diff = Regexes.Length - intersection.Regexes.Length;
			if (diff != 0)
				return diff;

			for (var i = 0; i < Regexes.Length; i++) {
				diff = Regexes [i].CompareTo (intersection.Regexes [i]);
				if (diff != 0)
					return diff;
			}

			return 0;
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

