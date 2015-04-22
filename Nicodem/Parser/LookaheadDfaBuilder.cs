using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nicodem.Parser
{
    public class LookaheadDfaBuilder<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
    {
        private class State : HashSet<LlConfiguration<TSymbol>> { }

        Dictionary<State, Dictionary<TSymbol, State>> states;
        private Grammar<TSymbol> grammar;

        public LookaheadDfaBuilder() { }

        private void AddEpsiEdges(State s)
        {
            var queue = new Queue<LlConfiguration<TSymbol>>();
            foreach (var conf in s) queue.Enqueue(conf);
            s.Clear();
            while (queue.Count > 0) {
                var conf = queue.Dequeue();
                if (!s.Add(conf)) continue;
                foreach (var next in grammar.OutgoingEpsiEdges(conf)) {
                    queue.Enqueue(next);
                }
            }
        }

        private void FilterSubsumedConfigs(State state)
        {
            var subsumedConfs = new HashSet<LlConfiguration<TSymbol>>();
            foreach (var conf1 in state) {
                foreach (var conf2 in state) {
                    if (conf1 == conf2) continue;
                    if (conf1.Subsumes(conf2)) subsumedConfs.Add(conf2);
                }
            }
            foreach (var subsumed in subsumedConfs) state.Remove(subsumed);
        }

        private List<TSymbol?> GetDecisions(State s) {
            var set = new HashSet<TSymbol?>(); // Ensure unique.
            foreach (var conf in s) {
                set.Add(conf.label);
            }
            return new List<TSymbol?>(set);
        }

        private bool IsAccepting(State state)
        {
            // TODO: what when there are no decisions
            // TODO: what about infinite many states blocker?
            return GetDecisions(state).Count < 2;
        }

        private void Dfs(State curState){
            if (states.ContainsKey(curState))
                return;

            if (IsAccepting(curState)) {
                return;
            }

            var nextStates = new Dictionary<TSymbol, State>();

            // TODO: DFA-Transitions representation? - intervals vs (all) edges
            // TODO: what about AlphabetClass and all TSymbol enumeration?
            foreach (var conf in curState) {
                foreach(KeyValuePair<TSymbol,LlConfiguration<TSymbol>> edge in grammar.OutgoingTerminalEdges(conf)) {
                    TSymbol symbol = edge.Key;
                    nextStates[symbol] = nextStates[symbol] ?? new State();
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

        private LookaheadDfa<TSymbol> BuildDfa(State startingState) {
            var dfaStates = new Dictionary<State, DfaState<TSymbol>>();
            foreach (var state in states.Keys) {
                dfaStates[state] = new DfaState<TSymbol>("TODO:id");
            }
            foreach (var state in states.Keys) {
                var edges = new List<KeyValuePair<TSymbol, DfaState<TSymbol>>>();
                foreach (var i in states[state].Keys) {
                    edges.Add(new KeyValuePair<TSymbol,DfaState<TSymbol>>(i, dfaStates[states[state][i]]));
                }
                dfaStates[state].Initialize(/* TODO */0, edges);
            }
            return new LookaheadDfa<TSymbol>(dfaStates[startingState], null /* TODO */);
        }

        public LookaheadDfa<TSymbol> Build(
            Grammar<TSymbol> grammar, 
            TSymbol initialSymbol, 
            DfaState<TSymbol> initialState)
        {
            states = new Dictionary<State,Dictionary<TSymbol,State>>();

            this.grammar = grammar;
            var startingSet = new State();
            foreach (var edge in initialState.Transitions) {
                TSymbol decision = edge.Key;
                var stack = ImmutableList.Create<DfaState<TSymbol>>();
                if (grammar.Automatons.ContainsKey(decision)) {
                    stack = stack.Add(edge.Value);
                    stack = stack.Add(grammar.Automatons[decision].Start);
                    startingSet.Add(new LlConfiguration<TSymbol>(decision, stack));
                } else {
                    stack = stack.Add(edge.Value);
                    startingSet.Add(new LlConfiguration<TSymbol>(decision, stack));
                }
            }
            AddEpsiEdges(startingSet);

            Dfs(startingSet);

            return BuildDfa(startingSet);
        }
    }
}

