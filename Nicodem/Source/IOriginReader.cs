namespace Nicodem.Source
{
    /// <summary>
    ///     Implementacja <see cref="IOrigin{T}" /> dostarcza implementacji funkcjonalności związanych z odczytem kodu
    ///     źródłowego poprzez obiekty implementujące ten interfejs.
    /// </summary>
    /// <typeparam name="T">Typ pamiątki tworzonej przez implementacje tego interfejsu</typeparam>
    public interface IOriginReader<T>
    {
        Location CurrentLocation { get; }
        char CurrentCharacter { get; }

        /// <summary>
        ///     Tworzy pamiątke stanu obiektu. Może być później wykorzystana do przywrócenia zapamiętanego stanu.
        /// </summary>
        /// <returns>Dowolny obiekt</returns>
        T MakeMemento();

        /// <summary>
        ///     Przywraca stan obiektu do stanu z chwili wywołania funkcji <see cref="MakeMemento" /> które zwróciło obiekt
        ///     przekazany jako <paramref name="memento" />. Wywołania funkcji <see cref="Rollback" /> będą się odbywać zawsze w
        ///     odwrotnej kolejności do wywołań <see cref="MakeMemento" /> i nigdy nie pominą w ciągu wywołań żadnego obiektu
        ///     zwróconego przez <see cref="MakeMemento" />.
        /// </summary>
        /// <param name="memento">Obiekt zwrócony przez wcześniejsze wywołanie <see cref="MakeMemento" /></param>
        void Rollback(T memento);

        bool MoveNext();
    }
}