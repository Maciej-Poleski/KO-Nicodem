using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
    public class FirstSet<TSymbol> where TSymbol : ISymbol<TSymbol>
    {
        private IDictionary<TSymbol, ISet<TSymbol>> first = new Dictionary<TSymbol, ISet<TSymbol>>();

        public FirstSet(IDictionary<TSymbol, ISet<TSymbol>> _first) {
            this.first = _first;
        }

        public FirstSet(IDictionary<TSymbol, Dfa<TSymbol>> automatons, ISet<TSymbol> nullable)
        {
            // begin with FIRST(A) := { A } foreach A
            foreach(var A in automatons.Keys) {
                this[A] = new HashSet<TSymbol> ();
                this[A].Add (A);
            }

            // foreach production A -> E
            // where A - symbol, E - automata
            foreach(var symbol in automatons.Keys) {
                var automata = automatons [symbol];

                // perform BFS from automata startstate
                // using only edges labeled by symbols from nullable set
                var Q = new Queue<DfaState<TSymbol>>();
                var visited = new HashSet<DfaState<TSymbol>> ();

                Q.Enqueue (automata.Start);
                visited.Add (automata.Start);

                while(Q.Count > 0) {
                    var state = Q.Dequeue();
                    // check all edges
                    foreach(var transition in state.Transitions) {
                        // add label to FIRST(symbol)
                        this[symbol].Add (transition.Key);
                        // if label is in NULLABLE set continue BFS using this edge
                        if (nullable.Contains (transition.Key) && !visited.Contains (transition.Value)) {
                            Q.Enqueue (transition.Value);
                            visited.Add (transition.Value);
                        }
                    }
                }
            }
            // compute transitive complement:
            // B \in D(A) => D(B) <= D(A)
            var change = true;
            while(change) {
                change = false;
                // diff between before-phase and after-phase sets
                var diff = new Dictionary<TSymbol, ISet<TSymbol>> ();
                foreach(var A in first.Keys) {
                    diff [A] = new HashSet<TSymbol> ();
                    foreach(var B in this[A]) {
                        foreach (var x in this[B]) {
                            change |= !(this[A].Contains (x) || diff [A].Contains (x));
                            diff [A].Add (x);
                        }
                    }
                }
                // update set
                foreach (var A in first.Keys)
                    foreach (var x in diff[A])
                        this[A].Add (x);
            }
        }

        public ISet<TSymbol> this[TSymbol t] {
            get{
                if (first.ContainsKey(t)) {
                    return first[t];
                } else {
                    return new HashSet<TSymbol>{t};
                }
            }
            private set{
                first[t] = value;
            }
        }
    }
}

