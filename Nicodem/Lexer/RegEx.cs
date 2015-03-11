using System;
using System.Collections.Generic;

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

        #endregion
    }
}