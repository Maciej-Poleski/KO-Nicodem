using System;
using System.Text;

namespace Nicodem.Source
{
    public class SourceDiagnostic
    {
        const string messageFormat = "{0} at line {1}:{2} \"{3}\" - {4}";

        public string PrepareMessage(string errorType, IFragment sourceFragment, string errorDescription)
        {
            OriginPosition pos = sourceFragment.GetBeginOriginPosition();
            return String.Format(messageFormat, errorType, pos.LineNumber, pos.CharNumber, 
                sourceFragment.GetOriginText(), errorDescription);
        }

        public void PrintMessage(string errorType, IFragment sourceFragment, string errorDescription)
        {
            System.Console.WriteLine(PrepareMessage(errorType, sourceFragment, errorDescription));
        }

        public void PrintFragmentInLine(IFragment fragment)
        {
            IOrigin origin = fragment.Origin;
            OriginPosition begPos = fragment.GetBeginOriginPosition();
            OriginPosition endPos = fragment.GetEndOriginPosition();
            int start = begPos.CharNumber;
            int end = endPos.CharNumber;
            System.Console.WriteLine(origin.GetLine(begPos.LineNumber));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < start; i++)
                sb.Append(' ');
            for (int i = start; i < end; i++)
                sb.Append((i + 1 == end) ? '^' : '~');
            System.Console.WriteLine(sb.ToString());
        }
    }
}
