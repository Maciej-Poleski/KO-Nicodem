using System;

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
    }
}
