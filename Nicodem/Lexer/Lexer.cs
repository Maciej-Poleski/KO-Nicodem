using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nicodem.Source;
using Nicodem.Source.Tmp;

namespace Nicodem.Lexer
{
    /// <summary>
    ///     Odpowiada za podział źródła na fragmenty (tokeny) przy użyciu podanych wyrażeń regularnych.
    ///     Każde wyrażenie regularne będzie odpowiadać pewnej kategori tokenów (oznaczonej numerem - pozycją wyrażenia
    ///     regularnego w tablicy przekazanej w konstruktorze <seealso cref="Lexer(RegEx<char>[])" />).
    ///     Obiekt tej klasy może być wielokrotnie wykorzystany do tokenizacji wielu źródeł.
    /// </summary>
    public class Lexer
    {
        private readonly uint _atomicCategoryLimit; // i <= _atomicCategoryLimit => i nie wymaga dalszej dekompresji

        private readonly Dictionary<Tuple<uint, uint>, uint> _compressionMapping =
            new Dictionary<Tuple<uint, uint>, uint>();

        private readonly Dictionary<uint, Tuple<uint, uint>> _decompressionMapping =
            new Dictionary<uint, Tuple<uint, uint>>();

        private readonly RegexDfa<char> _dfa;
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
        public Lexer(RegEx<char>[] regexCategories)
        {
            _atomicCategoryLimit = (uint) regexCategories.Length;
            _nextCategory = _atomicCategoryLimit + 1;
            if (regexCategories.Length == 0)
            {
                _dfa = ProductDfa.MakeEmptyLanguageDfa().Minimized<ProductDfa, ProductDfaState, char>();
                return;
            }
            var lastDfa = MakeRegexDfa(regexCategories[0], 1).Minimized<RegexDfa<char>, DFAState<char>, char>();
            for (uint i = 1; i < regexCategories.Length; ++i)
            {
                lastDfa = MakeMinimizedProductDfa(lastDfa, MakeRegexDfa(regexCategories[i], i + 1));
            }
            _dfa = lastDfa;
        }

        private ProductDfa MakeProductDfa<T, TU, U, UU>(T lastDfa, U newDfa)
            where T : IDfa<TU, char>
            where TU : IDfaState<TU, char>
            where U : IDfa<UU, char>
            where UU : IDfaState<UU, char>
        {
            return new ProductDfaBuilder<TU, UU>(this).Build(lastDfa.Start, newDfa.Start);
        }

        private RegexDfa<char> MakeMinimizedProductDfa(RegexDfa<char> lastDfa, RegexDfa<char> newDfa)
        {
            return MakeProductDfa<RegexDfa<char>, DFAState<char>, RegexDfa<char>, DFAState<char>>(lastDfa, newDfa)
                .Minimized<ProductDfa, ProductDfaState, char>();
        }

        private static RegexDfa<char> MakeRegexDfa(RegEx<char> regex, uint category)
        {
            return new RegexDfa<char>(regex, category);
        }

