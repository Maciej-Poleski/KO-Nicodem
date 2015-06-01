using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IFragment = Nicodem.Source.IFragment;
using ILocation = Nicodem.Source.ILocation;
using IOriginReader = Nicodem.Source.IOriginReader;

namespace Nicodem.Lexer
{
    /// <summary>
    ///     Odpowiada za podział źródła na fragmenty (tokeny) przy użyciu podanych wyrażeń regularnych.
    ///     Każde wyrażenie regularne będzie odpowiadać pewnej kategori tokenów (oznaczonej numerem - pozycją wyrażenia
    ///     regularnego w tablicy przekazanej w konstruktorze <seealso cref="Lexer(RegEx{char}[])" />).
    ///     Obiekt tej klasy może być wielokrotnie wykorzystany do tokenizacji wielu źródeł.
    /// </summary>
    public class Lexer
    {
        private readonly uint _atomicCategoryLimit; // i <= _atomicCategoryLimit => i nie wymaga dalszej dekompresji

        private readonly Dictionary<Tuple<uint, uint>, uint> _compressionMapping =
            new Dictionary<Tuple<uint, uint>, uint>();

        private readonly Dictionary<uint, Tuple<uint, uint>> _decompressionMapping =
            new Dictionary<uint, Tuple<uint, uint>>();

        //private readonly DfaUtils.MinimizedDfa<char> _dfa;
        private readonly IDfa<char> _dfa;
        private uint _nextCategory;

        /// <summary>
        ///     Tworzy lekser tokenizujący wejście przy użyciu podanych wyrażeń regularnych.
        /// </summary>
        /// <param name="regexCategories">
        ///     Wyrażenia regularne które będą używane do tokenizowania źródeł. Fragmenty dopasowane do
        ///     <value>regexCategories[i]</value>
        ///     będą oznaczane kategorią
        ///     <code>i</code>
        /// </param>
        public Lexer(params RegEx<char>[] regexCategories)
        {
            _atomicCategoryLimit = (uint) regexCategories.Length;
            _nextCategory = _atomicCategoryLimit + 1;
            if (regexCategories.Length == 0)
            {
                _dfa = DfaUtils.MakeEmptyLanguageDfa<char>();
                return;
            }
            IDfa<char> lastDfa = MakeRegexDfa(regexCategories[0], 1); //.Minimized<RegExDfa<char>, DFAState<char>, char>();
            DfaUtils.DfaStatesConcpetCheck<char>.CheckDfaStates(lastDfa);
            for (uint i = 1; i < regexCategories.Length; ++i)
            {
                lastDfa =
                    DfaUtils
                        .MakeMinimizedProductDfa
                        <//DfaUtils.MinimizedDfa<char>, DfaUtils.MinimizedDfaState<char>, RegExDfa<char>, DFAState<char>,
                            char>(lastDfa, MakeRegexDfa(regexCategories[i], i + 1), GetProductAccepting);
            }
            _dfa = lastDfa;
        }

        private uint GetProductAccepting(uint lastDfaAccepting, uint newDfaAccepting)
        {
            if (lastDfaAccepting == 0)
            {
                return newDfaAccepting;
            }
            if (newDfaAccepting == 0)
            {
                return lastDfaAccepting;
            }
            var pack = new Tuple<uint, uint>(lastDfaAccepting, newDfaAccepting);
            if (_compressionMapping.ContainsKey(pack))
            {
                return _compressionMapping[pack];
            }
            var category = _nextCategory++;
            _compressionMapping[pack] = category;
            _decompressionMapping[category] = pack;
            return category;
        }

        private static RegExDfa<char> MakeRegexDfa(RegEx<char> regex, uint category)
        {
            var result = new RegExDfa<char>(regex, category);
            DfaUtils.DfaStatesConcpetCheck<char>.CheckDfaStates(result);
            return result;
        }

