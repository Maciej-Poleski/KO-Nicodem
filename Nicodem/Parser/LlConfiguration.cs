using System;
using Strilanc.Value;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Nicodem.Parser
{
	public struct LlConfiguration
	{
		public May<ISymbol> Decision { get; private set; } // optional because the decision can be to accept
		public ImmutableList<DfaState<ISymbol>> Stack { get; private set; }

		public IEnumerable<KeyValuePair<May<ISymbol>, LlConfiguration>> OutgoingEdges
		{
			get {
				throw new NotImplementedException();
			}
		}

		public LlConfiguration(May<ISymbol> decision, ImmutableList<DfaState<ISymbol>> stack)
		{
			throw new NotImplementedException();
		}

		public bool Subsumes(LlConfiguration rhs)
		{
			throw new NotImplementedException();
		}
	}
}

