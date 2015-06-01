using Nicodem.Lexer;
using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	// TODO: This class should be immutable. Find a way to use IImmutableDictionary and IImmutableSet.
	public class Grammar<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
	{
		public TSymbol Start { get; private set; }
		public IDictionary<TSymbol, IProduction<TSymbol>[]> Productions { get; private set; } 
		public IDictionary<TSymbol, Dfa<TSymbol>> Automatons { get; private set; }
		internal IDictionary<uint, IProduction<TSymbol>> WhichProduction { get; private set; } // accepting state marker -> production
		internal ISet<TSymbol> Nullable { get; private set; }
		internal IDictionary<TSymbol, ISet<TSymbol>> _First { get; private set; }
        internal FirstSet<TSymbol> First { get; private set; }
        internal IDictionary<TSymbol, ISet<TSymbol>> Follow { get; private set; }
        internal bool HasLeftRecursion {
            get {
                if (SymbolListLeftRecursion == null)
                    return false;
                else
                    return true;
            }
        }
        internal List<TSymbol> SymbolListLeftRecursion { get; private set; }
		// a dictionary which for a given symbol stores a list of all the states s.t. there exists edge labelled by this symbol to the given state.
		// if a symbol does not occur on any edge, it isn't in the dictionary.
		internal IDictionary<TSymbol, List<DfaState<TSymbol>>> TargetStatesDictionary { get; private set; }
		// a dictionary which maps an accepting state to a symbol (nonterminal) whose DFA contains this state.
		internal IDictionary<DfaState<TSymbol>, TSymbol> AccStateOwnerDictionary { get; private set; }

        // returns true if word belongs to the FIRST+ set for given term
		internal bool InFirstPlus(TSymbol term, TSymbol word)
		{
			/*if(!First.ContainsKey (term)){
				return false;
			} else*/
            if(First[term].Contains(word)) {
				return true;
			} else {
				return Nullable.Contains(term) && Follow.ContainsKey(term) && Follow[term].Contains(word);
			}
		}

		internal bool IsTerminal(TSymbol term)
		{
			return !(Productions.ContainsKey(term)) || Productions[term].Length == 0;
		}

        // For testing purposes only!
        public void InjectAutomatons(Dictionary<TSymbol, Dfa<TSymbol>> automatons)
        {
            this.Automatons = automatons;
            TargetStatesDictionary = GrammarUtils<TSymbol>.computeTargetStatesDictionary(Automatons);
            AccStateOwnerDictionary = GrammarUtils<TSymbol>.computeAccStateOwnerDictionary(Automatons);
        }

		public Grammar(TSymbol start, IDictionary<TSymbol, IProduction<TSymbol>[]> productions)
		{
			Start = start;
            Productions = productions;
			WhichProduction = new Dictionary<uint, IProduction<TSymbol>>();
			Automatons = new Dictionary<TSymbol, Dfa<TSymbol>>();
			// Here we prepare automatons for each symbol.
			uint productionMarker = 1;
			foreach (var symbolProductions in Productions)
			{
				var automatons = new List<IDfa<TSymbol>>();
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
            SymbolListLeftRecursion = GrammarUtils<TSymbol>.HasLeftRecursion(Automatons, Nullable);
			TargetStatesDictionary = GrammarUtils<TSymbol>.computeTargetStatesDictionary (Automatons);
			AccStateOwnerDictionary = GrammarUtils<TSymbol>.computeAccStateOwnerDictionary (Automatons);
        }
            
        public List<LlConfiguration<TSymbol>> OutgoingEpsiEdges(LlConfiguration<TSymbol> llconf)
        {
            if (llconf.Count() == 0) {
                // If intially the stack is empty we cannot figure out next edges, so return an empty list.
                return new List<LlConfiguration<TSymbol>> ();
            }

            var topState = llconf.stack[llconf.stack.Count-1];
            var transitions = topState.Transitions;
            var result = new List<LlConfiguration<TSymbol>>();
            foreach (var kv in transitions) {
                var symbol = kv.Key;
                var targetState = kv.Value;
                var newllconf = llconf.Pop(); // a copy without top state
                if (!IsTerminal(symbol)) {
                    newllconf = newllconf.Push(targetState);
                    newllconf = newllconf.Push(Automatons[symbol].Start);
                    result.Add(newllconf);
                }
            }

            if (topState.Accepting > 0) {
                // We also add a separate epsilon-edge for the situation in which
                // top state is accepting state for some nonterminal's automaton and
                // we don't move forward but just pop the top state from the stack.
                // then we consider two cases: when the stack is empty and when it's not.
                if (llconf.Count () == 1) {
                    // prevSymbol is a nonterminal whose DFA contains topState (and topState is accepting)
                    var prevSymbol = AccStateOwnerDictionary [topState];
                    var newllconf = llconf.Pop(); // a copy without top state
                    // browse the list of target states for the prevSymbol and for each of them create
                    // a separate configuration with the stack containing the given state.
					if (TargetStatesDictionary.ContainsKey (prevSymbol)) {
						foreach (var targetState in TargetStatesDictionary[prevSymbol]) {
							result.Add (newllconf.Push (targetState));
						}
					}
                } else {
                    result.Add (llconf.Pop());
                }
            }

            return result;
        }
			
        // Indexed by beginnings of terminal ranges.
        public List<KeyValuePair<TSymbol,LlConfiguration<TSymbol>>> OutgoingTerminalEdges(LlConfiguration<TSymbol> llconf)
        {
			var topState = llconf.stack[llconf.stack.Count-1];
			var transitions = topState.Transitions;
			var resultDict = new SortedDictionary<TSymbol, LlConfiguration<TSymbol>> ();
			foreach (var kv in transitions) {
				var symbol = kv.Key;
				var targetState = kv.Value;
				// topState---symbol-->targetState (we pop topState and push targetState)
				if (IsTerminal (symbol)) {
					var newllconf = llconf.Pop(); // a copy without top state
					newllconf = newllconf.Push(targetState);
					resultDict.Add (symbol, newllconf);
				}
			}
			var result = new List<KeyValuePair<TSymbol,LlConfiguration<TSymbol>>>();
			foreach (var kv in resultDict) {
				result.Add (kv);
			}
			return result;
        }
	}
}

