using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nicodem.Source;

namespace Nicodem.Lexer
{
    public class Lexer
    {
        private readonly uint _atomicCategoryLimit; // i <= _atomicCategoryLimit => i nie wymaga dalszej dekompresji

        private readonly Dictionary<Tuple<uint, uint>, uint> _compressionMapping =
            new Dictionary<Tuple<uint, uint>, uint>();

        private readonly Dictionary<uint, Tuple<uint, uint>> _decompressionMapping =
            new Dictionary<uint, Tuple<uint, uint>>();

        private readonly DFA _dfa;
        private uint _nextCategory;

        public Lexer(RegEx[] regexCategories)
        {
            _atomicCategoryLimit = (uint) regexCategories.Length;
            _nextCategory = _atomicCategoryLimit + 1;
            if (regexCategories.Length == 0)
            {
                _dfa = ProductDfa.MakeEmptyLanguageDfa().Minimized<ProductDfa, ProductDfaState>();
                return;
            }
            var lastDfa = MakeRegexDfa(regexCategories[0], 1).Minimized<DFA, DFAState>();
            for (uint i = 1; i < regexCategories.Length; ++i)
            {
                lastDfa = MakeMinimizedProductDfa(lastDfa, MakeRegexDfa(regexCategories[i], i + 1));
            }
            _dfa = lastDfa;
        }

        private ProductDfa MakeProductDfa<T, TU, U, UU>(T lastDfa, U newDfa)
            where T : IDfa<TU>
            where TU : IDfaState<TU>
            where U : IDfa<UU>
            where UU : IDfaState<UU>
        {
            return new ProductDfaBuilder<TU, UU>(this).Build(lastDfa.Start, newDfa.Start);
        }

        private DFA MakeMinimizedProductDfa(DFA lastDfa, DFA newDfa)
        {
            return MakeProductDfa<DFA, DFAState, DFA, DFAState>(lastDfa, newDfa)
                .Minimized<ProductDfa, ProductDfaState>();
        }

        private static DFA MakeRegexDfa(RegEx regex, uint category)
        {
            return new DFA(regex, category);
        }

        /// <summary>
        ///     Może klient mógłby określić jaki chce typ enumeratora...
        /// </summary>
        /// <param name="sourceOrigin"></param>
        /// <returns>
        ///     Lista tokenów powiązanych z informacją o kategoriach do których dany token należy. Lista składa się z tokenów
        ///     najdłuższego prefiksu źródła, którego udało się sparsować.
        /// </returns>
        public TokenizerResult<TOrigin, TMemento, TLocation, TFragment> Process<TOrigin, TMemento, TLocation, TFragment>
            (
            TOrigin sourceOrigin)
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
                if (dfaState.IsAccepting())
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
                    if (dfaState.IsAccepting())
                    {
                        lastAcceptingReaderState = sourceReader.MakeMemento();
                        lastAcceptedDfaState = dfaState;
                    }
                }
                if (succeed)
                {
                    sourceReader.Rollback(lastAcceptingReaderState);
                    dfaState = lastAcceptedDfaState;
                    var currentLocation = sourceReader.CurrentLocation;
                    var currentFrame = currentLocation.Origin.MakeFragment(lastAcceptedLocation, currentLocation);
                    result.Add(new Tuple<TFragment, IEnumerable<int>>(currentFrame, getCategoriesFromState(dfaState)));
                    lastAcceptedLocation = currentLocation;
                }
                else
                {
                    break;
                }
            }
            return new TokenizerResult<TOrigin, TMemento, TLocation, TFragment>(result, lastAcceptedLocation);
        }

        private IEnumerable<int> getCategoriesFromState<T>(T dfaState) where T : IDfaState<T>
        {
            throw new NotImplementedException();
        }

        private T FindTransition<T>(KeyValuePair<char, T>[] transitions, char c) where T : IDfaState<T>
        {
            return Array.FindLast(transitions, pair => pair.Key <= c).Value;
        }

        private class ProductDfaBuilder<T, U> where T : IDfaState<T> where U : IDfaState<U>
        {
            private readonly Lexer _lexer;

            private readonly Dictionary<Tuple<T, U>, ProductDfaState> _productionMapping =
                new Dictionary<Tuple<T, U>, ProductDfaState>();

            public ProductDfaBuilder(Lexer lexer)
            {
                _lexer = lexer;
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
                var category = _lexer._nextCategory++;
                var pack = new Tuple<uint, uint>(lastDfaAccepting, newDfaAccepting);
                _lexer._compressionMapping[pack] = category;
                _lexer._decompressionMapping[category] = pack;
                return category;
            }
        }

        private class ProductDfaState : IDfaState<ProductDfaState>
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

        private struct ProductDfa : IDfa<ProductDfaState>
        {
            public ProductDfaState Start { get; internal set; }

            internal static ProductDfa MakeEmptyLanguageDfa()
            {
                return new ProductDfa {Start = ProductDfaState.MakeDeadState()};
            }
        }
    }
}