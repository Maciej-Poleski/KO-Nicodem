using System;
using System.IO;

namespace Nicodem.Source
{
    public class AbstractOrigin : IOrigin
    {
        internal readonly OriginLocation begin;
        internal readonly OriginLocation end;
        internal readonly string[] sourceLines;

        protected AbstractOrigin(string[] sourceLines)
        {
            this.begin = new OriginLocation(this, new OriginPosition(0, 0));
            this.sourceLines = sourceLines;
            int nLines = sourceLines.Length;
            if (nLines == 0) {
                this.end = begin;
            } else {
                this.end = new OriginLocation(this, new OriginPosition(nLines - 1, sourceLines[nLines - 1].Length));
            }
        }

        // -------------- IOrigin methods --------------
        public IOriginReader GetReader()
        {
            return new OriginReader(this);
        }
        public IFragment MakeFragment(ILocation from, ILocation to)
        {
            if (from.Origin == this && to.Origin == this) {
                return new OriginFragment(this, ((OriginLocation)from).pos, ((OriginLocation)to).pos);
            } else {
                throw new ArgumentException("Invalid origin");
            }
        }
        public ILocation Begin
        {
            get
            {
                return begin;
            }
        }
    }
}
