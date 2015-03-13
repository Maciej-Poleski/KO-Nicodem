namespace Nicodem.Source.Tmp
{
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
}

