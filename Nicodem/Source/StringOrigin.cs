using System;

namespace Nicodem.Source
{
    public class StringOrigin : AbstractOrigin
    {
        internal readonly String source;

        public StringOrigin(String source) : base(source.Split('\n'))
        {
            this.source = source;
        }

        public override string ToString()
        {
            return "StringOrigin of: " + source;
        }
    }
}
