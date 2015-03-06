using System;
using System.Collections.Generic;

namespace Nicodem.Source
{
    /// <summary>
    ///     Rezultat pracy Leksera.
    /// </summary>
    /// <typeparam name="TOrigin">Typ użytego źródła</typeparam>
    /// <typeparam name="TMemento">
    ///     Typ użytej pamiętki
    ///     <see cref="IOriginReader{TOrigin,TMemento,TLocation,TFragment}.MakeMemento" />
    /// </typeparam>
    /// <typeparam name="TLocation">Typ lokacji</typeparam>
    /// <typeparam name="TFragment">Typ fragmentu</typeparam>
    public struct TokenizerResult<TOrigin, TMemento, TLocation, TFragment>
        where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
        where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
    {
        private readonly TLocation _endLocation;
        private readonly IEnumerable<Tuple<TFragment, IEnumerable<int>>> _tokens;

        public TokenizerResult(IEnumerable<Tuple<TFragment, IEnumerable<int>>> tokens, TLocation endLocation)
        {
            _tokens = tokens;
            _endLocation = endLocation;
        }

        /// <summary>
        ///     Lista kolejnych tokenów na które udało się podzielić pewien prefiks źródła.
        /// </summary>
        /// <value>
        ///     Każdy fragment oznacza przedział wewnątrz źródła. Jest powiązany z listą kategori (wyrażeń regularnych) do
        ///     których należy dany przedział
        /// </value>
        public IEnumerable<Tuple<TFragment, IEnumerable<int>>> Tokens
        {
            get { return _tokens; }
        }

        /// <summary>
        ///     Położenie końca ostatniego dopasowanego tokenu ze źródła. Jeżeli nie jest ono końcem źródło - czegoś zaczynając od
        ///     pozycji <code>EndLocation</code> nie udało się dopasować.
        /// </summary>
        public TLocation EndLocation
        {
            get { return _endLocation; }
        }
    }
}