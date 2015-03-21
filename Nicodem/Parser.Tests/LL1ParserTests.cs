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

			var grammar = new Grammar<StringProduction>(new Dictionary<ISymbol, StringProduction[]>{
				{ start, new []{prodSymbol} }
			});

			var parser = new LlParser<StringProduction>(grammar);
			var emptyInput = new List<IParseTree<StringProduction>>();

			Assert.IsNotNull(parser.Parse(emptyInput));
		}
	}
}

