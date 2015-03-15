namespace Nicodem.Source
{
    public struct FileFragment : IFragment
    {
        internal readonly FileOrigin origin;
        internal readonly long streamBegPos;
        internal readonly long streamEndPos;

        internal FileFragment(FileOrigin origin, long beg, long end){
            this.origin = origin;
            this.streamBegPos = beg;
            this.streamEndPos = end;
        }
        // -------------- IFragment methods --------------
        public IOrigin Origin {
            get {
                return origin;
            }
        }
    }
}

