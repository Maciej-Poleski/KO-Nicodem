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
        public IParseTree<TSymbol> Parse(IEnumerable<IEnumerable<IParseTree<TSymbol>>> word)
		{
            var memoizedWord = new MemoizedInput<IEnumerable<IParseTree<TSymbol>>>(word);
            var result = ParseTerm(_grammar.Start, memoizedWord, memoizedWord.Begin, 0, true).ElementAt(0);

            // whole input has to be eaten
            if(result && result.Iterator == memoizedWord.End) {
                return result.Tree;
            } else {
                return null;
            }
		}

        // returns either a list of successfull parsed results or a singleton list with failure 
        private IEnumerable<ParseResult<TSymbol>> ParseTerm(TSymbol term, MemoizedInput<IEnumerable<IParseTree<TSymbol>>> word, MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator input, int inputOption, bool canBacktrackOnOption)
		{
			var dfa = _grammar.Automatons[term];
			// stack for backtracking - <position in word, current state of appropriate DFA>
			var st = new Stack<ParseState>(); 
			var children = new Stack<IParseTree<TSymbol>>(); 
            bool accepted = false;
			var eof = ParserUtils<TSymbol>.GetEOF();

			// the furthest in terms of position in the word parsed paths along with productions
			var furthestParsed = new ParseScope(new List<IParseTree<TSymbol>>(), input, inputOption); // maybe used in the future for error handling

            st.Push(new ParseState(dfa.Start, 0, input, inputOption, canBacktrackOnOption));

			while(st.Any()) {
				var parseState = st.Peek();
				var node = parseState.State;
				var it = parseState.Iterator;
                int opt = parseState.InputOption;
                bool cbopt = parseState.CanBacktrackOnOption;
                TSymbol currentSymbol = (it != word.End) ? it.Current.ElementAt(opt).Symbol : eof;

                if(node.Accepting > 0 && (currentSymbol.Equals(eof) || _grammar.Follow[term].Contains(currentSymbol))) {
                    accepted = true;
					var parsedChildren = children.ToList();
					parsedChildren.Reverse();
					var parsedTree = new ParseBranch<TSymbol>(
                        GetFragmentRange(input.Current.ElementAt(opt).Fragment, children.Peek().Fragment),
						term, 
						_grammar.WhichProduction[node.Accepting], 
						parsedChildren);

					yield return new ParseResult<TSymbol>(parsedTree, it, opt, cbopt);
				}

				var trans = node.Transitions;
				var ind = parseState.TransitionIndex;
				for(; ind < trans.Count; ind++) {

                    if(_grammar.InFirstPlus(trans[ind].Key, currentSymbol)) {
						if(_grammar.IsTerminal(trans[ind].Key)) {

                            children.Push(new ParseLeaf<TSymbol>(it != word.End ? it.Current.ElementAt(opt).Fragment : null, currentSymbol)); // TODO It would be better to have special END fragment
							st.Push(new ParseState(trans[ind].Value, 0, it.Next(), 0, true));
							break;
						} else {
							IEnumerator<ParseResult<TSymbol>> resultIt = ParseTerm(trans[ind].Key, word, it, opt, cbopt).GetEnumerator();
                            resultIt.MoveNext();
                            if(resultIt.Current) { // TODO else and look at the furthest parsed
                                children.Push(resultIt.Current.Tree);
                                st.Push(new ParseState(trans[ind].Value, 0, resultIt.Current.Iterator, resultIt.Current.InputOption, resultIt.Current.CanBacktrackOnInput, resultIt));
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
                        furthestParsed = new ParseScope(newFurthestParsed, st.Peek().Iterator, st.Peek().InputOption);
					}
					Backtrack(st, children, word);
				}
			}

			if(accepted) {
                yield break;
			} else {
				var branch = new ParseBranch<TSymbol>(
                    input != word.End ? GetFragmentRange(input.Current.ElementAt(inputOption).Fragment, furthestParsed.Children.Last().Fragment) : null, 
						term, 
						_grammar.Productions[term][0],  // TODO could not parse any productions
						furthestParsed.Children);
                yield return new ParseResult<TSymbol>(branch, furthestParsed.Iterator, furthestParsed.InputOption, false, false);
			}
		}

        private static void Backtrack(Stack<ParseState> stack, Stack<IParseTree<TSymbol>> children, MemoizedInput<IEnumerable<IParseTree<TSymbol>>> word)
		{
            while(stack.Any()) {
                var state = stack.Pop();
                if(children.Any()) {
                    children.Pop();
                }

                if(state.NextPossibleResult != null && state.NextPossibleResult.MoveNext()) {

                    var nextRes = state.NextPossibleResult.Current;
                    children.Push(nextRes.Tree);
                    stack.Push(new ParseState(state.State, state.TransitionIndex, nextRes.Iterator, nextRes.InputOption, nextRes.CanBacktrackOnInput, state.NextPossibleResult));
                    return;
                } else if(state.CanBacktrackOnOption && !state.Iterator.Equals(word.End) && state.InputOption + 1 < state.Iterator.Current.Count()) {

                    stack.Push(new ParseState(state.State, state.TransitionIndex, state.Iterator, state.InputOption + 1));
                    return;
                } else if(state.TransitionIndex + 1 < state.State.Transitions.Count) {

                    stack.Push(new ParseState(state.State, state.TransitionIndex + 1, state.Iterator, state.InputOption));
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
			public MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator Iterator { get; private set; }
            public int InputOption { get; private set; }
            public bool CanBacktrackOnOption { get; private set; }
            // used when function may return multiple ok results during one call
			public IEnumerator<ParseResult<TSymbol>> NextPossibleResult { get; private set; } 

            public ParseState(DfaState<TSymbol> state, 
                int transitionIndex, 
                MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator iterator, 
                int inputOption = 0,
                bool canBacktrackOnOption = false,
				IEnumerator<ParseResult<TSymbol>> nextPossibleResult = null)
			{
				State = state;
				TransitionIndex = transitionIndex;
				Iterator = iterator;
                InputOption = inputOption;
                CanBacktrackOnOption = canBacktrackOnOption;
                NextPossibleResult = nextPossibleResult;
			}
		}

		private class ParseScope
		{
			public List<IParseTree<TSymbol>> Children { get; set; }
			public MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator Iterator { get; set; }
            public int InputOption;

            public ParseScope(List<IParseTree<TSymbol>> children, MemoizedInput<IEnumerable<IParseTree<TSymbol>>>.Iterator iterator, int inputOption)
			{
				Children = children;
				Iterator = iterator;
                InputOption = inputOption;
			}
		}

	}
}
