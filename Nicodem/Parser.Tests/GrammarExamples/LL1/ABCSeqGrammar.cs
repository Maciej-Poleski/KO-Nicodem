using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class ABCSeqGrammar : GrammarExample
	{
		public ABCSeqGrammar()
			: base(
				"S",
				new Tuple<string, string[]>[] {
					new Tuple<string, string[]>("S", new string[]{ "$" }),
					new Tuple<string, string[]>("S", new string[]{ "F", "S" }),
					new Tuple<string, string[]>("F", new string[]{ "a", "b" }),
					new Tuple<string, string[]>("F", new string[]{ "c" }),
				}
			)
		{
		}

		#region GrammarExample implementation

		public override GrammarType Type
		{
			get { return GrammarType.LL1; }
		}

		public override string Name
		{
			get { return "(ab|c)* grammar"; }
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] ValidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Empty", 
						BuildCode(new string[]{ })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"c", 
						BuildCode(new string[]{ "c" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"ab", 
						BuildCode(new string[]{ "a", "b" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"cababc", 
						BuildCode("c.a.b.a.b.c".Split('.'))
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"abababcabcccab", 
						BuildCode("a.b.a.b.a.b.c.a.b.c.c.c.a.b".Split('.'))
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"ccc", 
						BuildCode(new string[]{"c","c","c"})
					)
				};
			}
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"a", 
						BuildCode(new string[]{ "a" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"b", 
						BuildCode(new string[]{ "b" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"acb", 
						BuildCode(new string[]{ "a", "c", "b" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"cabac", 
						BuildCode("c.a.b.a.c".Split('.'))
					),
				};
			}
		}

		#endregion
	}
}

