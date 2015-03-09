using System;
using System.Collections.Generic;

namespace Nicodem.Parser
{
	public interface IParser<TProduction> where TProduction:IProduction
	{
		IParseTree<TProduction> Parse(IEnumerable<IParseTree<TProduction>> word);
	}
}

