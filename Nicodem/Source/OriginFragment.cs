using System;

namespace Nicodem.Source
{
    public class OriginFragment : IFragment
    {
        public readonly IOrigin origin;
        public readonly OriginPosition beg;
        public readonly OriginPosition end;

        public OriginFragment(IOrigin origin, OriginPosition beg, OriginPosition end){
            this.origin = origin;
            this.beg = beg;
            this.end = end;
        }

        public IOrigin Origin {
            get {
                return origin;
            }
        }

        public OriginPosition GetBeginOriginPosition()
        {
            return new OriginPosition(beg.LineNumber + 1, beg.CharNumber);
        }

        public OriginPosition GetEndOriginPosition()
        {
            return new OriginPosition(end.LineNumber + 1, end.CharNumber);
        }

        public string GetOriginText()
        {
            return origin.GetText(this);
        }
    }
}

