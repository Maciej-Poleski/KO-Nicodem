using System;
using System.Collections.Generic;
using System.Collections;

namespace Nicodem.Parser
{
	public class LlParser<TProduction> : IParser<TProduction> where TProduction:IProduction
	{ 
		public LlParser(Grammar<TProduction> grammar)
		{
			throw new NotImplementedException();
		}

		public IParseTree<TProduction> 
			Parse(IEnumerable<IParseTree<TProduction>> word)
		{
			throw new NotImplementedException();
		}
	}
}

