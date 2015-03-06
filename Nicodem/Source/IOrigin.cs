namespace Nicodem.Source
{
    public interface IOrigin<T>
    {
        Location begin { get;}
        IOriginReader<T> GetReader();
    }
}
