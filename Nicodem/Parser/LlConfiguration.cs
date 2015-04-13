using System;
using Strilanc.Value;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nicodem.Parser
{
	public struct LlConfiguration<TSymbol> : IEquatable<LlConfiguration<TSymbol>> where TSymbol : ISymbol<TSymbol>
	{
		public readonly TSymbol label; 
		private Stack<DfaState<TSymbol> > stack;

		public LlConfiguration(TSymbol label) {
			this.label = label;
			stack = new Stack<DfaState<TSymbol> > ();
		}

		public LlConfiguration(TSymbol label, Stack<DfaState<TSymbol>> stack) {
			this.label = label;
			this.stack = stack;
		}

		public void Push(DfaState<TSymbol> state)
		{
			stack.Push (state);
		}

		// Performs shallow copy of the stack
		public Stack<DfaState<TSymbol>> copyOfStack() {
			var copy = new Stack<DfaState<TSymbol>> ();
			var array = stack.ToArray ();
			Array.Reverse (array);
			foreach (var state in array) {
				copy.Push (state);
			}
			return copy;
		}

		public DfaState<TSymbol> Pop() {
			return stack.Pop ();
		}

		public DfaState<TSymbol> Peek() {
			return stack.Peek();
		}

		public int Count() {
			return stack.Count;
		}

		public bool Subsumes(LlConfiguration<TSymbol> configuration)
		{
			if ( (this.label.CompareTo(configuration.label) != 0 ) 
				|| this.stack.Count > configuration.Count())
				return false;

			var firstStack = this.stack.ToArray();
			Array.Reverse (firstStack);

			var secondStack = this.stack.ToArray();
			Array.Reverse (secondStack);

			for (int i = 0; i < firstStack.Length; i++) {
				if (firstStack [i] != secondStack [i])
					return false;
			}

			return true;
		}


		#region IEquatable implementation
		public bool Equals (LlConfiguration<TSymbol> other)
		{
			return this.Subsumes (other) && other.Subsumes (this);
		}
		#endregion
	}
}

