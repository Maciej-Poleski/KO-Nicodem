using System;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Source;

namespace Nicodem.Parser.Tests
{
	internal class SimpleTwoProds : GrammarExample<CharSymbol>
	{
		private Grammar<CharSymbol> _grammar;

		public SimpleTwoProds()
		{
			CharSymbol start = new CharSymbol('S');
			CharSymbol F = new CharSymbol('F');

			StringProduction prod0 = new StringProduction(start.C, CharSymbol.EOF.ToString());
			StringProduction prod1 = new StringProduction(start.C, "F" + CharSymbol.EOF);
			StringProduction prod2a = new StringProduction(F.C, "\\(F\\+F\\)");
			StringProduction prod2b = new StringProduction(F.C, "\\(F\\-F\\)");
			StringProduction prod2c = new StringProduction(F.C, "\\(F\\*F\\)");
			StringProduction prod3 = new StringProduction(F.C, "a");

			_grammar = new Grammar<CharSymbol>(start, new Dictionary<CharSymbol, IProduction<CharSymbol>[]> {
				{ start, new IProduction<CharSymbol>[]{ prod0, prod1 } },
				{ F, new IProduction<CharSymbol>[]{ prod2a, prod2b, prod2c, prod3 } }
			});
		}

		#region GrammarExample implementation

		public Grammar<CharSymbol> Grammar
		{
			get { return _grammar; }
		}

		public GrammarType Type
		{
			get { return GrammarType.LL1; }
		}

		public Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] ValidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Empty", 
						ToTree("")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Just a", 
						ToTree("a")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple add", 
						ToTree("(a+a)")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple substract", 
						ToTree("(a-a)")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple multiply", 
						ToTree("(a*a)")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Three Ops 1", 
						ToTree("((a*a)+(a-a))")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Three Ops 2", 
						ToTree("(a*(a+(a-a)))")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"More complex", 
						ToTree("((a-(a-a))*(a+(a+a)))")
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Longest", 
						ToTree("((a*(a+(a-a)))-((a-(a-a))*(a+(a+a))))")
					)
				};
			}
		}

		public Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[]{
				};
			}
		}

		private static IEnumerable<ParseLeaf<CharSymbol>> ToTree(string s)
		{
			var origin = new StringOrigin(s);
			return s.Select(c => new ParseLeaf<CharSymbol>(new OriginFragment(origin, new OriginPosition(), new OriginPosition()), 
				new CharSymbol(c)));
		}

		#endregion
	}
}

