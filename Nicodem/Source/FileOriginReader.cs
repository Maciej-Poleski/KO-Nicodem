using System;

namespace Nicodem.Source
{
    public class FileOriginReader : IOriginReader
    {
        private FileOrigin origin;
        private OriginLocation curLocation;

        public FileOriginReader(FileOrigin fileOrigin)
        {
            this.origin = fileOrigin;
            this.curLocation = origin.begin;
        }

        // -------------- IOriginReader methods --------------
        public bool MoveNext()
        {
            if(origin.end.Equals(curLocation))
                return false;
            if(curLocation.pos.charNumber + 1 <= origin.sourceLines[curLocation.pos.lineNumber].Length) {
                ++curLocation.pos.charNumber;
            } else {
                ++curLocation.pos.lineNumber;
                curLocation.pos.charNumber = 0;
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
                if(curLocation.pos.charNumber == 0) {
                    return '\n';
                }
                return origin.sourceLines[curLocation.pos.lineNumber][curLocation.pos.charNumber - 1];
            }
        }

        // -------------- IDisposable Dispose --------------
        public void Dispose()
        {
        }
    }
}

