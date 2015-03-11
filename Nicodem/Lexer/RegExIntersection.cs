using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nicodem.Lexer
{
    public class RegExIntersection<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
    {
        internal static readonly RegExIntersection<T> RegexAll = new RegExIntersection<T>();

        internal RegExIntersection(params RegEx<T>[] regexes)
        {
            Debug.Assert(regexes.Length > 0); // set intersection is defined for nonempty sets only
            TypeId = 6;
            Regexes = regexes;
        }

        public RegEx<T>[] Regexes { private set; get; }

        public override int CompareTo(RegEx<T> other)
        {
            var diff = TypeId - other.TypeId;
            if (diff != 0)
                return diff;

            var intersection = other as RegExIntersection<T>;
            diff = Regexes.Length.CompareTo(intersection.Regexes.Length);
            if (diff != 0)
                return diff;

            for (var i = 0; i < Regexes.Length; i++)
            {
                diff = Regexes[i].CompareTo(intersection.Regexes[i]);
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

        public override IEnumerable<T> DerivChanges()
        {
            IEnumerable<T> changes = new T[] {};
            foreach (var i in Regexes)
            {
                changes = changes.Union(i.DerivChanges());
            }
            return changes;
        }

        public override RegEx<T> Derivative(T c)
        {
            var derivatives = new List<RegEx<T>>();
            foreach (var i in Regexes)
            {
                derivatives.Add(i.Derivative(c));
            }
            return RegExFactory.Intersection(derivatives.ToArray());
        }
    }
}