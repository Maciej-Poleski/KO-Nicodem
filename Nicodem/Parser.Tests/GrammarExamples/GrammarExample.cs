using System;
using Nicodem.Source;
using System.Linq;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal enum GrammarType {
		LL1, 
		LL2, 
		OTHER
	}

	internal abstract class GrammarExample
	{
		private ProductionBuilder _builder = new ProductionBuilder();
		private Grammar<CharSymbol> _grammar;

		public GrammarExample(string start, Tuple<string, string[]>[] productions)
		{
			Dictionary<CharSymbol, List<StringProduction>> sprods = new Dictionary<CharSymbol, List<StringProduction>>();
			foreach(var prod in productions) {
				var p = _builder.GetProduction(prod.Item1, prod.Item2);
				if(!sprods.ContainsKey(p.Lhs)) {
					sprods[p.Lhs] = new List<StringProduction>();
				}
				sprods[p.Lhs].Add(p);
			}
			var dict = new Dictionary<CharSymbol, IProduction<CharSymbol>[]>();
			foreach(var kvp in sprods) {
				dict[kvp.Key] = kvp.Value.ToArray();
			}
			_grammar = new Grammar<CharSymbol>(_builder.GetSymbolForTerm(start), dict);
		}

		public Grammar<CharSymbol> Grammar 
		{ 
			get { return _grammar; }
		}

		public abstract string Name { get; }

		public abstract GrammarType Type { get; }
		// Tuple<Description, Code>
		public abstract Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] ValidPrograms { get; }
		public abstract Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms { get; }

		protected IEnumerable<ParseLeaf<CharSymbol>> BuildCode(string[] input)
		{
			var origin = new StringOrigin("");
			return input.Select(k => {
				return new ParseLeaf<CharSymbol>(
					new OriginFragment(origin, 
						new OriginPosition(), 
						new OriginPosition()), 
					_builder.GetSymbolForTerm(k));
			});
		}
	}
}

