namespace Nicodem.Source
{
    public interface IOriginReader
    {
        /// <summary>
        ///     ILocation object representing position in IOrigin. Before first call to MoveNext it
        ///     points to origin.Begin. Setting this property allows to change readers position. You
        ///     can only use ILocation with the same Origin property as IOriginReader.
        /// </summary>
        ILocation CurrentLocation { get; set; }

        /// <summary>
        ///     Character in origin lying under CurrentLocation. You have to call MoveNext before 
        ///     accessing the first character. Calls previous to MoveNext (when location is set to
        ///     origin.Begin) have undefined behaviour.
        /// </summary>
        char CurrentCharacter { get; }

        /// <summary>
        ///     Advances reader to the next character, changing CurrentLocation. If the end of the 
        ///     origin was reached - it returns false and nothing changes.
        ///     <remarks>
        ///         You have to call this method before accessing the first character.
        ///     </remarks>
        /// </summary>
        /// <returns>true if reader's location was advanced, false otherwise</returns>
        bool MoveNext();
    }
}