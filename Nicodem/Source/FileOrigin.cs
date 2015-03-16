using System;

namespace Nicodem.Source
{
    public class FileOrigin : IOrigin
    {
        internal readonly FileLocation begin;
        internal readonly String sourcePath;

        public FileOrigin(String sourcePath)
        {
            this.begin = FileLocation.BeginLocation(this);
            this.sourcePath = sourcePath;
        }

        public override string ToString()
        {
            return "FileOrigin of: " + sourcePath;
        }

        // -------------- IOrigin methods --------------
        public IOriginReader GetReader ()
        {
            return new FileOriginReader(this);
        }
        public IFragment MakeFragment (ILocation from, ILocation to)
        {
            if (from.Origin == this && to.Origin == this)
            {
                return new FileFragment(this, ((FileLocation)from).streamPos, ((FileLocation)to).streamPos);
            }
            else
            {
                throw new ArgumentException("Invalid origin");
            }
        }
        public ILocation Begin {
            get {
                return begin;
            }
        }
    }
}
