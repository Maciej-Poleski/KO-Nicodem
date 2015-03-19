using System;

namespace Nicodem.Source
{
    public class OriginReader : IOriginReader
    {
        private AbstractOrigin origin;
        private OriginLocation curLocation;

        public OriginReader(AbstractOrigin origin)
        {
            this.origin = origin;
            this.curLocation = origin.begin;
        }

        // -------------- IOriginReader methods --------------
        public bool MoveNext()
        {
            if(origin.end.Equals(curLocation))
                return false;
            if(curLocation.pos.CharNumber + 1 <= origin.sourceLines[curLocation.pos.LineNumber].Length) {
                ++curLocation.pos.CharNumber;
            } else {
                ++curLocation.pos.LineNumber;
                curLocation.pos.CharNumber = 0;
            }
            return true;
        }

        public ILocation CurrentLocation {
            get {
                return curLocation;
            }
            set {
                if (value.Origin == origin)
                {
                    curLocation.pos = ((OriginLocation)value).pos;
                }
                else
                {
                    throw new ArgumentException("Invalid origin");
                }
            }
        }

        public char CurrentCharacter {
            get {
                if(curLocation.pos.CharNumber == 0) {
                    return '\n';
                }
                return origin.sourceLines[curLocation.pos.LineNumber][curLocation.pos.CharNumber - 1];
            }
        }

        // -------------- IDisposable Dispose --------------
        public void Dispose()
        {
        }
    }
}

