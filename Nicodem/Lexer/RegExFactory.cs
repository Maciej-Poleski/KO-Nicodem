using System;
using System.Linq;
using System.Collections.Generic;

namespace Nicodem.Lexer
{
    public static class RegExFactory
    {
		/// <summary>
		/// 	Checks if collection contains both X and ~X.
		/// </summary>
		private static bool checkContainsDisjointSymbols<T>( IEnumerable<RegEx<T>> elements ) where T : IEquatable<T>, IComparable<T>
		{
			var normal = new HashSet<RegEx<T>> ();
			var complement = new HashSet<RegEx<T>> ();

			foreach (var elem in elements) {
				var compl = elem as RegExComplement<T>;
				if (compl != null)
					complement.Add (compl.Regex);
				else
					normal.Add (elem);
			}

			if (normal.Count < complement.Count) {
				foreach (var elem in normal)
					if (complement.Contains (elem))
						return true;
			} else {
				foreach (var elem in complement)
					if (normal.Contains (elem))
						return true;
			}

			return false;
		}

        /// <summary>
        ///     RegEx representing empty language.
        /// </summary>
        public static RegEx<T> Empty<T>() where T : IComparable<T>, IEquatable<T>
        {
            // empty = sum {}
            return RegExUnion<T>.RegexEmpty;
        }

        /// <summary>
        ///     RegEx representing whole universe (sigma star).
        /// </summary>
        public static RegEx<T> All<T>() where T : IComparable<T>, IEquatable<T>
        {
            // universe = intersect {}
            return RegExIntersection<T>.RegexAll;
        }

        /// <summary>
        ///     RegEx representing language that contains only empty word.
        ///     L = {epsi}
        /// </summary>
        public static RegEx<T> Epsilon<T>() where T : IComparable<T>, IEquatable<T>
        {
            return RegExEpsilon<T>.RegexEpsilon;
        }

        /// <summary>
        ///     RegEx representing range of elements [c, end)
        /// </summary>
        /// <param name="c">begin of the range</param>
        public static RegEx<T> Range<T>(T c) where T : IComparable<T>, IEquatable<T>
        {
            return new RegExRange<T>(c);
        }

        /// <summary>
        ///     RegEx representing the range of elements [c, d)
        /// </summary>
        /// <param name="c">beginning of the range</param>
        /// <param name="d">end of the range</param>
        public static RegEx<T> Range<T>(T c, T d) where T : IComparable<T>, IEquatable<T>
        {
            return Intersection(Range(c), Complement(Range(d)));
        }

        /// <summary>
        ///     RegEx representing union of RegExes.
        ///     Properties:
        ///     {} ~ empty language
        ///     {X} ~ X
        ///     {X,X} ~ {X}
        ///     {X,Y} ~ {Y,X}
        ///     {empty,X} ~ X
        ///     {all, X} ~ all
		/// 	{X, ~X} ~ all
        /// </summary>
        /// <param name="regexes">set of regexes</param>
        public static RegEx<T> Union<T>(params RegEx<T>[] regexes) where T : IComparable<T>, IEquatable<T>
        {
			// sum(X, ~X) = all
			if (checkContainsDisjointSymbols (regexes))
				return All<T> ();

            // build set of all regexes
			// X + Y ~ Y + X
			// X + X ~ X
			var S = new SortedSet<RegEx<T>>();
            foreach (var regex in regexes)
            {
                // check if regex is empty
                if (0 == Empty<T>().CompareTo(regex))
                    continue;

                // check if regex is whole universe
                if (0 == All<T>().CompareTo(regex))
                    return All<T>();

                // other cases
                var rconv = regex as RegExUnion<T>;
				if (rconv != null)
					foreach (var reg in rconv.Regexes)
						S.Add (reg);
				else
					S.Add (regex);
            }

			// final list of distinct elements
			var distinct = S.ToArray ();

            // {X} -> X
            return distinct.Length == 1 ? distinct[0] : new RegExUnion<T>(distinct);
        }

