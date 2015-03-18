namespace Nicodem.Source
{
    public interface IFragment
    {
        IOrigin Origin { get; }
        OriginPosition GetBeginOriginPosition();
        OriginPosition GetEndOriginPosition();
        string GetOriginText();
    }
}