namespace Nicodem.Source
{
    public struct FileLocation : ILocation
    {
        internal readonly FileOrigin origin;
        internal long streamPos;
        internal FileLocation(FileOrigin origin, long streamPos){
            this.origin = origin;
            this.streamPos = streamPos;
        }
        // -------------- ILocation methods --------------
        public IOrigin Origin {
            get {
                return origin;
            }
        }
    }
}

