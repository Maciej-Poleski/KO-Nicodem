using System;

namespace Nicodem.Source
{
    public struct OriginLocation : ILocation
    {
        internal readonly IOrigin origin;
        internal OriginPosition pos;

        internal OriginLocation(IOrigin origin, OriginPosition pos){
            this.origin = origin;
            this.pos = pos;
        }

        public override string ToString()
        {
            return "line: " + pos.lineNumber + ":" + pos.charNumber; 
        }

        // -------------- ILocation methods --------------
        public IOrigin Origin {
            get {
                return origin;
            }
        }
    }
}
