using System;

namespace Nicodem.Source
{
    public struct OriginPosition
    {
        public int LineNumber { get; internal set; }
        public int CharNumber { get; internal set; }

        internal OriginPosition(int lineNumber, int charNumber) : this() {
            this.LineNumber = lineNumber;
            this.CharNumber = charNumber;
        }
    }
}
