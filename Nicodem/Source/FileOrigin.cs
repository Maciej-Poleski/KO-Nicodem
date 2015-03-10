using System;

namespace Nicodem.Source.Tmp
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

    public class FileOrigin : IOrigin
    {
        internal readonly FileLocation begin;
        internal readonly String sourcePath;

        FileOrigin(String sourcePath)
        {
            this.begin = new FileLocation(this, -1);
            this.sourcePath = sourcePath;
        }

        // -------------- IOrigin methods --------------
        public IOriginReader GetReader ()
        {
            return new FileOriginReader(this);
        }
        public IFragment MakeFragment (ILocation from, ILocation to)
        {
            if (from.Origin == this && to.Origin == this) {
                return new FileFragment (this, ((FileLocation) from).streamPos, ((FileLocation) to).streamPos);
            }
            throw new ArgumentException("Invalid origin");
        }
        public ILocation Begin {
            get {
                return begin;
            }
        }
    }
}
