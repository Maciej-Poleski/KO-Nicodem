namespace Nicodem.Source
{
    [System.Obsolete("change of interface, new version in Tmp sub namespace")]
    public interface IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
        where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
    {
        TLocation begin { get; }
        IOriginReader<TOrigin, TMemento, TLocation, TFragment> GetReader();

        /// <summary>
        ///     Tworzy fragment reprezentujący przedział od lokacji <paramref name="from" /> do lokacji <paramref name="to" />.
        ///     <remarks>
        ///         Obie lokacje powinny odnosić się do tego źródła.
        ///     </remarks>
        /// </summary>
        /// <param name="from">Początek fragmentu</param>
        /// <param name="to">Koniec fragmentu (znak wskazywany przez tą lokacje powinien NIE być częścią wynikowego fragmentu)</param>
        /// <returns>Fragment reprezentujący dany przedział</returns>
        TFragment MakeFragment(TLocation from, TLocation to);
    }
}

namespace Nicodem.Source.Tmp
{
    public interface IOrigin
    {
        ILocation Begin { get; }
        IOriginReader GetReader();
        IFragment MakeFragment(ILocation from, ILocation to);
    }
}