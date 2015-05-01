using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class SimpleTwoProds : GrammarExample<CharSymbol>
	{
		private Grammar<CharSymbol> _grammar;

		public SimpleTwoProds()
		{
			CharSymbol start = new CharSymbol('S');
			CharSymbol F = new CharSymbol('F');

			StringProduction prod1 = new StringProduction(start.C, "F");
			StringProduction prod2 = new StringProduction(start.C, "(S+F)");
			StringProduction prod3 = new StringProduction(F.C, "a");

			_grammar = new Grammar<CharSymbol>(start, new Dictionary<CharSymbol, IProduction<CharSymbol>[]> {
				{ start, new IProduction<CharSymbol>[]{ prod1, prod2 } },
				{ F, new IProduction<CharSymbol>[]{ prod3 } }
			});
		}

		#region GrammarExample implementation

		public Grammar<CharSymbol> Grammar {
			get {
				return _grammar;
			}
		}

		public GrammarType Type {
			get {
				return GrammarType.LL1;
			}
		}

		public IEnumerable<ParseLeaf<CharSymbol>> ValidPrograms {
			get {
				return new List<ParseLeaf<CharSymbol>>();
			}
		}

		public IEnumerable<ParseLeaf<CharSymbol>> InvalidPrograms {
			get {
				return new List<ParseLeaf<CharSymbol>>();
			}
		}

		#endregion
	}
}

