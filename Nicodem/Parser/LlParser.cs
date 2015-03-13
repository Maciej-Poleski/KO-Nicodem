using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Nicodem.Lexer;

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
			_grammar = ParserUtils.SimplifyGrammar(grammar);
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

	internal class MemoizedInput<T>
	{

		private List<T> elements;
		private IEnumerator<T> current;

		public MemoizedInput(IEnumerable<T> input)
		{
			current = input.GetEnumerator();
			if(current.MoveNext()) {
				elements.Add(current.Current);
			}
		}

		public Iterator Begin
		{
			get {
				if(elements.Any()) {
					return new Iterator(this, 0);
				} else {
					return End;
				}
			}
		}

		public Iterator End
		{
			get {
				return new Iterator(this, -1);
			}
		}

		public Iterator At(int pos)
		{
			if(pos > elements.Count || pos < -1) {
				return End;
			} else {
				return new Iterator(this, pos);
			}
		}

		private bool GoNext() {
			if(current.MoveNext()) {
				elements.Add(current.Current);
				return true;
			} else {
				return false;
			}
		}

		public struct Iterator : IComparable<Iterator>
		{

			public int Pos { get; private set; }
			private readonly MemoizedInput<T> _parent;

			public Iterator(MemoizedInput<T> parent, int pos)
				: this()
			{
				this.Pos = pos;
				_parent = parent;
			}

			public T Current
			{
				get {
					return _parent.elements[Pos];
				}
			}

			public Iterator Next()
			{
				if(Pos + 1 < _parent.elements.Count) {
					_parent.GoNext();
				}
				return new Iterator(_parent, Pos + 1);
			}

			#region IComparable implementation

			public int CompareTo(Iterator other)
			{
				if(Pos == -1) {
					if(other.Pos == -1) {
						return 0;
					} else {
						return 1;
					}
				} else if(other.Pos == -1) {
					return -1;
				} else {
					return Pos - other.Pos;
				}
			}

			#endregion

			public static bool operator==(Iterator a, Iterator b)
			{
				return a.Pos == b.Pos && a._parent == b._parent;
			}

			public static bool operator!=(Iterator a, Iterator b)
			{
				return !(a.Pos == b.Pos);
			}

			public static bool operator>(Iterator a, Iterator b)
			{
				return a.CompareTo(b) > 0;
			}

			public static bool operator>=(Iterator a, Iterator b)
			{
				return a.CompareTo(b) > 0 || a == b;
			}

			public static bool operator<(Iterator a, Iterator b)
			{
				return !(a >= b);
			}

			public static bool operator<=(Iterator a, Iterator b)
			{
				return !(a > b);
			}

			public override bool Equals(Object other)
			{
				if(other is Iterator) {
					return this == (Iterator) other;
				} else {
					return false;
				}
			}

			public override int GetHashCode()
			{
				return Pos.GetHashCode() + _parent.GetHashCode();
			}
		}
	}
}

