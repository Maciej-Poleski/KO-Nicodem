using System;
using System.Collections.Generic;

using Nicodem.Source;

namespace Nicodem.Parser
{
	internal class ParseBranch<TProduction> : IParseTree<TProduction>
		where TProduction:IProduction
	{
		public Symbol Symbol { get; private set; }
		public IFragment Fragment { get; private set; }
		public TProduction Production { get; private set; }
		public IEnumerable<IParseTree<TProduction>> Children { get; private set; }

		public ParseBranch()
		{
		}
	}
}

