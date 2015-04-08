using System;
using Nicodem.Lexer;
using NUnit.Framework;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	[TestFixture]
	public class GrammarTests
	{
		Grammar<CharSymbol> grammar;

		/* A -> BC
		 * A -> BD
		 * E -> EE
		 */
		[TestFixtureSetUp]
		public void init()
		{

		}

		[Test]
		public void test1()
		{
			var productions = new Dictionary<CharSymbol, IProduction<CharSymbol>[]>();
			/*
			productions[new CharSymbol('A')] = new StringProduction[]{ 
				new StringProduction('A', "BC"), 
				new StringProduction('A', "BD")
			}; */
			productions[new CharSymbol('E')] = new StringProduction[]{ 
				new StringProduction('E', "A")
			};
			grammar = new Grammar<CharSymbol>(productions);
			// TODO: this test fails, because grammar construction fails.
		}
	}
}

