namespace Nicodem.Source
{
    public interface IOrigin
    {
        Location begin { get; set;}
        IOriginReader GetReader();
    }
}
