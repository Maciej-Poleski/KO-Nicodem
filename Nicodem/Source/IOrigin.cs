namespace Nicodem.Source
{
    public interface IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
        where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
    {
        TLocation begin { get; }
        IOriginReader<TOrigin, TMemento, TLocation, TFragment> GetReader();
    }
}