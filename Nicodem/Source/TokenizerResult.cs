using System;
using System.Collections.Generic;

namespace Nicodem.Source
{
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

        public IEnumerable<Tuple<TFragment, IEnumerable<int>>> Tokens
        {
            get { return _tokens; }
        }

        public TLocation EndLocation
        {
            get { return _endLocation; }
        }
    }
}