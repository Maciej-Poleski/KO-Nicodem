namespace Nicodem.Source
{
    interface IOrigin
    {
        Location begin { get; set;}
        IOriginReader GetReader();
    }
}
