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
            var result = ParseTerm(_grammar.Start, memoizedWord, new InputPosition<TSymbol>(memoizedWord.Begin, 0, true)).First();

            // whole input has to be eaten
            if(result && result.Position.Iterator == memoizedWord.End) {
                return result.Tree;
            } else {
                return null;
            }
		}

        // returns either a list of successfull parsed results or a singleton list with failure 
        private IEnumerable<ParseResult<TSymbol>> ParseTerm(TSymbol term, MemoizedInput<IEnumerable<IParseTree<TSymbol>>> word, InputPosition<TSymbol> pos)
		{
			var dfa = _grammar.Automatons[term];
			// stack for backtracking - <position in word, current state of appropriate DFA>
			var st = new Stack<ParseState>(); 
			var children = new Stack<IParseTree<TSymbol>>(); 
            bool accepted = false;
			var eof = ParserUtils<TSymbol>.GetEOF();

            st.Push(new ParseState(dfa.Start, 0, pos));

			while(st.Any()) {
				var parseState = st.Peek();
				var node = parseState.State;
				var it = parseState.Position.Iterator;
                var opt = parseState.Position.InputOption;
                TSymbol currentSymbol = (it != word.End) ? it.Current.ElementAt(opt).Symbol : eof;

                if(node.Accepting > 0 && (currentSymbol.Equals(eof) || _grammar.Follow[term].Contains(currentSymbol))) {
                    accepted = true;
					var parsedChildren = children.ToList();
					parsedChildren.Reverse();
					var parsedTree = new ParseBranch<TSymbol>(
                        GetFragmentRange(pos.Iterator.Current.ElementAt(opt).Fragment, children.Peek().Fragment),
						term, 
						_grammar.WhichProduction[node.Accepting], 
						parsedChildren);

                    yield return new ParseResult<TSymbol>(parsedTree, parseState.Position);
				}

				var trans = node.Transitions;
				var ind = parseState.TransitionIndex;
				for(; ind < trans.Count; ind++) {

                    if(_grammar.InFirstPlus(trans[ind].Key, currentSymbol)) {
						if(_grammar.IsTerminal(trans[ind].Key)) {

                            children.Push(new ParseLeaf<TSymbol>(it != word.End ? it.Current.ElementAt(opt).Fragment : null, currentSymbol)); // TODO It would be better to have special END fragment
                            st.Push(new ParseState(trans[ind].Value, 0, new InputPosition<TSymbol>(it.Next(), 0, true)));
							break;
						} else {
                            var inputPos = new InputPosition<TSymbol>(it, opt);
                            IEnumerator<ParseResult<TSymbol>> resultIt = ParseTerm(trans[ind].Key, word, inputPos).GetEnumerator();
                            resultIt.MoveNext();
                            if(resultIt.Current) {
                                children.Push(resultIt.Current.Tree);
                                st.Push(new ParseState(trans[ind].Value, 0, resultIt.Current.Position, resultIt));
                                break;
                            }
						}
					}
				}
				parseState.TransitionIndex = ind;

				// could not find next parsing transition
				if(ind >= trans.Count) { 
					Backtrack(st, children, word);
				}
			}

			if(accepted) {
                yield break;
			} else {
                yield return new ParseResult<TSymbol>(null, pos, false);
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
                    stack.Push(
                        new ParseState(state.State, 
                        state.TransitionIndex, 
                        new InputPosition<TSymbol>(nextRes.Position.Iterator, nextRes.Position.InputOption, nextRes.Position.BacktrackableInput), 
                        state.NextPossibleResult));
                    return;
                } else if(state.TransitionIndex + 1 < state.State.Transitions.Count) {

                    stack.Push(new ParseState(state.State, state.TransitionIndex + 1, state.Position));
                    return;
                } else if(state.Position.BacktrackableInput && 
                    state.Position.Iterator != word.End && state.Position.InputOption + 1 < state.Position.Iterator.Current.Count()) 
                {
                    stack.Push(new ParseState(state.State, 0, 
                        new InputPosition<TSymbol>(state.Position.Iterator, state.Position.InputOption + 1, true)));
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
            public InputPosition<TSymbol> Position { get; private set; }
            // used when function may return multiple ok results during one call
			public IEnumerator<ParseResult<TSymbol>> NextPossibleResult { get; private set; } 

            public ParseState(DfaState<TSymbol> state, 
                int transitionIndex, 
                InputPosition<TSymbol> position,
				IEnumerator<ParseResult<TSymbol>> nextPossibleResult = null)
			{
				State = state;
				TransitionIndex = transitionIndex;
                Position = position;
                NextPossibleResult = nextPossibleResult;
			}
		}
	}
}
