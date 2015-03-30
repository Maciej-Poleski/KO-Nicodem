using System;
using Strilanc.Value;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nicodem.Parser
{
	public struct LlConfiguration<TSymbol> where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
	{
		public readonly TSymbol label; 
		private Stack<DfaState<TSymbol> > stack;

		public LlConfiguration(TSymbol label) {
			this.label = label;
			stack = new Stack<DfaState<TSymbol> > ();
		}

		public void Push(DfaState<TSymbol> state)
		{
			stack.Push (state);
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
			if ( (this.label.CompareTo(configuration.label) == 0 ) 
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
	}
}