        /// <summary>
        ///     Dzieli podane (pojedyńcze) źródło (<seealso cref="IOrigin{TOrigin,TMemento,TLocation,TFragment}" />) na tokeny przy
        ///     użyciu wyrażeń regularnych dostarczonych do Leksera w konstruktorze. Każdy token będzie miał przypisany zbiór
        ///     kategori - listę (<see cref="IEnumerable{int}" />) składającą się z indeksów tych elementów tablicy wyrażeń
        ///     regularnych (podanej w konstruktorze) które zostały dopasowane do danego wyrażenia regularnego.
        ///     Wynikiem jest ciąg kolejnych tokenów (<see cref="IFragment{TOrigin,TMemento,TLocation,TFragment}" />) na które
        ///     udało się podzielić źródło. Jeżeli ostatni token (<code>LastParsedLocation</code>) kończy się przed końcem źródła, to
        ///     znaczy że dalszej jego części nie udało się dopasować do żadnego wyrażenia regularnego.
        /// </summary>
        /// <param name="sourceOrigin">Źródło które będzie tokenizowane</param>
        /// <returns>
        ///     Lista tokenów powiązanych z informacją o kategoriach do których dany token należy. Lista składa się z najdłuższych
        ///     tokenów najdłuższego prefiksu źródła, którego udało się sparsować.
        /// </returns>
        private TokenizerResult<TOrigin, TMemento, TLocation, TFragment> Process
            <TOrigin, TMemento, TLocation, TFragment>
            (TOrigin sourceOrigin)
            where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
            where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
            where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
        {
            var result = new List<Tuple<TFragment, IEnumerable<int>>>();
            var sourceReader = sourceOrigin.GetReader();
            var lastAcceptedLocation = sourceReader.CurrentLocation;
            TLocation failedAtLocation;
            for (;;)
            {
                var succeed = false;
                var dfaState = _dfa.Start;
                var lastAcceptedDfaState = dfaState;
                TMemento lastAcceptingReaderState;
                if (dfaState.IsAccepting</*DfaUtils.MinimizedDfaState<char>,*/ char>())
                {
                    lastAcceptingReaderState = sourceReader.MakeMemento();
                    succeed = true;
                    // Potencjalnie dopuszcza zapętlenie przez akceptowanie pustych słów jeżeli takie istnieją w języku
                }
                else
                {
                    lastAcceptingReaderState = default(TMemento);
                }
                //FIXME: use isDead() when DFA minimization will be fixed
                while (!dfaState.IsPseudoDead() && sourceReader.MoveNext())
                {
                    //Debug.Assert(dfaState.IsDead() == dfaState.IsPseudoDead()); 
                    var c = sourceReader.CurrentCharacter;
                    dfaState = FindTransition(dfaState.Transitions.ToArray(), c);
                    if (dfaState.IsAccepting</*DfaUtils.MinimizedDfaState<char>,*/ char>())
                    {
                        lastAcceptingReaderState = sourceReader.MakeMemento();
                        lastAcceptedDfaState = dfaState;
                        succeed = true;
                    }
                }
                //Debug.Assert(dfaState.IsDead() == dfaState.IsPseudoDead()); // DFA _is_ minimized
                if (succeed)
                {
                    sourceReader.Rollback(lastAcceptingReaderState);
                    dfaState = lastAcceptedDfaState;
                    var currentLocation = sourceReader.CurrentLocation;
                    var currentFrame = currentLocation.Origin.MakeFragment(lastAcceptedLocation, currentLocation);
                    result.Add(new Tuple<TFragment, IEnumerable<int>>(currentFrame, GetCategoriesFromState(dfaState)));
                    lastAcceptedLocation = currentLocation;
                }
                else
                {
                    failedAtLocation = sourceReader.CurrentLocation;
                    break;
                }
            }
            return new TokenizerResult<TOrigin, TMemento, TLocation, TFragment>(result, lastAcceptedLocation,
                failedAtLocation);
        }

        public LexerResult Process(Source.IOrigin origin)
        {
            var result = Process<BareOrigin, ILocation, BareLocation, BareFragment>(new BareOrigin(origin));
            return new LexerResult(
                from tuple in result.Tokens
                select new Tuple<IFragment, IEnumerable<int>>(tuple.Item1.Fragment, tuple.Item2),
                result.LastParsedLocation._location,
                result.FailedAtLocation._location
                );
        }

        [Obsolete]
        public IEnumerable<Tuple<IFragment, IEnumerable<int>>> ProcessBare(Source.IOrigin origin)
        {
            return Process(origin).Tokens;
        }

        #region CategoryDecompression

        private IEnumerable<int> GetCategoriesFromState(IDfaState<char> dfaState)
        {
            return new CategoryEnumerable(this, dfaState.Accepting);
        }

        private static IDfaState<char> FindTransition(KeyValuePair<char, IDfaState<char>>[] transitions, char c)
        {
            var i = Array.FindLastIndex(transitions, pair => pair.Key <= c);
            Debug.Assert(transitions[i].Key <= c);
            Debug.Assert(i + 1 == transitions.Length || transitions[i + 1].Key > c);
            return transitions[i].Value;
        }

        private class CategoryEnumerable : IEnumerable<int>
        {
            private readonly uint _category;
            private readonly Lexer _lexer;

            internal CategoryEnumerable(Lexer lexer, uint category)
            {
                _lexer = lexer;
                Debug.Assert(category != 0);
                _category = category;
            }

            public IEnumerator<int> GetEnumerator()
            {
                return new CategoryEnumerator(_lexer, _category);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class CategoryEnumerator : IEnumerator<int>
            {
                private readonly uint _category;
                private readonly Lexer _lexer;
                private uint _currentCategory;

                internal CategoryEnumerator(Lexer lexer, uint category)
                {
                    _lexer = lexer;
                    _category = category;
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (_currentCategory == 0)
                    {
                        _currentCategory = _category;
                        return true;
                    }
                    if (_currentCategory <= _lexer._atomicCategoryLimit)
                    {
                        return false;
                    }
                    _currentCategory = _lexer._decompressionMapping[_currentCategory].Item1;
                    return true;
                }

                public void Reset()
                {
                    _currentCategory = 0;
                }

                public int Current
                {
                    get
                    {
                        if (_currentCategory <= _lexer._atomicCategoryLimit)
                        {
                            return (int) (_currentCategory - 1);
                        }
                        return (int) (_lexer._decompressionMapping[_currentCategory].Item2 - 1);
                    }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }
            }
        }

