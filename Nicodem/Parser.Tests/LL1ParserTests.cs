using System;
using System.Linq;
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
			var emptyInput = new List<ParseLeaf<CharSymbol>>();

			var res = parser.Parse(emptyInput);
			Assert.IsNotNull(res);

			var resAsBranch = res as ParseBranch<CharSymbol>;
			Assert.IsNotNull(resAsBranch);
			Assert.AreEqual(resAsBranch.Children.Count(), 1);
			Assert.AreEqual(resAsBranch.Symbol, start);

			var resLeaf = resAsBranch.Children.ElementAt(0) as ParseLeaf<CharSymbol>;
			Assert.IsNotNull(resLeaf);
			Assert.AreEqual(resLeaf.Symbol, new CharSymbol('$'));
		}

		[Test]
		public void ValidProgramsTests()
		{
			var twoProdGrammar = new SimpleTwoProds();
			var parser = new LlParser<CharSymbol>(twoProdGrammar.Grammar);
			foreach(var prog in twoProdGrammar.ValidPrograms) {
				Assert.NotNull(parser.Parse(prog.Item2), prog.Item1);
			}
		}
	}
}

