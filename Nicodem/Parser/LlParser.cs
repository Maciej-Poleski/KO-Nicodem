using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Nicodem.Parser
{
	public class LlParser<TProduction> : IParser<TProduction> where TProduction:IProduction
	{ 
		private readonly Grammar<TProduction> _grammar;

		public LlParser(Grammar<TProduction> grammar)
		{
			_grammar = ParserUtils.SimplifyGrammar(grammar);
		}

		public IParseTree<TProduction> 
			Parse(IEnumerable<IParseTree<TProduction>> word)
		{
			var memoizedWord = new MemoizedInput<IParseTree<TProduction>>(word);
			return ParseTerm(_grammar.Start, memoizedWord.Begin());
		}

		private IParseTree<TProduction> ParseTerm(Symbol term, MemoizedInput<IParseTree<TProduction>>.Iterator input)
		{
			var currentPos = input.Pos;

			throw new NotImplementedException();
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

			public Iterator Begin()
			{
				if(elements.Any()) {
					return new Iterator(this, 0);
				} else {
					return End();
				}
			}

			public Iterator End()
			{
				return new Iterator(this, -1);
			}

			Iterator At(int pos)
			{
				if(pos > elements.Count || pos < -1) {
					return End();
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

			public class Iterator : IComparable<Iterator>
			{

				public int Pos { get; private set; }
				private MemoizedInput<T> parent;

				public Iterator(MemoizedInput<T> parent, int pos)
				{
					this.parent = parent;
					Pos = pos;
				}

				public T Current
				{
					get {
						return parent.elements[Pos];
					}
				}

				public Iterator Next()
				{
					if(Pos + 1 < parent.elements.Count || parent.GoNext()) {
						Pos++;
					} else {
						Pos = -1;
					}
					return this;
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
			}

		}
	}
}

