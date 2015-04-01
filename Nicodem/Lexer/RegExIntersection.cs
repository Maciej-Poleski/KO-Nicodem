using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nicodem.Lexer
{
    public class RegExIntersection<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
    {
        internal static readonly RegExIntersection<T> RegexAll = new RegExIntersection<T>();

        internal RegExIntersection(params RegEx<T>[] regexes)
        {
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

            for (var i = 0; i < Regexes.Length; i++) {
                diff = Regexes[i].CompareTo(intersection.Regexes[i]);
                if (diff != 0)
                    return diff;
            }

            return 0;
        }

		public override string ToString ()
		{
			var builder = new StringBuilder ();
			foreach (var r in Regexes)
				builder.Append ("(").Append (r).Append (") & ");
			return builder.ToString ();
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

        public override IEnumerable<T> DerivChanges()
        {
            IEnumerable<T> changes = new T[] {};
            foreach (var i in Regexes)
            {
                changes = changes.Union(i.DerivChanges());
            }
            return changes;
        }

		// inter( X1, ... )^a = inter( X1^a, ... )
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