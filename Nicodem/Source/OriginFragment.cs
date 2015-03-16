using System;

namespace Nicodem.Source
{
    public class OriginFragment : IFragment
    {
        internal readonly IOrigin origin;
        internal readonly OriginPosition beg;
        internal readonly OriginPosition end;

        internal OriginFragment(IOrigin origin, OriginPosition beg, OriginPosition end){
            this.origin = origin;
            this.beg = beg;
            this.end = end;
        }

        public IOrigin Origin {
            get {
                return origin;
            }
        }
    }
}

