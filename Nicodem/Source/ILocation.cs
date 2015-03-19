namespace Nicodem.Source
{
    public interface ILocation
    {
        IOrigin Origin { get; }
        OriginPosition GetOriginPosition();
    }
}