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
        public override string ToString()
        {
            return "FileLocation: " + origin + ", " + streamPos;
        }
        // -------------- ILocation methods --------------
        public IOrigin Origin {
            get {
                return origin;
            }
        }

        // -------------- ADDITIONAL methods --------------
        public static FileLocation BeginLocation(FileOrigin origin)
        {
            return new FileLocation(origin, -1);
        }

        public bool IsOriginBegin()
        {
            return streamPos == -1;
        }
    }
}

