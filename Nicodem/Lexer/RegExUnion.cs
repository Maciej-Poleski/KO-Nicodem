using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Lexer
{
    public class RegExUnion<T> : RegEx<T> where T : IComparable<T>, IEquatable<T>
    {
        internal static readonly RegExUnion<T> RegexEmpty=new RegExUnion<T>(); 
        internal RegExUnion(params RegEx<T>[] regexes)
        {
            TypeId = 5;
            Regexes = regexes;
        }

        public RegEx<T>[] Regexes { private set; get; }

        public override int CompareTo(RegEx<T> other)
        {
            var diff = TypeId - other.TypeId;
            if (diff != 0)
                return diff;

            var union = other as RegExUnion<T>;
            diff = Regexes.Length - union.Regexes.Length;
            if (diff != 0)
                return diff;

            for (var i = 0; i < Regexes.Length; i++)
            {
                diff = Regexes[i].CompareTo(union.Regexes[i]);
                if (diff != 0)
                    return diff;
            }

            return 0;
        }

        public override bool HasEpsilon()
        {
            foreach (var i in Regexes)
            {
                if (i.HasEpsilon())
                {
                    return true;
                }
            }
            return false;
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
            return RegExFactory.Union(derivatives.ToArray());
        }
    }
}