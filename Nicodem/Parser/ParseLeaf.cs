﻿using System;

namespace Nicodem.Parser
{
	internal class ParseLeaf<TProduction> : IParseTree<TProduction> where TProduction:IProduction
	{
		public Symbol Symbol { get; private set; }
		public IFragment Fragment { get; private set; }

		public ParseLeaf()
		{
		}
	}
}

