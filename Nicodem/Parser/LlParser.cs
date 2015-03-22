using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Nicodem.Core;
using Nicodem.Source;

namespace Nicodem.Parser
{
	public class LlParser<TProduction> : IParser<TProduction> where TProduction:IProduction
	{ 
		private readonly Grammar<TProduction> _grammar;

		public LlParser(Grammar<TProduction> grammar)
		{
			if(grammar.HasLeftRecursion()) {
                throw new ArgumentException("Grammar has left recursion");
			}
            _grammar = grammar;
		}

        // At this moment return null when parsing fails
        public IParseTree<TProduction> Parse(IEnumerable<IParseTree<TProduction>> word)
		{
			var memoizedWord = new MemoizedInput<IParseTree<TProduction>>(word);
            var result = ParseTerm(_grammar.Start, memoizedWord, memoizedWord.Begin).ElementAt(0);

            // whole input has to be eaten
            if(result && result.Iterator == memoizedWord.End) {
                return result.Tree;
            } else {
                return null;
            }
		}

        // returns either a list of successfull parsed results or a singleton list with failure 
        private IEnumerable<ParseResult> ParseTerm(ISymbol term, MemoizedInput<IParseTree<TProduction>> word, MemoizedInput<IParseTree<TProduction>>.Iterator input)
		{
			// stack for backtracking - <position in word, current state of appropriate DFA>
			var dfa = _grammar.Automatons[term];
			var st = new Stack<ParseState>(); 
			var children = new Stack<IParseTree<TProduction>>(); 
            bool accepted = false;

			// the furthest in terms of position in the word parsed paths along with productions
			var furthestParsed = new ParseScope(new List<IParseTree<TProduction>>(), input); // maybe used in the future for error handling

			st.Push(new ParseState(dfa.Start, 0, input));

			while(st.Any()) {
				var node = st.Peek().State;
				var it = st.Peek().Iterator;
                ISymbol currentSymbol = (it != word.End) ? it.Current.Symbol : term.EOF;

                if(node.Accepting > 0 && _grammar.Follow[term].Contains(currentSymbol)) {
                    accepted = true;
					var parsedChildren = children.ToList();
					parsedChildren.Reverse();
					var parsedTree = new ParseBranch<TProduction>(
						GetFragmentRange(input.Current.Fragment, children.Peek().Fragment),
						term, 
						_grammar.WhichProduction[node.Accepting], 
						parsedChildren);

                    yield return new ParseResult(parsedTree, it);
				}

				var trans = node.Transitions;
				for(int i = st.Peek().TransitionIndex; i < trans.Count; i++) {

                    if(_grammar.InFirstPlus(trans[i].Key, currentSymbol)) {
                        if(_grammar.IsTerminal(currentSymbol) || currentSymbol == term.EOF) {

							children.Push(new ParseLeaf<TProduction>(it.Current.Fragment, currentSymbol));
							st.Push(new ParseState(trans[i].Value, 0, it.Next()));
							break;
						} else {
                            IEnumerator<ParseResult> resultIt = ParseTerm(trans[i].Key, word, it).GetEnumerator();
                            resultIt.MoveNext();
                            if(resultIt.Current) { // TODO else and look at the furthest parsed
                                children.Push(resultIt.Current.Tree);
                                st.Push(new ParseState(trans[i].Value, 0, resultIt.Current.Iterator, resultIt));
                                break;
                            }
						}
					}
				}

				// could not find next parsing transition
				if(node == st.Peek().State) { 
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
				var branch = new ParseBranch<TProduction>(
					GetFragmentRange(input.Current.Fragment, furthestParsed.Children.Last().Fragment), 
						term, 
						_grammar.Productions[term][0],  // TODO could not parse any productions
						furthestParsed.Children);
				yield return new ParseResult(branch, furthestParsed.Iterator, false);
			}
		}

        private static void Backtrack(Stack<ParseState> stack, Stack<IParseTree<TProduction>> children)
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
		private struct ParseState
		{
			public DfaState<ISymbol> State { get; private set; } 
			public int TransitionIndex { get; private set; }
			public MemoizedInput<IParseTree<TProduction>>.Iterator Iterator { get; private set; }
            // used when function may return multiple ok results during one call
            public IEnumerator<ParseResult> NextPossibleResult { get; private set; } 

            public ParseState(DfaState<ISymbol> state, 
                int transitionIndex, 
                MemoizedInput<IParseTree<TProduction>>.Iterator iterator, 
                IEnumerator<ParseResult> nextPossibleResult = null)
				: this()
			{
				State = state;
				TransitionIndex = transitionIndex;
				Iterator = iterator;
                NextPossibleResult = nextPossibleResult;
			}
		}

		private class ParseScope
		{
			public List<IParseTree<TProduction>> Children { get; set; }
			public MemoizedInput<IParseTree<TProduction>>.Iterator Iterator { get; set; }

			public ParseScope(List<IParseTree<TProduction>> children, MemoizedInput<IParseTree<TProduction>>.Iterator iterator)
			{
				Children = children;
				Iterator = iterator;
			}
		}

		// TODO consider adding something denoting parsing error
		private struct ParseResult 
		{

			public IParseTree<TProduction> Tree { get; private set; } 
			public MemoizedInput<IParseTree<TProduction>>.Iterator Iterator { get; private set; }
			private bool _ok;

			public ParseResult(IParseTree<TProduction> tree, MemoizedInput<IParseTree<TProduction>>.Iterator iterator, bool ok = true)
				: this()
			{
				Tree = tree;
				Iterator = iterator;
				_ok = ok;
			}

			public static implicit operator bool(ParseResult result)
			{
				return result._ok;
			}
		}
	}
}