        /// <summary>
        ///     Dzieli podane (pojedyńcze) źródło (<seealso cref="IOrigin{TOrigin,TMemento,TLocation,TFragment}" />) na tokeny przy
        ///     użyciu wyrażeń regularnych dostarczonych do Leksera w konstruktorze. Każdy token będzie miał przypisany zbiór
        ///     kategori - listę (<see cref="IEnumerable{int}" />) składającą się z indeksów tych elementów tablicy wyrażeń
        ///     regularnych (podanej w konstruktorze) które zostały dopasowane do danego wyrażenia regularnego.
        ///     Wynikiem jest ciąg kolejnych tokenów (<see cref="IFragment{TOrigin,TMemento,TLocation,TFragment}" />) na które
        ///     udało się podzielić źródło. Jeżeli ostatni token (<code>EndLocation</code>) kończy się przed końcem źródła, to
        ///     znaczy że dalszej jego części nie udało się dopasować do żadnego wyrażenia regularnego.
        /// </summary>
        /// <param name="sourceOrigin">Źródło które będzie tokenizowane</param>
        /// <returns>
        ///     Lista tokenów powiązanych z informacją o kategoriach do których dany token należy. Lista składa się z najdłuższych
        ///     tokenów najdłuższego prefiksu źródła, którego udało się sparsować.
        /// </returns>
        public TokenizerResult<TOrigin, TMemento, TLocation, TFragment> Process<TOrigin, TMemento, TLocation, TFragment>
            (TOrigin sourceOrigin)
            where TOrigin : IOrigin<TOrigin, TMemento, TLocation, TFragment>
            where TLocation : ILocation<TOrigin, TMemento, TLocation, TFragment>
            where TFragment : IFragment<TOrigin, TMemento, TLocation, TFragment>
        {
            var result = new List<Tuple<TFragment, IEnumerable<int>>>();
            var sourceReader = sourceOrigin.GetReader();
            var lastAcceptedLocation = sourceReader.CurrentLocation;
            for (;;)
            {
                var succeed = false;
                var dfaState = _dfa.Start;
                var lastAcceptedDfaState = dfaState;
                TMemento lastAcceptingReaderState;
                if (dfaState.IsAccepting<DFAState<char>, char>())
                {
                    lastAcceptingReaderState = sourceReader.MakeMemento();
                    succeed = true;
                    // Potencjalnie dopuszcza zapętlenie przez akceptowanie pustych słów jeżeli takie istnieją w języku
                }
                else
                {
                    lastAcceptingReaderState = default(TMemento);
                }
                while (!dfaState.IsDead() && sourceReader.MoveNext())
                {
                    var c = sourceReader.CurrentCharacter;
                    dfaState = FindTransition(dfaState.Transitions, c);
                    if (dfaState.IsAccepting<DFAState<char>, char>())
                    {
                        lastAcceptingReaderState = sourceReader.MakeMemento();
                        lastAcceptedDfaState = dfaState;
                        succeed = true;
                    }
                }
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
                    break;
                }
            }
            return new TokenizerResult<TOrigin, TMemento, TLocation, TFragment>(result, lastAcceptedLocation);
        }

        public IEnumerable<Tuple<IFragment, IEnumerable<int>>> Process(IOrigin origin)
        {
            var result = Process<BareOrigin, BareLocation, BareLocation, BareFragment>(new BareOrigin(origin));
            return
                result.Tokens.Select(tuple => new Tuple<IFragment, IEnumerable<int>>(tuple.Item1.Fragment, tuple.Item2));
        }

        private IEnumerable<int> GetCategoriesFromState<T>(T dfaState) where T : IDfaState<T, char>
        {
            return new CategoryEnumerable(this, dfaState.Accepting);
        }

