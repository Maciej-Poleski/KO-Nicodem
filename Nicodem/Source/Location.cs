namespace Nicodem.Source
{
    public struct Location
    {
        public Location(IOrigin origin, int characterInLinePosition, int lineNumber) : this()
        {
            Origin = origin;
            CharacterInLinePosition = characterInLinePosition;
            LineNumber = lineNumber;
        }

        public IOrigin Origin { get; private set; }
        public int CharacterInLinePosition { get; private set; }
        public int LineNumber { get; private set; }
    }
}