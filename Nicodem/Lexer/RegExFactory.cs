using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
	public static class RegExFactory
	{
		public static RegEx Empty() 
		{
			// TODO(pmikos)
			throw new NotImplementedException ();
		}

		public static RegEx Range( char c )
		{
			return new RegExRange (c);
		}

		public static RegEx Union( params RegEx[] regexes )
		{
			// build list of all regexes
			var list = new LinkedList<RegEx> ();
			foreach (var regex in regexes)
			{
				var rconv = regex as RegExUnion;
				if (rconv != null)
					foreach (var r in rconv.Regexes)
						list.AddLast (r);
				else
					list.AddLast (regex);
			}

			// X + Y ~ Y + X
			// X + X ~ X
			var distinct = list.Distinct ().ToArray ();

			// {X} -> X
			return distinct.Length == 1 ? distinct [0] : new RegExUnion (distinct);
		}

		public static RegEx Intersection( params RegEx[] regexes )
		{
			// build list of all regexes
			var list = new LinkedList<RegEx> ();
			foreach (var regex in regexes)
			{
				var rconv = regex as RegExIntersection;
				if (rconv != null)
					foreach (var r in rconv.Regexes)
						list.AddLast (r);
				else
					list.AddLast (regex);
			}

			// X * Y ~ Y * X
			// X * X ~ X
			var distinct = list.Distinct ().ToArray ();

			// {X} -> X
			return distinct.Length == 1 ? distinct [0] : new RegExIntersection (distinct);
		}

		public static RegEx Concat( params RegEx[] regexes )
		{
			// build list of all regexes
			var list = new LinkedList<RegEx> ();
			foreach (var regex in regexes)
			{
				var rconv = regex as RegExConcat;
				if (rconv != null)
					foreach (var r in rconv.Regexes)
						list.AddLast (r);
				else
					list.AddLast (regex);
			}

			// (XY)Z ~ X(YZ)
			return new RegExConcat (list.ToArray ());
		}

		public static RegEx Star( RegEx regex )
		{
			// X** ~ X*
			if (regex is RegExStar)
				return regex;

			return new RegExStar (regex);
		}

		public static RegEx Complement( RegEx regex )
		{
			var rcomp = regex as RegExComplement;

			// ~~X ~ X
			return rcomp != null ? rcomp.Regex : new RegExComplement (regex);

		}
	}
}

