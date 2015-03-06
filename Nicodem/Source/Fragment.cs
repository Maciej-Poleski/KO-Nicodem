namespace Nicodem.Source
{
    public struct Fragment
    {
        public Fragment(IOrigin origin, int beginLineNumber, int beginCharacterInLinePosition, int endLineNumber,
            int afeterEndCharacterInLinePosition) : this()
        {
            Origin = origin;
            BeginLineNumber = beginLineNumber;
            BeginCharacterInLinePosition = beginCharacterInLinePosition;
            EndLineNumber = endLineNumber;
            AfeterEndCharacterInLinePosition = afeterEndCharacterInLinePosition;
        }

        public int AfeterEndCharacterInLinePosition { get; private set; }
        public int EndLineNumber { get; private set; }
        public int BeginCharacterInLinePosition { get; private set; }
        public int BeginLineNumber { get; private set; }
        public IOrigin Origin { get; private set; }
    }
}