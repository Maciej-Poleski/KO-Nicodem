using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nicodem.Lexer
{
    public class RegExConcat<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
    {
        internal RegExConcat(params RegEx<T>[] regexes)
        {
            TypeId = 4;
            Regexes = regexes;
        }

        public RegEx<T>[] Regexes { private set; get; }

        public override int CompareTo(RegEx<T> other)
        {
            var diff = TypeId - other.TypeId;
            if (diff != 0)
                return diff;

            var concat = other as RegExConcat<T>;
            diff = Regexes.Length - concat.Regexes.Length;
            if (diff != 0)
                return diff;

            for (var i = 0; i < Regexes.Length; i++) {
                diff = Regexes[i].CompareTo(concat.Regexes[i]);
                if (diff != 0)
                    return diff;
            }

            return 0;
        }

		public override string ToString ()
		{
			var builder = new StringBuilder ();
			foreach (var r in Regexes)
				builder.Append ("(").Append (r).Append (")");
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
            if (Regexes.Length == 0)
            {
                return new T[] {};
            }
            if (Regexes.Length == 1)
            {
                return Regexes[0].DerivChanges();
            }
            if (Regexes[0].HasEpsilon())
            {
                return Regexes[0].DerivChanges().Union(Regexes[1].DerivChanges());
            }
            return Regexes[0].DerivChanges();
        }

        public override RegEx<T> Derivative(T c)
        {
            if (Regexes.Length == 0)
            {
                return RegExFactory.Empty<T>();
            }
            if (Regexes.Length == 1)
            {
                return Regexes[0].Derivative(c);
            }
            RegEx<T> firstTwo;
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
                RegExFactory.Concat(new ArraySegment<RegEx<T>>(Regexes, 2, Regexes.Length - 2).Array));
        }
    }
}