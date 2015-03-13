using System;

namespace Nicodem.Source.Tmp
{
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
            if (from.Origin == this && to.Origin == this) {
                return new StringFragment(this, ((StringLocation)from).pos, ((StringLocation)to).pos);
            } else {
                throw new ArgumentException("Invalid origin");
            }
        }
        public ILocation Begin {
            get { return begin; }
        }
    }
}
