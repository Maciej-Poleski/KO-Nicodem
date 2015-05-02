﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Nicodem.Lexer;
using NUnit.Framework;

namespace Nicodem.Parser.Tests
{
	[TestFixture]
	public class ParsersTests
	{
		private GrammarExample[] _grammars = new GrammarExample[]
		{
			new SimpleTwoProds(),
			new ABCSeqGrammar(),
		};

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
		public void LL1ValidProgramsTests()
		{
			var ll1s = _grammars.Where(k => k.Type.Equals(GrammarType.LL1));

			foreach(var ge in ll1s) {
				Debug.WriteLine("Testing valid programs for grammar: " + ge.Name);
				var parser = new LlParser<CharSymbol>(ge.Grammar);

				foreach(var prog in ge.ValidPrograms) {
					Assert.NotNull(parser.Parse(prog.Item2), ge.Name + ": " + prog.Item1);
				}
			}
		}

		[Test]
		public void LL1InValidProgramsTests()
		{
			var ll1s = _grammars.Where(k => k.Type.Equals(GrammarType.LL1));

			foreach(var ge in ll1s) {
				Debug.WriteLine("Testing invalid programs for grammar: " + ge.Name);
				var parser = new LlParser<CharSymbol>(ge.Grammar);

				foreach(var prog in ge.InvalidPrograms) {
					Assert.IsNull(parser.Parse(prog.Item2), ge.Name + ": " + prog.Item1);
				}
			}
		}
	}
}
