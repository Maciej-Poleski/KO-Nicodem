using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
    public static class RegExFactory
    {
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
        /// </summary>
        /// <param name="regexes">set of regexes</param>
        public static RegEx<T> Union<T>(params RegEx<T>[] regexes) where T : IComparable<T>, IEquatable<T>
        {
            Array.Sort(regexes);

            // build list of all regexes
            var list = new LinkedList<RegEx<T>>();
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
                    foreach (var r in rconv.Regexes)
                        list.AddLast(r);
                else
                    list.AddLast(regex);
            }

            // X + Y ~ Y + X
            // X + X ~ X
            var distinct = list.Distinct().ToArray();

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
        /// </summary>
        /// <param name="regexes">set of regexes</param>
        public static RegEx<T> Intersection<T>(params RegEx<T>[] regexes) where T : IComparable<T>, IEquatable<T>
        {
            Array.Sort(regexes);

            // build list of all regexes
            var list = new LinkedList<RegEx<T>>();
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
                    foreach (var r in rconv.Regexes)
                        list.AddLast(r);
                else
                    list.AddLast(regex);
            }

            // X * Y ~ Y * X
            // X * X ~ X
            var distinct = list.Distinct().ToArray();

            // {X} -> X
            return distinct.Length == 1 ? distinct[0] : new RegExIntersection<T>(distinct);
        }

        /// <summary>
        ///     RegEx representing concatenation of RegExes.
        ///     Properties:
        ///     (XY)Z ~ X(YZ)
        ///     emptyX ~ Xempty ~ empty
        ///     epsiX ~ Xepsi ~ X
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

                // check if regex is epsi TODO check case {epsi}
                if (0 == Epsilon<T>().CompareTo(regex))
                    continue;

                // other cases
                var rconv = regex as RegExConcat<T>;
                if (rconv != null)
                    foreach (var r in rconv.Regexes)
                        list.AddLast(r);
                else
                    list.AddLast(regex);
            }

            // (XY)Z ~ X(YZ)
            var arr = list.ToArray();
            return arr.Length == 1 ? arr[0] : new RegExConcat<T>(arr);
        }

        /// <summary>
        ///     RegEx representing Kleene star of RegExes.
        ///     Properties:
        ///     X** = X*
        ///     epsi* ~ empty* ~ epsi
        /// </summary>
        /// <param name="regex">RegEx to be starred</param>
        public static RegEx<T> Star<T>(RegEx<T> regex) where T : IComparable<T>, IEquatable<T>
        {
            // epsi* ~ empty* ~ epsi
            if (0 == Epsilon<T>().CompareTo(regex) || 0 == Empty<T>().CompareTo(regex))
                return Epsilon<T>();

            // X** ~ X*
            return regex is RegExStar<T> ? regex : new RegExStar<T>(regex);
        }

        /// <summary>
        ///     Properties:
        ///     ~~X ~ X
        /// </summary>
        /// <param name="regex"></param>
        public static RegEx<T> Complement<T>(RegEx<T> regex) where T : IComparable<T>, IEquatable<T>
        {
            // ~~X ~ X
            var rcomp = regex as RegExComplement<T>;
            return rcomp != null ? rcomp.Regex : new RegExComplement<T>(regex);
        }
    }
}