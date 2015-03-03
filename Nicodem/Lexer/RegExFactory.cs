using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
	public static class RegExFactory
	{
		/// <summary>
		/// RegEx representing empty language.
		/// </summary>
		public static RegEx Empty() 
		{
			// empty = sum {}
			return Union ();
		}

		/// <summary>
		/// RegEx representing whole universe (sigma star).
		/// </summary>
		public static RegEx All()
		{
			// universe = intersect {}
			return Intersection ();
		}

		/// <summary>
		/// RegEx representing range of elements [c, end)
		/// </summary>
		/// <param name="c">begin of the range</param>
		public static RegEx Range( char c )
		{
			return new RegExRange (c);
		}
		//TODO
		/// <summary>
		/// RegEx representing union of RegExes.
		/// Properties:
		/// {} ~ empty language
		/// {X} ~ X
		/// {X,X} ~ {X}
		/// {X,Y} ~ {Y,X}
		/// </summary>
		/// <param name="regexes">set of regexes</param>
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
		//TODO
		/// <summary>
		/// RegEx representing intersection of RegExes.
		/// Properties:
		/// {} ~ universe
		/// {X} ~ X
		/// {X,X} ~ {X}
		/// {X,Y} ~ {Y,X}
		/// </summary>
		/// <param name="regexes">set of regexes</param>
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
		//TODO
		/// <summary>
		/// RegEx representing concatenation of RegExes.
		/// Properties:
		/// (XY)Z ~ X(YZ)
		/// emptyX ~ Xempty ~ empty
		/// epsiX ~ Xepsi ~ X
		/// </summary>
		/// <param name="regexes">set of regexes</param>
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
		//TODO
		/// <summary>
		/// RegEx representing Kleene star of RegExes.
		/// Properties:
		/// X** = X*
		/// epsi* ~ empty* ~ epsi
		/// </summary>
		/// <param name="regex">RegEx to be starred</param>
		public static RegEx Star( RegEx regex )
		{
			// X** ~ X*
			return regex is RegExStar ? regex : new RegExStar (regex);

		}

		/// <summary>
		/// Properties:
		/// ~~X ~ X
		/// </summary>
		/// <param name="regex"></param>
		public static RegEx Complement( RegEx regex )
		{
			// ~~X ~ X
			var rcomp = regex as RegExComplement;
			return rcomp != null ? rcomp.Regex : new RegExComplement (regex);

		}
	}
}

