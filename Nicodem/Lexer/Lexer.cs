using System;
using System.Collections.Generic;
using Nicodem.Source;

namespace Nicodem.Lexer
{
    public class Lexer
    {
        private DFA _dfa;

        public Lexer(RegEx[] regexCategories)
        {
            if (regexCategories.Length == 0)
            {
                _dfa = ProductDfa.MakeEmptyLanguageDfa().Minimized<ProductDfa,ProductDfaState>();
                return;
            }
            var lastDfa = MakeRegexDfa(regexCategories[0], 1).Minimized<DFA, DFAState>();
            for (uint i = 1; i < regexCategories.Length; ++i)
            {
                lastDfa = MakeMinimizedProductDfa(lastDfa, MakeRegexDfa(regexCategories[i], i + 1));
            }
            _dfa = lastDfa;
        }

        private static DFA MakeMinimizedProductDfa(DFA lastDfa, DFA newDfa)
        {
            throw new NotImplementedException();
        }

        private static DFA MakeRegexDfa(RegEx regex, uint category)
        {
            return new DFA(regex, category);
        }

        private ProductDfa MakeEmptyLanguageDfa()
        {
            return new ProductDfa();
        }

        /// <summary>
        /// Może klient mógłby określić jaki chce typ enumeratora...
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<Tuple<Fragment, IEnumerable<int>>> Process(IOrigin source)
        {
            throw new NotImplementedException();
        }
    }

    internal class ProductDfaState : IDfaState<ProductDfaState>
    {
        public uint Accepting { get; private set; }
        public KeyValuePair<char, ProductDfaState>[] Transitions { get; private set; }

        internal static ProductDfaState MakeDeadState()
        {
            var result = new ProductDfaState {Accepting = 0};
            result.Transitions = new[]{new KeyValuePair<char, ProductDfaState>('\0',result) };
            return result;
        }
    }

    internal struct ProductDfa : IDfa<ProductDfaState>
    {
        public ProductDfaState Start { get; private set; }

        internal static ProductDfa MakeEmptyLanguageDfa()
        {
            return new ProductDfa {Start = ProductDfaState.MakeDeadState()};
        }
    }
}