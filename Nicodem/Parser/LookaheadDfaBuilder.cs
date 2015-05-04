using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Nicodem.Parser
{
    public class LookaheadDfaBuilder<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
    {
        /// <summary>Class representing state of LookaheadDFA - set of LLConfigurations.</summary>
        private class State : HashSet<LlConfiguration<TSymbol>> { 
            static int nextId = 0;
            int stateId;
            public State() : base() {
                stateId = nextId++;
                Console.WriteLine("state " + stateId);
            }
            public override String ToString(){
                return "state " + stateId + ": " + string.Join(", ", this);
            }
        }

        // ----------- fields -----------

        /// <summary>
        /// Dictionary with transitions of currently built LookaheadDFA. From one State you can go
        /// with several edges - TSymbols, each leading you to a new State.
        /// </summary>
        Dictionary<State, Dictionary<TSymbol, State>> states;
        /// <summary>Currently used grammar.</summary>
        private Grammar<TSymbol> grammar;

        // ----------- constructor -----------

        public LookaheadDfaBuilder() { }

        // ----------- private methods -----------

        private void AddEpsiEdges(State s)
        {
            Console.WriteLine("AddEpsi for " + s); // DEBUG
            var queue = new Queue<LlConfiguration<TSymbol>>();
            foreach (var conf in s) queue.Enqueue(conf);
            s.Clear();
            while (queue.Count > 0) { // expand states in BFS manner
                var conf = queue.Dequeue();
                if (!s.Add(conf)) continue;
                foreach (var next in grammar.OutgoingEpsiEdges(conf)) {
                    queue.Enqueue(next);
                }
            }
            Console.WriteLine("DONE --> " + s); // DEBUG
        }

        private void FilterSubsumedConfigs(State state)
        {
            Console.WriteLine("FilterSubsumedConfigs for " + state); // DEBUG
            var subsumedConfs = new HashSet<LlConfiguration<TSymbol>>();
            foreach (var conf1 in state) {
                foreach (var conf2 in state) {
                    if (conf1 == conf2) continue;
                    if (conf1.Subsumes(conf2)) subsumedConfs.Add(conf2);
                }
            }
            foreach (var subsumed in subsumedConfs) state.Remove(subsumed);
            Console.WriteLine("DONE --> " + state); // DEBUG
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

        /// <summary>
        /// Recursively builds lookaheadDFA. Check what states are available from this state and if
        /// they are not already built - call recursively.
        /// </summary>
        private void RecursiveDfsBuild(State curState){
            if (states.ContainsKey(curState)) // state already built
                return;

            if (IsAccepting(curState)) { // accepting state, nothing to do here
                return;
            }

            var nextStates = new Dictionary<TSymbol, State>(); // transitions for this state

            // TODO: DFA-Transitions representation? - intervals vs (all) edges
            // TODO: what about AlphabetClass and all TSymbol enumeration?
            foreach (var conf in curState) { // go through llconfigurations present in this state
                foreach(KeyValuePair<TSymbol,LlConfiguration<TSymbol>> edge in grammar.OutgoingTerminalEdges(conf)) {
                    TSymbol symbol = edge.Key;
                    if (!nextStates.ContainsKey(symbol)) nextStates[symbol] = new State();
                    nextStates[symbol].Add(edge.Value);
                }
            }
            foreach (var nState in nextStates.Values) {
                AddEpsiEdges(nState);
                FilterSubsumedConfigs(nState);
            }
            states[curState] = nextStates;
            foreach (var nState in nextStates.Values) {
                RecursiveDfsBuild(nState);
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

        // ----------- public methods -----------

        /// <summary>
        /// Build the lookahead DFA for the given symbol and the given state from its productions DFA. Aim for
        /// this LookaheadDFA is to give parser an answer - which edge to choose in symbol DFA during parsing.
        /// </summary>
        public LookaheadDfa<TSymbol> Build(
            Grammar<TSymbol> grammar, 
            TSymbol initialSymbol, 
            DfaState<TSymbol> initialState)
        {
            // initialize
            states = new Dictionary<State,Dictionary<TSymbol,State>>();
            this.grammar = grammar;
            // create starting set
            var startingSet = new State();
            foreach (var edge in initialState.Transitions) { // for every decision (edge) add starting llconfig
                TSymbol decision = edge.Key; // label of DFA edge - possible decision
                var stack = ImmutableList.Create<DfaState<TSymbol>>(); // empty stack: '?'
                if (grammar.Automatons.ContainsKey(decision)) { // non-terminal
                    stack = stack.Add(edge.Value); // add target - state in which you would be if you go with this edge
                    stack = stack.Add(grammar.Automatons[decision].Start); // enter this symbol automaton
                    startingSet.Add(new LlConfiguration<TSymbol>(decision, stack));
                } else { // terminal - no automaton
                    stack = stack.Add(edge.Value);
                    startingSet.Add(new LlConfiguration<TSymbol>(decision, stack));
                }
            }
            Console.WriteLine("STARTING STATE -> " + startingSet); // DEBUG
            // complement starting state by epsi edges
            AddEpsiEdges(startingSet);
            // expand starting state to the whole LookaheadDFA
            RecursiveDfsBuild(startingSet);
            // convert result to LookaheadDfa class
            return BuildDfa(startingSet);
        }
    }
}

