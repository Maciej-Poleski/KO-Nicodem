using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Nicodem.Lexer;
using Nicodem.Core;

namespace Nicodem.Parser
{
	public class LlParser<TProduction> : IParser<TProduction> where TProduction:IProduction
	{ 
		private readonly Grammar<TProduction> _grammar;

		public LlParser(Grammar<TProduction> grammar)
		{
			if(grammar.HasLeftRecursion()) {
				// TODO fail somehow
			}
            _grammar = grammar;
		}

		// TODO specify what happens when parser fails to eat the word
		public IParseTree<TProduction> 
			Parse(IEnumerable<IParseTree<TProduction>> word)
		{
			var memoizedWord = new MemoizedInput<IParseTree<TProduction>>(word);
			// TODO check what if the whole input is not eaten
			return ParseTerm(_grammar.Start, memoizedWord, memoizedWord.Begin).Tree;
		}

		private ParseResult ParseTerm(Symbol term, MemoizedInput<IParseTree<TProduction>> word, MemoizedInput<IParseTree<TProduction>>.Iterator input)
		{
			// stack for backtracking - <position in word, current state of appropriate DFA>
			var dfa = _grammar.Automatons[term];
			var st = new Stack<ParseState>(); 
			var children = new Stack<IParseTree<TProduction>>(); 

			// the furthest in terms of position in the word parsed paths along with productions
			var furthestParsed = new ParseScope(new List<IParseTree<TProduction>>(), input); // maybe used in future for error handling
			ParseScope furthestAccepting = null;

			st.Push(new ParseState(dfa.Start, 0, input));

			while(st.Any()) {
				var node = st.Peek().State;
				var it = st.Peek().Iterator;

				if(node.Accepting > 0 && (furthestAccepting == null || it > furthestAccepting.Iterator)) {
					var newFurthestAccepting = children.ToList();
					newFurthestAccepting.Reverse();
					furthestAccepting = new ParseScope(newFurthestAccepting, it);
				}

				var trans = node.Transitions;
				for(int i = st.Peek().TransitionIndex; i < trans.Length; i++) {
					if(it == word.End) {
						// TODO EOF - what to do?
					} else if(_grammar.InFirstPlus(trans[i].Key, it.Current.Symbol)) {

						if(_grammar.IsTerminal(it.Current.Symbol)) {

							children.Push(new ParseLeaf<TProduction>()); // TODO set the leaf appropriately
							st.Push(new ParseState(trans[i].Value, 0, it.Next()));
							break;
						} else {

							var nextResult = ParseTerm(trans[i].Key, word, it);
							if(nextResult) {
								children.Push(nextResult.Tree);
								st.Push(new ParseState(trans[i].Value, 0, it));
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
					Backtrack(st);
				}
			}

			if(furthestAccepting != null) {
				// can return an ok tree choosing
				var branch = new ParseBranch<TProduction>(); // TODO setup branch appropriately
				return new ParseResult(branch, furthestAccepting.Iterator);
			} else {
				var branch = new ParseBranch<TProduction>(); // TODO setup branch appropriately
				return new ParseResult(branch, furthestParsed.Iterator, false);
			}
		}

		private static void Backtrack(Stack<ParseState> stack)
		{
			do {
				stack.Pop();
			} while(stack.Any() && stack.Peek().TransitionIndex + 1 >= stack.Peek().State.Transitions.Length);

			if(stack.Any()) {
				var oldPState = stack.Pop();
				stack.Push(new ParseState(oldPState.State, oldPState.TransitionIndex + 1, oldPState.Iterator));
			}
		}

		/* --- data types ----- */
		private struct ParseState
		{
			public DFAState<Symbol> State { get; private set; } 
			public int TransitionIndex { get; private set; }
			public MemoizedInput<IParseTree<TProduction>>.Iterator Iterator { get; private set; }

			public ParseState(DFAState<Symbol> state, int transitionIndex, MemoizedInput<IParseTree<TProduction>>.Iterator iterator)
				: this()
			{
				State = state;
				TransitionIndex = transitionIndex;
				Iterator = iterator;
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
