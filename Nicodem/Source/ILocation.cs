namespace Nicodem.Source
{
    [System.Obsolete("change of interface, new version in Tmp sub namespace")]
    public interface ILocation<TOrigin, TMemento, TLocation, TFragment>
        where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
        where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
    {
        TOrigin Origin { get; }
        int CharacterInLinePosition { get; }
        int LineNumber { get; }
    }
}

namespace Nicodem.Source.Tmp
{
    public interface ILocation
    {
        IOrigin Origin { get; }
    }
}