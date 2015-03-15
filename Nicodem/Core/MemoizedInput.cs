using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Core
{
    public class MemoizedInput<T>
    {

        private List<T> elements = new List<T>();
        private IEnumerator<T> current;

        public MemoizedInput(IEnumerable<T> input)
        {
            current = input.GetEnumerator();
            if(current.MoveNext()) {
                elements.Add(current.Current);
            }
        }

        public Iterator Begin
        {
            get {
                if(elements.Any()) {
                    return new Iterator(this, 0);
                } else {
                    return End;
                }
            }
        }

        public Iterator End
        {
            get {
                return new Iterator(this, -1);
            }
        }

        public Iterator At(int pos)
        {
            if(pos > elements.Count || pos < -1) {
                return End;
            } else {
                return new Iterator(this, pos);
            }
        }

        private bool GoNext() {
            if(current.MoveNext()) {
                elements.Add(current.Current);
                return true;
            } else {
                return false;
            }
        }

        public struct Iterator : IComparable<Iterator>
        {

            public int Pos { get; private set; }
            private readonly MemoizedInput<T> _parent;

            public Iterator(MemoizedInput<T> parent, int pos)
                : this()
            {
                this.Pos = parent.elements.Count > pos ? pos : -1;
                _parent = parent;
            }

            public T Current
            {
                get {
                    return _parent.elements[Pos];
                }
            }

            public Iterator Next()
            {
                if(Pos + 1 >= _parent.elements.Count) {
                    _parent.GoNext();
                }
                return new Iterator(_parent, Pos + 1);
            }

            #region IComparable implementation

            public int CompareTo(Iterator other)
            {
                if(Pos == -1) {
                    if(other.Pos == -1) {
                        return 0;
                    } else {
                        return 1;
                    }
                } else if(other.Pos == -1) {
                    return -1;
                } else {
                    return Pos - other.Pos;
                }
            }

            #endregion

            public static bool operator==(Iterator a, Iterator b)
            {
                return a.Pos == b.Pos && a._parent == b._parent;
            }

            public static bool operator!=(Iterator a, Iterator b)
            {
                return !(a.Pos == b.Pos);
            }

            public static bool operator>(Iterator a, Iterator b)
            {
                return a.CompareTo(b) > 0;
            }

            public static bool operator>=(Iterator a, Iterator b)
            {
                return a.CompareTo(b) > 0 || a == b;
            }

            public static bool operator<(Iterator a, Iterator b)
            {
                return !(a >= b);
            }

            public static bool operator<=(Iterator a, Iterator b)
            {
                return !(a > b);
            }

            public override bool Equals(object other)
            {
                if(other is Iterator) {
                    return this == (Iterator) other;
                } else {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return Pos.GetHashCode() + _parent.GetHashCode();
            }
        }
    }
}

