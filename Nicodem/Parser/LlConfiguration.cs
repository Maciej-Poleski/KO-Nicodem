using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nicodem.Parser
{
    public class LlConfiguration<TSymbol> : IEquatable<LlConfiguration<TSymbol>> where TSymbol : ISymbol<TSymbol>
	{
        public readonly TSymbol label; 
        public readonly ImmutableList<DfaState<TSymbol> > stack = ImmutableList.Create<DfaState<TSymbol> >();

		public LlConfiguration(TSymbol label) {
			this.label = label;
		}

        public LlConfiguration(TSymbol label, Stack<DfaState<TSymbol>> stack) {
            this.label = label;
            var array = stack.ToArray();
            Array.Reverse(array);

            foreach (var state in array) {
                this.stack = this.stack.Add(state);
            }
        }

        public LlConfiguration(TSymbol label, ImmutableList<DfaState<TSymbol> > stack) {
            this.label = label;
            this.stack = stack;
        }

        public LlConfiguration<TSymbol> Push(DfaState<TSymbol> state) {
            return new LlConfiguration<TSymbol>(label, stack.Add(state));
		}

        public LlConfiguration<TSymbol> Pop() {
            return new LlConfiguration<TSymbol>(label, stack.RemoveAt(stack.Count - 1));
		}

		public DfaState<TSymbol> Peek() {
            return stack[stack.Count - 1];
		}

		public int Count() {
			return stack.Count;
		}

        // Performs shallow copy of the stack
        public Stack<DfaState<TSymbol>> copyOfStack() {
            var copy = new Stack<DfaState<TSymbol>> ();
            foreach (var state in stack) {
                copy.Push (state);
            }
            return copy;
        }

		public bool Subsumes(LlConfiguration<TSymbol> configuration)
		{
			if ( (this.label.CompareTo(configuration.label) != 0 ) 
				|| this.stack.Count > configuration.Count())
				return false;

			var firstStack = this.stack;

            var secondStack = configuration.stack;

            for (int i = 0; i < firstStack.Count; i++) {
				if (firstStack [i] != secondStack[i])
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

