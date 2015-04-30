using System;
using Nicodem.Lexer;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nicodem.Parser
{
    public class LookaheadDfa<TSymbol> : Dfa<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
    {
        // Decisions[AcceptingState.Accepting] is the result of lookahead.
        public ReadOnlyDictionary<uint, IEnumerable<TSymbol?>> Decisions { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nicodem.Parser.LookaheadDfa`1"/> class.
        /// </summary>
        /// <param name="start">Starting state of this lookahead DFA.</param>
        /// <param name="decisions">Decisions connected with accepting states.</param>
        public LookaheadDfa(DfaState<TSymbol> start,
                            ReadOnlyDictionary<uint, IEnumerable<TSymbol?>> decisions) : base(start)
        {
            Decisions = decisions;
        }
    }
}

