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
		/// RegEx representing language that contains only empty word.
		/// L = {epsi}
		/// </summary>
		public static RegEx Epsilon()
		{
			return new RegExEpsilon ();
		}

		/// <summary>
		/// RegEx representing range of elements [c, end)
		/// </summary>
		/// <param name="c">begin of the range</param>
		public static RegEx Range( char c )
		{
			return new RegExRange (c);
		}

		/// <summary>
		/// RegEx representing the range of elements [c, d)
		/// </summary>
		/// <param name="c">beginning of the range</param>
		/// <param name="d">end of the range</param>
		public static RegEx Range(char c, char d)
		{
			return Intersection(Range(c), Complement(Range(d)));
		}

		/// <summary>
		/// RegEx representing the character c
		/// </summary>
		/// <param name="c">the character</param> 
		public static RegEx Character(char c)
		{
			return Range(c, (char) (((int) c) + 1));
		}

		/// <summary>
		/// RegEx representing union of RegExes.
		/// Properties:
		/// {} ~ empty language
		/// {X} ~ X
		/// {X,X} ~ {X}
		/// {X,Y} ~ {Y,X}
		/// {empty,X} ~ X
		/// {all, X} ~ all
		/// </summary>
		/// <param name="regexes">set of regexes</param>
		public static RegEx Union( params RegEx[] regexes )
		{
			var regex_empty = Empty ();
			var regex_all = All ();

			// build list of all regexes
			var list = new LinkedList<RegEx> ();
			foreach (var regex in regexes)
			{
				// check if regex is empty
				if (0 == regex_empty.CompareTo (regex))
					continue;

				// check if regex is whole universe
				if (0 == regex_all.CompareTo (regex))
					return regex_all;

				// other cases
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

		/// <summary>
		/// RegEx representing intersection of RegExes.
		/// Properties:
		/// {} ~ universe
		/// {X} ~ X
		/// {X,X} ~ {X}
		/// {X,Y} ~ {Y,X}
		/// {empty, X} ~ empty
		/// {all, X} ~ X 
		/// </summary>
		/// <param name="regexes">set of regexes</param>
		public static RegEx Intersection( params RegEx[] regexes )
		{
			var regex_empty = Empty ();
			var regex_all = All ();

			// build list of all regexes
			var list = new LinkedList<RegEx> ();
			foreach (var regex in regexes)
			{
				// check if regex is empty
				if (0 == regex_empty.CompareTo (regex))
					return regex_empty;

				// check if regex is whole universe
				if (0 == regex_all.CompareTo (regex))
					continue;

				// other cases
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
			var regex_empty = Empty ();
			var regex_epsi = Epsilon ();

			// build list of all regexes
			var list = new LinkedList<RegEx> ();
			foreach (var regex in regexes)
			{
				// check if regex is empty
				if (0 == regex_empty.CompareTo (regex))
					return regex_empty;

				// check if regex is epsi TODO check case {epsi}
				if (0 == regex_epsi.CompareTo (regex))
					continue;

				// other cases
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

		/// <summary>
		/// RegEx representing Kleene star of RegExes.
		/// Properties:
		/// X** = X*
		/// epsi* ~ empty* ~ epsi
		/// </summary>
		/// <param name="regex">RegEx to be starred</param>
		public static RegEx Star( RegEx regex )
		{
			var regex_epsi = Epsilon ();
			var regex_empty = Empty ();

			// epsi* ~ empty* ~ epsi
			if (0 == regex_epsi.CompareTo (regex) || 0 == regex_empty.CompareTo (regex))
				return regex_epsi;

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