        private static T FindTransition<T>(KeyValuePair<char, T>[] transitions, char c) where T : IDfaState<T, char>
        {
            return Array.FindLast(transitions, pair => pair.Key <= c).Value;
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

        private struct ProductDfaBuilder<T, U> where T : IDfaState<T, char> where U : IDfaState<U, char>
        {
            private readonly Lexer _lexer;
            private readonly Dictionary<Tuple<T, U>, ProductDfaState> _productionMapping;

            public ProductDfaBuilder(Lexer lexer)
            {
                _lexer = lexer;
                _productionMapping = new Dictionary<Tuple<T, U>, ProductDfaState>();
            }

            public ProductDfa Build(T lastDfaStart, U newDfaStart)
            {
                return new ProductDfa {Start = GetProductState(lastDfaStart, newDfaStart)};
            }

            private ProductDfaState GetProductState(T lastDfaState, U newDfaState)
            {
                var statePack = new Tuple<T, U>(lastDfaState, newDfaState);
                if (_productionMapping.ContainsKey(statePack))
                {
                    return _productionMapping[statePack];
                }
                // Tworzę stub stanu, tak aby można było się do niego odnosić
                var productState = new ProductDfaState
                {
                    Accepting = GetProductAccepting(lastDfaState.Accepting, newDfaState.Accepting)
                };
                _productionMapping[statePack] = productState;
                // Przygotowuję przejścia
                var productTransitions = MakeProductTransitions(lastDfaState.Transitions, newDfaState.Transitions);
                // Zapisuję wygenerowane przejścia w stanie
                productState.Transitions = productTransitions.ToArray();
                // Stan jest gotowy
                return productState;
            }

            private List<KeyValuePair<char, ProductDfaState>> MakeProductTransitions(
                KeyValuePair<char, T>[] lastTransitions, KeyValuePair<char, U>[] newTransitions)
            {
                Debug.Assert(lastTransitions[0].Key == '\0');
                Debug.Assert(newTransitions[0].Key == '\0');
                int lastIndex = 0, newIndex = 0; // Zakładam że na pozycji 0 są znaki '\0'
                var productTransitions =
                    new List<KeyValuePair<char, ProductDfaState>>(Math.Max(lastTransitions.Length, newTransitions.Length));
                while (lastIndex < lastTransitions.Length && newIndex < newTransitions.Length)
                {
                    // Dodaj obecnie widoczne przejście
                    var c = lastTransitions[lastIndex].Key > newTransitions[newIndex].Key
                        ? lastTransitions[lastIndex].Key
                        : newTransitions[newIndex].Key;
                    productTransitions.Add(new KeyValuePair<char, ProductDfaState>(c,
                        GetProductState(lastTransitions[lastIndex].Value, newTransitions[newIndex].Value)));
                    // Szukaj granicy kolejnego przejścia
                    if (lastIndex == lastTransitions.Length - 1)
                    {
                        newIndex += 1;
                    }
                    else if (newIndex == newTransitions.Length - 1)
                    {
                        lastIndex += 1;
                    }
                    else
                    {
                        Debug.Assert(lastTransitions[lastIndex].Key < lastTransitions[lastIndex + 1].Key);
                        Debug.Assert(newTransitions[newIndex].Key < newTransitions[newIndex + 1].Key);
                        if (lastTransitions[lastIndex + 1].Key == newTransitions[newIndex + 1].Key)
                        {
                            lastIndex += 1;
                            newIndex += 1;
                        }
                        else if (lastTransitions[lastIndex + 1].Key < newTransitions[newIndex + 1].Key)
                        {
                            lastIndex += 1;
                            Debug.Assert(lastTransitions[lastIndex].Key > newTransitions[newIndex].Key);
                        }
                        else
                        {
                            newIndex += 1;
                            Debug.Assert(newTransitions[newIndex].Key > lastTransitions[lastIndex].Key);
                        }
                    }
                }
                return productTransitions;
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
                if (_lexer._compressionMapping.ContainsKey(pack))
                {
                    return _lexer._compressionMapping[pack];
                }
                var category = _lexer._nextCategory++;
                _lexer._compressionMapping[pack] = category;
                _lexer._decompressionMapping[category] = pack;
                return category;
            }
        }

        private class ProductDfaState : IDfaState<ProductDfaState, char>
        {
            public uint Accepting { get; internal set; }
            public KeyValuePair<char, ProductDfaState>[] Transitions { get; internal set; }

            internal static ProductDfaState MakeDeadState()
            {
                var result = new ProductDfaState {Accepting = 0};
                result.Transitions = new[] {new KeyValuePair<char, ProductDfaState>('\0', result)};
                return result;
            }
        }

        private struct ProductDfa : IDfa<ProductDfaState, char>
        {
            public ProductDfaState Start { get; internal set; }

            internal static ProductDfa MakeEmptyLanguageDfa()
            {
                return new ProductDfa {Start = ProductDfaState.MakeDeadState()};
            }
        }

        #region BareSourceWrappers

        private struct BareOrigin : IOrigin<BareOrigin, BareLocation, BareLocation, BareFragment>
        {
            private readonly IOrigin _origin;

            public BareOrigin(IOrigin origin)
            {
                _origin = origin;
            }

            public BareLocation begin
            {
                get { return new BareLocation(_origin.Begin); }
            }

            public IOriginReader<BareOrigin, BareLocation, BareLocation, BareFragment> GetReader()
            {
                return new BareOriginReader(_origin.GetReader());
            }

            public BareFragment MakeFragment(BareLocation from, BareLocation to)
            {
                return new BareFragment(_origin.MakeFragment(from._location, to._location));
            }
        }

        private struct BareOriginReader : IOriginReader<BareOrigin, BareLocation, BareLocation, BareFragment>
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

            public BareLocation MakeMemento()
            {
                return CurrentLocation;
            }

            public void Rollback(BareLocation memento)
            {
                _originReader.CurrentLocation = memento._location;
            }

            public bool MoveNext()
            {
                return _originReader.MoveNext();
            }
        }

        private struct BareLocation : ILocation<BareOrigin, BareLocation, BareLocation, BareFragment>
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

        private struct BareFragment : IFragment<BareOrigin, BareLocation, BareLocation, BareFragment>
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
    }
}