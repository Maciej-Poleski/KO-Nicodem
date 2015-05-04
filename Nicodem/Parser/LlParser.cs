using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Nicodem.Core;
using Nicodem.Source;

namespace Nicodem.Parser
{
	public class LlParser<TSymbol> : IParser<TSymbol> where TSymbol : struct, ISymbol<TSymbol>
	{ 
		private readonly Grammar<TSymbol> _grammar;

		public LlParser(Grammar<TSymbol> grammar)
		{
			if(grammar.HasLeftRecursion) {
                throw new ArgumentException("Grammar has left recursion");
			}
            _grammar = grammar;
		}

        // At this moment return null when parsing fails
        public IParseTree<TSymbol> Parse(IEnumerable<ParseLeaf<TSymbol>> word)
		{
            var memoizedWord = new MemoizedInput<ParseLeaf<TSymbol>>(word);
            var result = ParseTerm(_grammar.Start, memoizedWord, memoizedWord.Begin).First();

            // whole input has to be eaten
            if(result && result.Iterator == memoizedWord.End) {
                return result.Tree;
            } else {
                return null;
            }
		}

        // returns either a list of successfull parsed results or a singleton list with failure 
		private IEnumerable<ParseResult<TSymbol>> ParseTerm(TSymbol term, MemoizedInput<ParseLeaf<TSymbol>> word, MemoizedInput<ParseLeaf<TSymbol>>.Iterator iterator)
		{
			var dfa = _grammar.Automatons[term];
			// stack for backtracking - <position in word, current state of appropriate DFA>
			var st = new Stack<ParseState>(); 
			var children = new Stack<IParseTree<TSymbol>>(); 
            bool accepted = false;
			var eof = ParserUtils<TSymbol>.GetEOF();

            st.Push(new ParseState(dfa.Start, 0, iterator));

			while(st.Any()) {
				var parseState = st.Peek();
				var node = parseState.State;
				var it = parseState.Iterator;
                TSymbol currentSymbol = (it != word.End) ? it.Current.Symbol : eof;

                if(node.Accepting > 0 && (currentSymbol.Equals(eof) || _grammar.Follow[term].Contains(currentSymbol))) {
                    accepted = true;
					var parsedChildren = children.ToList();
					parsedChildren.Reverse();
					var parsedTree = new ParseBranch<TSymbol>(
						iterator != word.End ? GetFragmentRange(iterator.Current.Fragment, children.Peek().Fragment) : null,
						term, 
						_grammar.WhichProduction[node.Accepting], 
						parsedChildren);

					yield return new ParseResult<TSymbol>(parsedTree, it);
				}

				var trans = node.Transitions;
				var ind = parseState.TransitionIndex;
				for(; ind < trans.Count; ind++) {

                    if(_grammar.InFirstPlus(trans[ind].Key, currentSymbol)) {
						if(trans[ind].Key.IsTerminal) {

                            children.Push(new ParseLeaf<TSymbol>(it != word.End ? it.Current.Fragment : null, currentSymbol)); // TODO It would be better to have special END fragment
							st.Push(new ParseState(trans[ind].Value, 0, it != word.End ? it.Next() : word.End));
							break;
						} else {
							IEnumerator<ParseResult<TSymbol>> resultIt = ParseTerm(trans[ind].Key, word, it).GetEnumerator();
                            resultIt.MoveNext();
                            if(resultIt.Current) {
                                children.Push(resultIt.Current.Tree);
								st.Push(new ParseState(trans[ind].Value, 0, resultIt.Current.Iterator, resultIt));
                                break;
                            }
						}
					}
				}
				parseState.TransitionIndex = ind;

				// could not find next parsing transition
				if(ind >= trans.Count) { 
					Backtrack(st, children);
				}
			}

			if(accepted) {
                yield break;
			} else {
				yield return new ParseResult<TSymbol>(null, iterator, false);
			}
		}

        private static void Backtrack(Stack<ParseState> stack, Stack<IParseTree<TSymbol>> children)
		{
			Console.WriteLine("Backtrack");
            while(stack.Any()) {
                var state = stack.Pop();

				if(state.NextPossibleResult != null && state.NextPossibleResult.MoveNext()) {

					children.Pop();
					var nextRes = state.NextPossibleResult.Current;
					children.Push(nextRes.Tree);
					stack.Push(
						new ParseState(state.State, 
							0, 
							nextRes.Iterator, 
							state.NextPossibleResult));
					break;
				} else if(state.TransitionIndex + 1 < state.State.Transitions.Count) {

					stack.Push(new ParseState(state.State, state.TransitionIndex + 1, state.Iterator));
					break;
				} else if(children.Any()) {
	                children.Pop();
				}
            }
		}

		private static IFragment GetFragmentRange(IFragment begin, IFragment end) {
			if(begin == null || end == null) { // TODO handle it better
				return new OriginFragment(null, new OriginPosition(), new OriginPosition());
			}
			return new OriginFragment(begin.Origin, begin.GetBeginOriginPosition(), end.GetEndOriginPosition());
		}

		/* --- data types ----- */
		private class ParseState
		{
			public DfaState<TSymbol> State { get; private set; } 
			public int TransitionIndex { get; set; }
            public MemoizedInput<ParseLeaf<TSymbol>>.Iterator Iterator { get; private set; }
            // used when function may return multiple ok results during one call
			public IEnumerator<ParseResult<TSymbol>> NextPossibleResult { get; private set; } 

            public ParseState(DfaState<TSymbol> state, 
                int transitionIndex, 
				MemoizedInput<ParseLeaf<TSymbol>>.Iterator iterator,
				IEnumerator<ParseResult<TSymbol>> nextPossibleResult = null)
			{
				State = state;
				TransitionIndex = transitionIndex;
				Iterator = iterator;
                NextPossibleResult = nextPossibleResult;
			}
		}

	}
}
