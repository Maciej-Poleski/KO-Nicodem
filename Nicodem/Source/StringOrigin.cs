using System;
using System.Collections.Generic;
using System.Text;

namespace Nicodem.Source.Tmp
{
    public struct StringLocation : ILocation
    {
        internal readonly StringOrigin origin;
        internal int pos;
        internal StringLocation(StringOrigin origin, int pos){
            this.origin = origin;
            this.pos = pos;
        }
        // -------------- ILocation methods --------------
        public IOrigin Origin {
            get {
                return origin;
            }
        }
    }

    public struct StringFragment : IFragment
    {
        internal readonly StringOrigin origin;
        internal readonly int beg;
        internal readonly int end;

        internal StringFragment(StringOrigin origin, int beg, int end){
            this.origin = origin;
            this.beg = beg;
            this.end = end;
        }
        // -------------- IFragment methods --------------
        public IOrigin Origin {
            get {
                return origin;
            }
        }
    }

    public class StringOrigin : IOrigin
    {
        internal readonly StringLocation begin;
        internal readonly String source;

        public StringOrigin(String source)
        {
            this.begin = new StringLocation(this, -1);
            this.source = source;
        }

        public override string ToString()
        {
            return "StringOrigin of: " + source;
        }

        // -------------- IOrigin methods --------------
        public IOriginReader GetReader ()
        {
            return new StringOriginReader(this);
        }
        public IFragment MakeFragment (ILocation from, ILocation to)
        {
            if (from.Origin == this && to.Origin == this)
            {
                return new StringFragment(this, ((StringLocation)from).pos, ((StringLocation)to).pos);
            }
            else
            {
                throw new ArgumentException("Invalid origin");
            }
        }
        public ILocation Begin {
            get {
                return begin;
            }
        }
    }
}
