using System;

namespace Nicodem.Source
{
    public struct OriginPosition
    {
		internal int lineNumber;
		internal int charNumber;

        internal OriginPosition(int lineNumber, int charNumber){
			this.lineNumber = lineNumber;
			this.charNumber = charNumber;
        }
    }
}
