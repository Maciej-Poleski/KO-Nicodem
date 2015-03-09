using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
	public class RegExConcat : RegEx
	{
		public RegEx[] Regexes { private set; get; }

		internal RegExConcat ( params RegEx[] regexes )
		{
			this.TypeId = 4;
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			var diff = TypeId - other.TypeId;
			if (diff != 0)
				return diff;

			var concat = other as RegExConcat;
			diff = Regexes.Length - concat.Regexes.Length;
			if (diff != 0)
				return diff;

			for (var i = 0; i < Regexes.Length; i++) {
				diff = Regexes [i].CompareTo (concat.Regexes [i]);
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
			return true;
		}

		public override IEnumerable<Char> DerivChanges()
		{
			if (Regexes.Length == 0)
			{
				return new char[] { };
			}
			else if (Regexes.Length == 1)
			{
				return Regexes[0].DerivChanges();
			}
			if (Regexes[0].HasEpsilon()) 
			{ 
				return Regexes[0].DerivChanges().Union(Regexes[1].DerivChanges());
			} 
			else 
			{
				return Regexes[0].DerivChanges();
			}
		}

		public override RegEx Derivative(Char c)
		{
			if (Regexes.Length == 0)
			{
				return RegExFactory.Empty();
			}
			else if (Regexes.Length == 1)
			{
				return Regexes[0].Derivative(c);
			}
			RegEx firstTwo;
			if (Regexes[0].HasEpsilon()) 
			{ 
				firstTwo = RegExFactory.Union(RegExFactory.Concat(Regexes[0].Derivative(c), Regexes[1]),
						Regexes[1].Derivative(c));
			} 
			else 
			{
				firstTwo = RegExFactory.Concat(Regexes[0].Derivative(c), Regexes[1]);
			}
			return RegExFactory.Concat(firstTwo,
				RegExFactory.Concat(new ArraySegment<RegEx>(Regexes, 2, Regexes.Length - 2).Array));
		}
	}
}

