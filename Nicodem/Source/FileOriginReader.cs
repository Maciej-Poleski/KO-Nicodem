using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Nicodem.Source.Tmp
{
    public class FileOriginReader : IOriginReader
    {
        private FileOrigin origin;
        private FileLocation lastLocation; // location in momet of get/set current location
        private StreamReader reader;

        public FileOriginReader(FileOrigin fileOrigin)
        {
            this.origin = fileOrigin;
            this.lastLocation = origin.begin; //location is copied here, since it is struct
            this.reader = new StreamReader(new FileStream(origin.sourcePath, FileMode.Open), System.Text.Encoding.UTF8);
        }

        // -------------- IOriginReader methods --------------
        public bool MoveNext ()
        {
            // TODO - what about first read?
            if (reader.EndOfStream)
                return false;
            reader.Read();
            return true;
        }
        public ILocation CurrentLocation {
            get {
                lastLocation.streamPos = reader.BaseStream.Position;
                // TODO maybe we should copy it before?
                return lastLocation;
            }
            set {
                if (value.Origin == origin) {
                    lastLocation.streamPos = ((FileLocation) value).streamPos;
                    reader.BaseStream.Position = lastLocation.streamPos;
                }
                throw new ArgumentException("Invalid origin");
            }
        }
        public char CurrentCharacter {
            get {
                return (char) reader.Peek();
            }
        }
    }
}
