namespace Nicodem.Source
{
    public interface IFragment<TOrigin, TMemento, TLocation, TFragment>
        where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
        where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
    {
        int AfeterEndCharacterInLinePosition { get; }
        int EndLineNumber { get; }
        int BeginCharacterInLinePosition { get; }
        int BeginLineNumber { get; }
        TOrigin Origin { get; }
    }
}