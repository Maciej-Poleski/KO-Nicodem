﻿using System;

namespace Nicodem.Parser
{
	public interface IParseTree<TProduction> where TProduction:IProduction
	{
		Symbol Symbol { get; }
		IFragment Fragment { get; }
	}
}
