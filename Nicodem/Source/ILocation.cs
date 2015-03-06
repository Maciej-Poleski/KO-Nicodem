namespace Nicodem.Source
{
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