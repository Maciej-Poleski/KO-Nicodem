using System;
using Nicodem.Lexer;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Nicodem.Parser
{
	public class DfaState<TSymbol>
		where TSymbol : IComparable<TSymbol>, IEquatable<TSymbol>
	{
        public uint Accepting { get; private set; }
        [DebuggerDisplay("{TransitionsString()}")]
        public IReadOnlyList<KeyValuePair<TSymbol, DfaState<TSymbol>>> Transitions { get; private set; }
        public string Id { get; private set; } // For debugging purposes.
        private static int nextDebugId = 0;

		public DfaState()
        {
            this.Id = "" + (nextDebugId++);
        }

        public DfaState(string id)
        {
            this.Id = id;
        }

        public void Initialize(uint accepting, IReadOnlyList<KeyValuePair<TSymbol, DfaState<TSymbol>>> transitions)
		{
			if (Transitions != null) {
				throw new InvalidOperationException("DfaState may be initialized only once.");
			}
			Accepting = accepting;
			Transitions = transitions;
		}

		public DfaState<TSymbol> Transition(TSymbol symbol) {
			//return Transitions.FirstOrDefault(kv => kv.Key.Equals(symbol)).Value;
			throw new NotImplementedException();
		}

        public string TransitionsString()
        {
            var builder = new StringBuilder();
            foreach (var i in Transitions) {
                builder.Append(" ");
                builder.Append(i.Key);
                builder.Append(":");
                builder.Append(i.Value.Id);
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            return Id;
        }
	}
}

