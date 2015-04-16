using System;
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
        public IParseTree<TSymbol> Parse(IEnumerable<IParseTree<TSymbol>> word)
		{
			var memoizedWord = new MemoizedInput<IParseTree<TSymbol>>(word);
            var result = ParseTerm(_grammar.Start, memoizedWord, memoizedWord.Begin).ElementAt(0);

            // whole input has to be eaten
            if(result && result.Iterator == memoizedWord.End) {
                return result.Tree;
            } else {
                return null;
            }
		}

        // returns either a list of successfull parsed results or a singleton list with failure 
		private IEnumerable<ParseResult<TSymbol>> ParseTerm(TSymbol term, MemoizedInput<IParseTree<TSymbol>> word, MemoizedInput<IParseTree<TSymbol>>.Iterator input)
		{
			var dfa = _grammar.Automatons[term];
			// stack for backtracking - <position in word, current state of appropriate DFA>
			var st = new Stack<ParseState>(); 
			var children = new Stack<IParseTree<TSymbol>>(); 
            bool accepted = false;
			var eof = ParserUtils<TSymbol>.GetEOF();

			// the furthest in terms of position in the word parsed paths along with productions
			var furthestParsed = new ParseScope(new List<IParseTree<TSymbol>>(), input); // maybe used in the future for error handling

			st.Push(new ParseState(dfa.Start, 0, input));

			while(st.Any()) {
				var parseState = st.Peek();
				var node = parseState.State;
				var it = parseState.Iterator;
                TSymbol currentSymbol = (it != word.End) ? it.Current.Symbol : eof;

                if(node.Accepting > 0 && _grammar.Follow[term].Contains(currentSymbol)) {
                    accepted = true;
					var parsedChildren = children.ToList();
					parsedChildren.Reverse();
					var parsedTree = new ParseBranch<TSymbol>(
						GetFragmentRange(input.Current.Fragment, children.Peek().Fragment),
						term, 
						_grammar.WhichProduction[node.Accepting], 
						parsedChildren);

					yield return new ParseResult<TSymbol>(parsedTree, it);
				}

				var trans = node.Transitions;
				var ind = parseState.TransitionIndex;
				for(; ind < trans.Count; ind++) {

                    if(_grammar.InFirstPlus(trans[ind].Key, currentSymbol)) {
						if(_grammar.IsTerminal(trans[ind].Key)) {

							children.Push(new ParseLeaf<TSymbol>(it.Current.Fragment, currentSymbol));
							st.Push(new ParseState(trans[ind].Value, 0, it.Next()));
							break;
						} else {
							IEnumerator<ParseResult<TSymbol>> resultIt = ParseTerm(trans[ind].Key, word, it).GetEnumerator();
                            resultIt.MoveNext();
                            if(resultIt.Current) { // TODO else and look at the furthest parsed
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
					if(st.Peek().Iterator > furthestParsed.Iterator) {
						var newFurthestParsed = children.ToList();
						newFurthestParsed.Reverse();
						furthestParsed = new ParseScope(newFurthestParsed, st.Peek().Iterator);
					}
					Backtrack(st, children);
				}
			}

			if(accepted) {
                yield break;
			} else {
				var branch = new ParseBranch<TSymbol>(
					input != word.End ? GetFragmentRange(input.Current.Fragment, furthestParsed.Children.Last().Fragment) : null, 
						term, 
						_grammar.Productions[term][0],  // TODO could not parse any productions
						furthestParsed.Children);
				yield return new ParseResult<TSymbol>(branch, furthestParsed.Iterator, false);
			}
		}

        private static void Backtrack(Stack<ParseState> stack, Stack<IParseTree<TSymbol>> children)
		{
            while(stack.Any()) {
                var state = stack.Pop();
                if(children.Any()) {
                    children.Pop();
                }

                if(state.NextPossibleResult != null && state.NextPossibleResult.MoveNext()) {

                    var nextRes = state.NextPossibleResult.Current;
                    children.Push(nextRes.Tree);
                    stack.Push(new ParseState(state.State, state.TransitionIndex, nextRes.Iterator, state.NextPossibleResult));
                    return;
                } else if(state.TransitionIndex + 1 < state.State.Transitions.Count) {

                    stack.Push(new ParseState(state.State, state.TransitionIndex + 1, state.Iterator));
                    return;
                }

            }
		}

		private static IFragment GetFragmentRange(IFragment begin, IFragment end) {
			return new OriginFragment(begin.Origin, begin.GetBeginOriginPosition(), end.GetEndOriginPosition());
		}

		/* --- data types ----- */
		private class ParseState
		{
			public DfaState<TSymbol> State { get; private set; } 
			public int TransitionIndex { get; set; }
			public MemoizedInput<IParseTree<TSymbol>>.Iterator Iterator { get; private set; }
            // used when function may return multiple ok results during one call
			public IEnumerator<ParseResult<TSymbol>> NextPossibleResult { get; private set; } 

            public ParseState(DfaState<TSymbol> state, 
                int transitionIndex, 
                MemoizedInput<IParseTree<TSymbol>>.Iterator iterator, 
				IEnumerator<ParseResult<TSymbol>> nextPossibleResult = null)
			{
				State = state;
				TransitionIndex = transitionIndex;
				Iterator = iterator;
                NextPossibleResult = nextPossibleResult;
			}
		}

		private class ParseScope
		{
			public List<IParseTree<TSymbol>> Children { get; set; }
			public MemoizedInput<IParseTree<TSymbol>>.Iterator Iterator { get; set; }

			public ParseScope(List<IParseTree<TSymbol>> children, MemoizedInput<IParseTree<TSymbol>>.Iterator iterator)
			{
				Children = children;
				Iterator = iterator;
			}
		}

	}
}
