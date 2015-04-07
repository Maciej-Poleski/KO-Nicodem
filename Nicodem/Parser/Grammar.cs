using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	// TODO: This class should be immutable. Find a way to use IImmutableDictionary and IImmutableSet.
	public class Grammar<TSymbol> where TSymbol : class, ISymbol<TSymbol>
	{
		public TSymbol Start { get; private set; }
		public IDictionary<TSymbol, Production<TSymbol>[]> Productions { get; private set; } 
		internal IDictionary<TSymbol, Dfa<TSymbol>> Automatons { get; private set; }
		internal IDictionary<uint, Production<TSymbol>> WhichProduction { get; private set; } // accepting state marker -> production
		internal ISet<TSymbol> Nullable { get; private set; }
		internal IDictionary<TSymbol, ISet<TSymbol>> First { get; private set; }
		internal IDictionary<TSymbol, ISet<TSymbol>> Follow { get; private set; }
        internal bool HasLeftRecursion { get; private set; }
		// a dictionary which for each symbol stores a list of all the states s.t. there exists edge labelled by this symbol to the given state.
		internal IDictionary<TSymbol, List<DfaState<TSymbol>>> TargetStatesDictionary { get; private set; }
		// a dictionary which maps an accepting state to a symbol (nonterminal) whose DFA contains this state.
		internal IDictionary<DfaState<TSymbol>, TSymbol> AccStateOwnerDictionary { get; private set; }

        // returns true if word belongs to the FIRST+ set for given term
		internal bool InFirstPlus(TSymbol term, TSymbol word)
		{
			return First[term].Contains(word) || (Nullable.Contains(term) && Follow[term].Contains(word));
		}

		internal bool IsTerminal(TSymbol term)
		{
			return Productions[term].Length == 0;
		}

		public Grammar(IDictionary<TSymbol, Production<TSymbol>[]> productions)
		{
            Productions = productions;
			WhichProduction = new Dictionary<uint, Production<TSymbol>>();
			Automatons = new Dictionary<TSymbol, Dfa<TSymbol>>();
			// Here we prepare automatons for each symbol.
			uint productionMarker = 1;
			foreach (var symbolProductions in Productions)
			{
				var automatons = new List<Lexer.DfaUtils.MinimizedDfa<TSymbol>>();
				foreach (var production in symbolProductions.Value)
				{
					automatons.Add(Dfa<TSymbol>.RegexDfa(production.Rhs, productionMarker));
                    WhichProduction[productionMarker] = production;
					productionMarker++;
				}
				Automatons[symbolProductions.Key] = Dfa<TSymbol>.ProductDfa(automatons.ToArray());
			}

			Nullable = GrammarUtils<TSymbol>.ComputeNullable (Automatons);
			First = GrammarUtils<TSymbol>.ComputeFirst (Automatons, Nullable);
			Follow = GrammarUtils<TSymbol>.ComputeFollow (Automatons, Nullable, First);
			HasLeftRecursion = GrammarUtils<TSymbol>.HasLeftRecursion(Automatons, Nullable);
			TargetStatesDictionary = GrammarUtils<TSymbol>.computeTargetStatesDictionary (Automatons);
			AccStateOwnerDictionary = GrammarUtils<TSymbol>.computeAccStateOwnerDictionary (Automatons);
		}

		// for a given LlConfiguration the function computes list of edges to all other possible LlConfigurations.
		public List<KeyValuePair<TSymbol,LlConfiguration<TSymbol>>> OutgoingEdges(LlConfiguration<TSymbol> llconf) {
			if (llconf.Count() == 0) {
				// If intially the stack is empty we cannot figure out next edges, so return an empty list.
				return new List<KeyValuePair<TSymbol, LlConfiguration<TSymbol>>> ();
			}

			var topState = llconf.Pop ();
			var transitions = topState.Transitions;
			var result = new List<KeyValuePair<TSymbol,LlConfiguration<TSymbol>>>();

			foreach (var kv in transitions) {
				var symbol = kv.Key;
				var targetState = kv.Value;
				var newStack = llconf.copyOfStack (); // shallow copy of the stack
				TSymbol newEdgeSymbol = null; // symbol on the edge to new LlConfiguration, epsilon == null

				// topState---symbol-->targetState (we pop topState and push targetState)
				if (IsTerminal (symbol)) {
					newEdgeSymbol = symbol;
					newStack.Push (targetState);
				} else {
					newStack.Push (targetState);
					newStack.Push (Automatons [symbol].Start); //we also push start state of the symbol's automaton.
				}

				var newLlConf = new LlConfiguration<TSymbol> (llconf.label, newStack);
				var newEdge = new KeyValuePair<TSymbol, LlConfiguration<TSymbol>> (newEdgeSymbol, newLlConf);
				result.Add (newEdge);
			}

			if (topState.Accepting > 0) {
				// We also add a separate epsilon-edge for the situation in which
				// top state is accepting state for some nonterminal's automaton and
				// we don't move forward but just pop the top state from the stack.
				// then we consider two cases: when the stack is empty and when it's not. 
				if (llconf.Count () == 0) {
					// prevSymbol is a nonterminal whose DFA contains topState (and topState is accepting)
					var prevSymbol = AccStateOwnerDictionary [topState];
					// browse the list of target states for the prevSymbol and for each of them create
					// a separate configuration with the stack containing the given state.
					foreach (var targetState in TargetStatesDictionary[prevSymbol]) {
						var newStack = new Stack<DfaState<TSymbol>> ();
						newStack.Push (targetState);
						var newLlConf = new LlConfiguration<TSymbol> (llconf.label, newStack);
						var newEdge = new KeyValuePair<TSymbol, LlConfiguration<TSymbol>> (null, newLlConf);
						result.Add (newEdge);
					}
				} else {
					var newStack = llconf.copyOfStack (); // shallow copy of the stack
					var newLlConf = new LlConfiguration<TSymbol> (llconf.label, newStack);
					var newEdge = new KeyValuePair<TSymbol, LlConfiguration<TSymbol>> (null, newLlConf);
					result.Add (newEdge);
				}
			}
			return result;
		}

	}
}

