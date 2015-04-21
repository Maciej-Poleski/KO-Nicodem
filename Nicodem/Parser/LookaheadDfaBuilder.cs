using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
    public class LookaheadDfaBuilder<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
    {
        Dictionary<HashSet<LlConfiguration<TSymbol>>,
        Dictionary<TSymbol,HashSet<LlConfiguration<TSymbol>>>> states;
        private Grammar<TSymbol> grammar;
        private IDictionary<TSymbol, Dfa<TSymbol>> automatons; 

        public LookaheadDfaBuilder()
        {
        }

        private void AddEpsiEdges(HashSet<LlConfiguration<TSymbol>> s)
        {
        }

        private void FilterSubsumedConfigs(HashSet<LlConfiguration<TSymbol>> s)
        {
        }

        private List<TSymbol?> GetDecisions(HashSet<LlConfiguration<TSymbol>> s) {
            throw new NotImplementedException();
        }

        private void Dfs(HashSet<LlConfiguration<TSymbol>> curState){
            if (states.ContainsKey(curState))
                return;
            // TODO: what when there are no decisions
            // TODO: what about infinite many states blocker?
            if (GetDecisions(curState).Count < 2)
                return;

            var nextStates = new Dictionary<TSymbol,HashSet<LlConfiguration<TSymbol>>>();

            // TODO: DFA-Transitions representation? - intervals vs (all) edges
            // TODO: what about AlphabetClass and all TSymbol enumeration?
            foreach (var conf in curState) {
                foreach(KeyValuePair<TSymbol,LlConfiguration<TSymbol>> edge in grammar.OutgoingTerminalEdges(conf)) {
                    TSymbol symbol = edge.Key;
                    nextStates[symbol] = nextStates[symbol] ?? new HashSet<LlConfiguration<TSymbol>>();
                    nextStates[symbol].Add(edge.Value);
                }
            }
            foreach (var nState in nextStates.Values) {
                AddEpsiEdges(nState);
                FilterSubsumedConfigs(nState);
            }
            states[curState] = nextStates;
            foreach (var nState in nextStates.Values) {
                Dfs(nState);
            }
        }

        private LookaheadDfa<TSymbol> BuildDfa(){
            throw new NotImplementedException();
        }

        // Build requires Automatons explicitly, because at the moment of writing this code
        // grammar.Automatons does not work.

        public LookaheadDfa<TSymbol> Build(
            Grammar<TSymbol> grammar,
            TSymbol initialSymbol,
            DfaState<TSymbol> initialState)
        {
            return Build(grammar, grammar.Automatons, initialSymbol, initialState);
        }

        public LookaheadDfa<TSymbol> Build(
            Grammar<TSymbol> grammar, 
            IDictionary<TSymbol, Dfa<TSymbol>> automatons,
            TSymbol initialSymbol, 
            DfaState<TSymbol> initialState)
        {
            states = new Dictionary<HashSet<LlConfiguration<TSymbol>>,Dictionary<TSymbol,HashSet<LlConfiguration<TSymbol>>>>();

            this.grammar = grammar;
            this.automatons = automatons;
            var startingSet = new HashSet<LlConfiguration<TSymbol>>();
            foreach (var edge in initialState.Transitions) {
                TSymbol decision = edge.Key;
                var conf = new LlConfiguration<TSymbol>(decision);
                conf.Push(edge.Value);
                conf.Push(automatons[decision].Start);
                startingSet.Add(conf);
            }
            AddEpsiEdges(startingSet);

            Dfs(startingSet);

            return BuildDfa();
        }
    }
}