        /// <summary>
        ///     RegEx representing intersection of RegExes.
        ///     Properties:
        ///     {} ~ universe
        ///     {X} ~ X
        ///     {X,X} ~ {X}
        ///     {X,Y} ~ {Y,X}
        ///     {empty, X} ~ empty
        ///     {all, X} ~ X
		/// 	{X, ~X} ~ empty
        /// </summary>
        /// <param name="regexes">set of regexes</param>
        public static RegEx<T> Intersection<T>(params RegEx<T>[] regexes) where T : IComparable<T>, IEquatable<T>
        {
			// intersect(X, ~X) = empty
			if (checkContainsDisjointSymbols (regexes))
				return Empty<T> ();

            // build set of all regexes
			// X * Y ~ Y * X
			// X * X ~ X
			var S = new SortedSet<RegEx<T>> ();
            foreach (var regex in regexes)
            {
                // check if regex is empty
                if (0 == Empty<T>().CompareTo(regex))
                    return Empty<T>();

                // check if regex is whole universe
                if (0 == All<T>().CompareTo(regex))
                    continue;

                // other cases
                var rconv = regex as RegExIntersection<T>;
				if (rconv != null)
					foreach (var reg in rconv.Regexes)
						S.Add (reg);
				else
					S.Add (regex);
            }

			// final list of distinct regexes
			var distinct = S.ToArray ();

            // {X} -> X
            return distinct.Length == 1 ? distinct[0] : new RegExIntersection<T>(distinct);
        }

        /// <summary>
        ///     RegEx representing concatenation of RegExes.
        ///     Properties:
        ///     (XY)Z ~ X(YZ)
        ///     emptyX ~ Xempty ~ empty
        ///     epsiX ~ Xepsi ~ X
		/// 	X*X* ~ X*
        /// </summary>
        /// <param name="regexes">set of regexes</param>
        public static RegEx<T> Concat<T>(params RegEx<T>[] regexes) where T : IComparable<T>, IEquatable<T>
        {
            // build list of all regexes
            var list = new LinkedList<RegEx<T>>();
            foreach (var regex in regexes)
            {
                // check if regex is empty
                if (0 == Empty<T>().CompareTo(regex))
                    return Empty<T>();

                // check if regex is epsi
                if (0 == Epsilon<T>().CompareTo(regex))
                    continue;

                // other cases
                var rconv = regex as RegExConcat<T>;
				if (rconv != null)
					foreach (var reg in rconv.Regexes)
						list.AddLast (reg);
				else
					list.AddLast (regex);
            }

			// reduce consecutive stars
			var lst = new LinkedList<RegEx<T>> ();
			RegExStar<T> lastStar = null;

			foreach (var regex in list) {
				var star = regex as RegExStar<T>;

				// next element in sequence is a star
				if (star != null) {
					if (lastStar != null) {
						if (!lastStar.Equals (star)) {
							lst.AddLast (lastStar);
							lastStar = star;
						}
					} else
						lastStar = star;
				}

				// next element in sequence is not a star
				else {
					if (lastStar != null)
						lst.AddLast (lastStar);
					lastStar = null;
					lst.AddLast (regex);
				}
			}

			if (lastStar != null)
				lst.AddLast (lastStar);

            // (XY)Z ~ X(YZ)
            var arr = lst.ToArray();

			// concat() = epsi
			if (arr.Length == 0)
				return Epsilon<T> ();

			// concat(a) = a
			if (arr.Length == 1)
				return arr [0];
            
			return new RegExConcat<T>(arr);
        }

        /// <summary>
        ///     RegEx representing Kleene star of RegExes.
        ///     Properties:
        ///     X** = X*
        ///     epsi* ~ empty* ~ epsi
		/// 	all* ~ all
        /// </summary>
        /// <param name="regex">RegEx to be starred</param>
        public static RegEx<T> Star<T>(RegEx<T> regex) where T : IComparable<T>, IEquatable<T>
        {
			// all* ~ all
			if (0 == All<T> ().CompareTo (regex))
				return All<T> ();

            // epsi* ~ empty* ~ epsi
            if (0 == Epsilon<T>().CompareTo(regex) || 0 == Empty<T>().CompareTo(regex))
                return Epsilon<T>();

            // X** ~ X*
            return regex is RegExStar<T> ? regex : new RegExStar<T>(regex);
        }

        /// <summary>
        ///     Properties:
        ///     ~~X ~ X
		/// 	~all ~ empty
		/// 	~empty ~ all
        /// </summary>
        /// <param name="regex"></param>
        public static RegEx<T> Complement<T>(RegEx<T> regex) where T : IComparable<T>, IEquatable<T>
        {
			// ~all ~ empty
			if (All<T> ().Equals (regex))
				return Empty<T> ();

			// ~empty ~ all
			if (Empty<T> ().Equals (regex))
				return All<T> ();

            // ~~X ~ X
            var rcomp = regex as RegExComplement<T>;
            return rcomp != null ? rcomp.Regex : new RegExComplement<T>(regex);
        }
    }
}