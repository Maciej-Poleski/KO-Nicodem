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
}