        #endregion

        #region BareSourceWrappers

        private struct BareOrigin : IOrigin<BareOrigin, ILocation, BareLocation, BareFragment>
        {
            private readonly Source.IOrigin _origin;

            public BareOrigin(Source.IOrigin origin)
            {
                _origin = origin;
            }

            public BareLocation begin
            {
                get { return new BareLocation(_origin.Begin); }
            }

            public IOriginReader<BareOrigin, ILocation, BareLocation, BareFragment> GetReader()
            {
                return new BareOriginReader(_origin.GetReader());
            }

            public BareFragment MakeFragment(BareLocation from, BareLocation to)
            {
                return new BareFragment(_origin.MakeFragment(from._location, to._location));
            }
        }

        private struct BareOriginReader : IOriginReader<BareOrigin, ILocation, BareLocation, BareFragment>
        {
            private readonly IOriginReader _originReader;

            public BareOriginReader(IOriginReader originReader)
            {
                _originReader = originReader;
            }

            public BareLocation CurrentLocation
            {
                get { return new BareLocation(_originReader.CurrentLocation); }
            }

            public char CurrentCharacter
            {
                get { return _originReader.CurrentCharacter; }
            }

            public ILocation MakeMemento()
            {
                return _originReader.CurrentLocation;
            }

            public void Rollback(ILocation memento)
            {
                _originReader.CurrentLocation = memento;
            }

            public bool MoveNext()
            {
                return _originReader.MoveNext();
            }
        }

        private struct BareLocation : ILocation<BareOrigin, ILocation, BareLocation, BareFragment>
        {
            internal readonly ILocation _location;

            public BareLocation(ILocation location)
            {
                _location = location;
            }

            public BareOrigin Origin
            {
                get { return new BareOrigin(_location.Origin); }
            }
        }

        private struct BareFragment : IFragment<BareOrigin, ILocation, BareLocation, BareFragment>
        {
            private readonly IFragment _fragment;

            public BareFragment(IFragment fragment)
            {
                _fragment = fragment;
            }

            internal IFragment Fragment
            {
                get { return _fragment; }
            }

            public BareOrigin Origin
            {
                get { return new BareOrigin(_fragment.Origin); }
            }
        }

        #endregion

        #region BareSourceInterfaces

        private interface IFragment<TOrigin, TMemento, TLocation, TFragment>
            where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
            where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
            where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
        {
            TOrigin Origin { get; }
        }

        private interface ILocation<TOrigin, TMemento, TLocation, TFragment>
            where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
            where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
            where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
        {
            TOrigin Origin { get; }
        }

        private interface IOrigin<TOrigin, TMemento, TLocation, TFragment>
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

        /// <summary>
        ///     Implementacja <see cref="IOrigin{T}" /> dostarcza implementacji funkcjonalności związanych z odczytem kodu
        ///     źródłowego poprzez obiekty implementujące ten interfejs.
        /// </summary>
        private interface IOriginReader<TOrigin, TMemento, TLocation, TFragment>
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

        /// <summary>
        ///     Rezultat pracy Leksera.
        /// </summary>
        /// <typeparam name="TOrigin">Typ użytego źródła</typeparam>
        /// <typeparam name="TMemento">
        ///     Typ użytej pamiątki
        ///     <see cref="IOriginReader{TOrigin,TMemento,TLocation,TFragment}.MakeMemento" />
        /// </typeparam>
        /// <typeparam name="TLocation">Typ lokacji</typeparam>
        /// <typeparam name="TFragment">Typ fragmentu</typeparam>
        private struct TokenizerResult<TOrigin, TMemento, TLocation, TFragment>
            where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
            where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
            where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
        {
            private readonly IEnumerable<Tuple<TFragment, IEnumerable<int>>> _tokens;
            private readonly TLocation _lastParsedLocation;
            private readonly TLocation _failedAtLocation;

            public TokenizerResult(IEnumerable<Tuple<TFragment, IEnumerable<int>>> tokens, TLocation lastParsedLocation, TLocation failedAtLocation)
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
            public IEnumerable<Tuple<TFragment, IEnumerable<int>>> Tokens
            {
                get { return _tokens; }
            }

            /// <summary>
            ///     Położenie końca ostatniego dopasowanego tokenu ze źródła. Jeżeli nie jest ono końcem źródło - czegoś zaczynając od
            ///     pozycji <code>LastParsedLocation</code> nie udało się dopasować.
            /// </summary>
            public TLocation LastParsedLocation
            {
                get { return _lastParsedLocation; }
            }

            /// <summary>
            ///     Location in origin just-after character which put Lexer in dead state.
            /// </summary>
            public TLocation FailedAtLocation
            {
                get { return _failedAtLocation; }
            }
        }

        #endregion
    }
}