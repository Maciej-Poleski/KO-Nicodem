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
			this.Regexes = regexes;
		}

		public override int CompareTo (RegEx other)
		{
			//TODO(pmikos)
			throw new System.NotImplementedException ();
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

