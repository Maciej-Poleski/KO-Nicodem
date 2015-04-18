using System;
using System.Collections.Generic;
using Nicodem.Lexer;
using NUnit.Framework;

namespace Nicodem.Parser.Tests
{
	[TestFixture]
	public class LL1ParserTests
	{
		[Test]
		public void EmptyGrammarTest()
		{
			var start = new CharSymbol('S');
			var prodChar = RegExParser.Parse("$");
			var prodSymbol = new StringProduction(start.C, prodChar);

			var grammar = new Grammar<CharSymbol>(start, new Dictionary<CharSymbol, IProduction<CharSymbol>[]>{
				{ start, new []{prodSymbol} }
			});

			var parser = new LlParser<CharSymbol>(grammar);
			var emptyInput = new List<IEnumerable<IParseTree<CharSymbol>>>();

			Assert.IsNotNull(parser.Parse(emptyInput));
		}
	}
}

