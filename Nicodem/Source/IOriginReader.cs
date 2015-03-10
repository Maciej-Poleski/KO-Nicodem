namespace Nicodem.Source
{
    /// <summary>
    ///     Implementacja <see cref="IOrigin{T}" /> dostarcza implementacji funkcjonalności związanych z odczytem kodu
    ///     źródłowego poprzez obiekty implementujące ten interfejs.
    /// </summary>
    [System.Obsolete("change of interface, new version in Tmp sub namespace")]
    public interface IOriginReader<TOrigin, TMemento, TLocation, TFragment>
        where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
        where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
        where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
    {
        TLocation CurrentLocation { get; }
        char CurrentCharacter { get; }

        /// <summary>
        ///     Tworzy pamiątke stanu obiektu. Może być później wykorzystana do przywrócenia zapamiętanego stanu.
        /// </summary>
        /// <returns>Dowolny obiekt</returns>
        TMemento MakeMemento();

        /// <summary>
        ///     Przywraca stan obiektu do stanu z chwili wywołania funkcji <see cref="MakeMemento" /> które zwróciło obiekt
        ///     przekazany jako <paramref name="memento" />. Wywołania funkcji <see cref="Rollback" /> będą się odbywać zawsze w
        ///     odwrotnej kolejności do wywołań <see cref="MakeMemento" /> i nigdy nie pominą w ciągu wywołań żadnego obiektu
        ///     zwróconego przez <see cref="MakeMemento" />.
        /// </summary>
        /// <param name="memento">Obiekt zwrócony przez wcześniejsze wywołanie <see cref="MakeMemento" /></param>
        void Rollback(TMemento memento);

        bool MoveNext();
    }
}

namespace Nicodem.Source.Tmp
{
    public interface IOriginReader
    {
        ILocation CurrentLocation { get; set; }
        char CurrentCharacter { get; }
        bool MoveNext();
    }
}