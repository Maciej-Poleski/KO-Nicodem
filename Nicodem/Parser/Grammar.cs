using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	// TODO: This class should be immutable. Find a way to use IImmutableDictionary and IImmutableSet.
	public class Grammar<TProduction> where TProduction:IProduction
	{
		public ISymbol Start { get; private set; }
		public IDictionary<ISymbol, TProduction[]> Productions { get; private set; } 
		internal IDictionary<ISymbol, Dfa<ISymbol>> Automatons { get; private set; }
		internal IDictionary<uint, TProduction> WhichProduction { get; private set; } // accepting state marker -> production
		internal ISet<ISymbol> Nullable { get; private set; }
		internal IDictionary<ISymbol, ISet<ISymbol>> First { get; private set; }
		internal IDictionary<ISymbol, ISet<ISymbol>> Follow { get; private set; }
        internal bool HasLeftRecursion { get; private set; }
		// for each symbol it has a list of all the states s.t. there exists edge labelled by this symbol to the given state.
		internal IDictionary<ISymbol, List<DfaState<ISymbol>>> TargetStatesDictionary { get; private set; }
		
        // returns true if word belongs to the FIRST+ set for given term
		internal bool InFirstPlus(ISymbol term, ISymbol word)
		{
			return First[term].Contains(word) || (Nullable.Contains(term) && Follow[term].Contains(word));
		}

		internal bool IsTerminal(ISymbol term)
		{
			return Productions[term].Length == 0;
		}

        public Grammar(IDictionary<ISymbol, TProduction[]> productions)
		{
            Productions = productions;
			// Here we prepare automatons for each symbol.
			uint productionMarker = 1;
			foreach (var symbolProductions in Productions)
			{
				var automatons = new List<Lexer.DfaUtils.MinimizedDfa<ISymbol>>();
				foreach (var production in symbolProductions.Value)
				{
					automatons.Add(Dfa<ISymbol>.RegexDfa(production.Rhs, productionMarker));
                    WhichProduction[productionMarker] = production;
					productionMarker++;
				}
				Automatons[symbolProductions.Key] = Dfa<ISymbol>.ProductDfa(automatons.ToArray());
			}

			// set Nullable, First and Follow
			Nullable = GrammarUtils.ComputeNullable (Automatons);
			First = GrammarUtils.ComputeFirst (Automatons, Nullable);
			Follow = GrammarUtils.ComputeFollow (Automatons, Nullable, First);
            HasLeftRecursion = GrammarUtils.HasLeftRecursion(Automatons, Nullable);
			TargetStatesDictionary = GrammarUtils.computeTargetStatesDictionary (Automatons);
		}

		// for a given LlConfiguration the function computes list of edges to all other possible LlConfigurations.
		public List<KeyValuePair<ISymbol,LlConfiguration<ISymbol>>> OutgoingEdges(LlConfiguration<ISymbol> llconf) {
			if (llconf.Count() == 0) {
				// If intially the stack is empty we cannot figure out next edges.
				return new List<KeyValuePair<ISymbol, LlConfiguration<ISymbol>>> ();
			}

			var topState = llconf.Pop ();
			var transitions = topState.Transitions;
			var result = new List<KeyValuePair<ISymbol,LlConfiguration<ISymbol>>>();

			foreach (var kv in transitions) {
				var symbol = kv.Key;
				var targetState = kv.Value;
				var newStack = llconf.copyOfStack (); // shallow copy of the stack
				ISymbol newEdgeSymbol = null; // symbol on the edge to new LlConfiguration, epsilon == null

				// topState---symbol-->targetState (we pop topState and push targetState)
				if (IsTerminal (symbol)) {
					newEdgeSymbol = symbol;
					newStack.Push (targetState);
				} else {
					newStack.Push (targetState);
					newStack.Push (Automatons [symbol].Start); //we also push start state of the symbol's automaton.
				}

				var newLlConf = new LlConfiguration<ISymbol> (llconf.label, newStack);
				var newEdge = new KeyValuePair<ISymbol, LlConfiguration<ISymbol>> (newEdgeSymbol, newLlConf);
				result.Add (newEdge);
			}

			// We also add a separate epsilon-edge for the situation in which
			// we don't move forward but just pop the top state from the stack.
			if (llconf.Count() == 0) {
				// the stack is empty, so the last state on the stack was a start state of some symbol's automaton.
				ISymbol initialSymbol = null;
				foreach (var symbol in Automatons.Keys) {
					if (Automatons [symbol].Start == topState) {
						initialSymbol = symbol;
						break;
					}
				}
				if (initialSymbol == null) {
					//TODO is it possible ?
				}
				// browse the list of target states for the initialSymbol and for each of them create
				// a separate configuration with the stack containing the given state.
				foreach (var targetState in TargetStatesDictionary[initialSymbol]) {
					var newStack = new Stack<DfaState<ISymbol>> ();
					newStack.Push (targetState);
					var newLlConf = new LlConfiguration<ISymbol> (llconf.label, newStack);
					var newEdge = new KeyValuePair<ISymbol, LlConfiguration<ISymbol>> (null, newLlConf);
					result.Add (newEdge);
				}
			} else {
				var newStack = llconf.copyOfStack (); // shallow copy of the stack
				var newLlConf = new LlConfiguration<ISymbol> (llconf.label, newStack);
				var newEdge = new KeyValuePair<ISymbol, LlConfiguration<ISymbol>> (null, newLlConf);
				result.Add (newEdge);
			}

			return result;
		}

	}
}

