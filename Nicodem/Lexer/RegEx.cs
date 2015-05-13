﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
    public abstract class RegEx<T> : IComparable<RegEx<T>>, IEquatable<RegEx<T>>
        where T : IComparable<T>, IEquatable<T>
    {
        internal int TypeId { set; get; }

        #region IComparable implementation

        public virtual int CompareTo(RegEx<T> other)
        {
            //Derived classes should override this method
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable implementation

        public bool Equals(RegEx<T> other)
        {
            // should be correlated with compareTo
            return CompareTo(other) == 0;
        }

        #endregion

        public virtual bool HasEpsilon()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<T> DerivChanges()
        {
            throw new NotImplementedException();
        }

        public virtual RegEx<T> Derivative(T c)
        {
            throw new NotImplementedException();
        }

        #region standard methods

        public override bool Equals(object obj)
        {
            var regex = obj as RegEx<T>;
            return regex != null && Equals(regex);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

		public override string ToString ()
		{
			return string.Format ("RegEx ");
		}

		protected bool isRegExComplex( RegEx<T> r ) {
			var intersection = r as RegExIntersection<T>;
			var union = r as RegExUnion<T>;

			if (intersection != null)
				return intersection.Regexes.Length > 1;

			if (union != null)
				return union.Regexes.Length > 1;

			return false;
		}

        #endregion

		public static RegEx<N> Convert<N>(RegEx<T> regex, Func<T, N> converter)
			where N : IComparable<N>, IEquatable<N>
		{
			switch(regex.TypeId) {
			case 0:
				return RegExEpsilon<N>.RegexEpsilon;
			case 1:
				{
					var r = regex as RegExRange<T>;
					return new RegExRange<N>(converter(r.Character));
				}
			case 2:
				{
					var r = regex as RegExStar<T>;
					return new RegExStar<N>(Convert(r.Regex, converter));
				}
			case 3:
				{
					var r = regex as RegExComplement<T>;
					return new RegExComplement<N>(Convert(r.Regex, converter));
				}
			case 4:
				{
					var r = regex as RegExConcat<T>;
					var l = new LinkedList<RegEx<N>>();
					foreach(var recr in r.Regexes) {
						l.AddLast(Convert(recr, converter));
					}
					return new RegExConcat<N>(l.ToArray());
				}
			case 5:
				{
					var r = regex as RegExUnion<T>;
					var l = new LinkedList<RegEx<N>>();
					foreach(var recr in r.Regexes) {
						l.AddLast(Convert(recr, converter));
					}
					return new RegExUnion<N>(l.ToArray());
				}
			case 6:
				{
					var r = regex as RegExIntersection<T>;
					var l = new LinkedList<RegEx<N>>();
					foreach(var recr in r.Regexes) {
						l.AddLast(Convert(recr, converter));
					}
					return new RegExIntersection<N>(l.ToArray());
				}
			}
			return null;
		}
			
    }
}
