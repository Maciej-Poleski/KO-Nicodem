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
			this.TypeId = 5;
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			var diff = TypeId - other.TypeId;
			if (diff != 0)
				return diff;

			var union = other as RegExUnion;
			diff = Regexes.Length - union.Regexes.Length;
			if (diff != 0)
				return diff;

			for (var i = 0; i < Regexes.Length; i++) {
				diff = Regexes [i].CompareTo (union.Regexes [i]);
				if (diff != 0)
					return diff;
			}

			return 0;
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

