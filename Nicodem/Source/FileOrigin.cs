using System;
using System.IO;

namespace Nicodem.Source
{
    public class FileOrigin : IOrigin
    {
        internal readonly OriginLocation begin;
		internal readonly OriginLocation end;
        internal readonly String sourcePath;
        internal readonly string[] sourceLines;

        public FileOrigin(String sourcePath)
        {
			this.begin = new OriginLocation(this, new OriginPosition(0,0));
            this.sourcePath = sourcePath;
            this.sourceLines = File.ReadAllLines(sourcePath,System.Text.Encoding.UTF8);
			int nLines = sourceLines.Length;
			if (nLines == 0) {
				this.end = begin;
			} else {
				this.end = new OriginLocation(this, new OriginPosition(nLines-1, sourceLines[nLines-1].Length));
			}
        }

        public override string ToString()
        {
            return "FileOrigin of: " + sourcePath;
        }

        // -------------- IOrigin methods --------------
        public IOriginReader GetReader()
        {
            return new FileOriginReader(this);
        }
        public IFragment MakeFragment(ILocation from, ILocation to)
        {
            if (from.Origin == this && to.Origin == this)
            {
                return new OriginFragment(this, ((OriginLocation)from).pos, ((OriginLocation)to).pos);
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
