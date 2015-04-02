using System;
using System.Collections.Generic;
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
			var s = new System.Collections.Generic.HashSet<T> ();

			var index = 0;
			while (index < Regexes.Length && Regexes [index].HasEpsilon ())
				foreach (var c in Regexes [index++].DerivChanges ())
					s.Add (c);

			if (index < Regexes.Length)
				foreach (var c in Regexes [index].DerivChanges ())
					s.Add (c);

			return s;
        }

		// concat(XY)^a = X^aY if epsi not in X
		// concat(XY)^a = X^aY + Y^a  if epsi in X
        public override RegEx<T> Derivative(T c)
        {
			return ComputeDerivative (0, c);
		}

		private RegEx<T> ComputeDerivative( int depth, T c )
		{
			// the last one
			if (depth == Regexes.Length - 1)
				return Regexes[depth].Derivative(c);

			// (R[i])^a(R[i+1])....
			var this_level = RegExFactory.Concat (
				           Regexes [depth].Derivative (c),
				           ConcatRegexes (depth + 1)
			           );

			// first can be skipped
			if (Regexes [depth].HasEpsilon ())
				return RegExFactory.Union (this_level, ComputeDerivative (depth + 1, c));

			// cannot be skipped
			return this_level;
		}

		// return (R[i])(R[i+1])....(R[len-1])
		private RegEx<T> ConcatRegexes( int depth ) 
		{
			if (depth == Regexes.Length - 1)
				return Regexes [depth];

			return RegExFactory.Concat (Regexes [depth], ConcatRegexes (depth + 1));
		}
    }
}