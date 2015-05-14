using System;
using System.Text;

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

        public string GetText(IFragment fragment)
        {
            if (fragment.Origin == this) {
                return this.GetText(fragment.GetBeginOriginPosition(), fragment.GetEndOriginPosition());
            } else {
                throw new ArgumentException("Invalid origin");
            }
        }

        internal string GetText(OriginPosition beg, OriginPosition end)
        {
            --beg.LineNumber;
            --end.LineNumber;
            if (beg.LineNumber == end.LineNumber) {
                if (beg.CharNumber == end.CharNumber)
                    return "";
                return sourceLines[beg.LineNumber].Substring(beg.CharNumber, end.CharNumber - beg.CharNumber);
            }
            else {
                StringBuilder sb = new StringBuilder();
                System.Console.WriteLine("!=");
                sb.Append(sourceLines[beg.LineNumber].Substring(beg.CharNumber));
                sb.Append('\n');
                for (int line = beg.LineNumber + 1; line < end.LineNumber; ++line)
                {
                    sb.Append(sourceLines[line]);
                    sb.Append('\n');
                }
                sb.Append(sourceLines[end.LineNumber].Substring(0, end.CharNumber));
                return sb.ToString();
            }
        }
    }
}
