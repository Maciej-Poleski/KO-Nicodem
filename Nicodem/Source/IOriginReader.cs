namespace Nicodem.Source
{
    public interface IOriginReader
    {
        Location CurrentLocation { get; set; }
        char CurrentCharacter { get; set; }

        bool MoveNext();
    }
}
