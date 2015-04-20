using System;
using System.Collections.Generic;
using Nicodem.Source;

namespace Nicodem.Lexer
{
    public struct LexerResult
    {
        private readonly ILocation _failedAtLocation;
        private readonly ILocation _lastParsedLocation;
        private readonly IEnumerable<Tuple<IFragment, IEnumerable<int>>> _tokens;

        public LexerResult(IEnumerable<Tuple<IFragment, IEnumerable<int>>> tokens, ILocation lastParsedLocation,
            ILocation failedAtLocation)
        {
            _tokens = tokens;
            _lastParsedLocation = lastParsedLocation;
            _failedAtLocation = failedAtLocation;
        }

        /// <summary>
        ///     Lista kolejnych tokenów na które udało się podzielić pewien prefiks źródła.
        /// </summary>
        /// <value>
        ///     Każdy fragment oznacza przedział wewnątrz źródła. Jest powiązany z listą kategori (wyrażeń regularnych) do
        ///     których należy dany przedział
        /// </value>
        public IEnumerable<Tuple<IFragment, IEnumerable<int>>> Tokens
        {
            get { return _tokens; }
        }

        /// <summary>
        ///     Położenie końca ostatniego dopasowanego tokenu ze źródła. Jeżeli nie jest ono końcem źródło - czegoś zaczynając od
        ///     pozycji <code>LastParsedLocation</code> nie udało się dopasować.
        /// </summary>
        public ILocation LastParsedLocation
        {
            get { return _lastParsedLocation; }
        }

        /// <summary>
        ///     Location in origin just-after character which put Lexer in dead state (or equal to <code>LastParsedLocation</code>
        ///     if the whole origin was successfully tokenized.
        /// </summary>
        public ILocation FailedAtLocation
        {
            get { return _failedAtLocation; }
        }
    }
}