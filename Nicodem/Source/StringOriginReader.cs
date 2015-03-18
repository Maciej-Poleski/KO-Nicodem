using System;

namespace Nicodem.Source
{
    public class StringOriginReader : IOriginReader
    {
        private StringOrigin origin;
        private StringLocation curLocation;

        public StringOriginReader(StringOrigin stringOrigin)
        {
            this.origin = stringOrigin;
            this.curLocation = origin.begin; //location is copied here, since it is struct
        }

        // -------------- IOriginReader methods --------------
        public bool MoveNext ()
        {
            if (curLocation.pos + 1 < origin.source.Length)
            {
                ++curLocation.pos;
                return true;
            }
            return false;
        }
        public ILocation CurrentLocation {
            get {
                return curLocation;
            }
            set {
                if (value.Origin == origin)
                {
                    curLocation.pos = ((StringLocation)value).pos;
                }
                else
                {
                    throw new ArgumentException("Invalid origin");
                }
            }
        }
        public char CurrentCharacter {
            get {
                return origin.source[curLocation.pos];
            }
        }

        // -------------- IDisposable Dispose --------------
        public void Dispose()
        {
        }
    }
}

