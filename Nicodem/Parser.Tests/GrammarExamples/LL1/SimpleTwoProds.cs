using System;
using System.Collections.Generic;
using System.Linq;
using Nicodem.Source;

namespace Nicodem.Parser.Tests
{
	internal class SimpleTwoProds : GrammarExample
	{
		public SimpleTwoProds()
			: base(
				"S",
				new Tuple<string, string[]>[] {
					new Tuple<string, string[]>("S", new string[]{ "$" }),
					new Tuple<string, string[]>("S", new string[]{ "F", "$" }),
					new Tuple<string, string[]>("F", new string[]{ "(", "F", "+", "F", ")" }),
					new Tuple<string, string[]>("F", new string[]{ "(", "F", "*", "F", ")" }),
					new Tuple<string, string[]>("F", new string[]{ "(", "F", "-", "F", ")" }),
					new Tuple<string, string[]>("F", new string[]{ "a" })
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
			get { return "Simple arithmetics"; }
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
						"Just a", 
						BuildCode(new string[]{ "a" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple add", 
						BuildCode(new string[]{ "(", "a", "+", "a", ")" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple substract", 
						BuildCode(new string[]{ "(", "a", "-", "a", ")" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple multiply", 
						BuildCode(new string[]{ "(", "a", "*", "a", ")" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Three Ops 1", 
						BuildCode(new string[]{"(","(","a","*","a",")","+","(","a","-","a",")",")"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Three Ops 2", 
						BuildCode(new string[]{"(","a","*","(","a","+","(","a","-","a",")",")", ")"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"More complex", 
						BuildCode(new string[]{"(","(","a","-","(","a","-","a",")",")","*","(","a","+","(","a","+","a",")",")",")"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Longest", 
						BuildCode("(.(.a.*.(.a.+.(.a.-.a.).).).-.(.(.a.-.(.a.-.a.).).*.(.a.+.(.a.+.a.).).).)".Split('.'))
					)
				};
			}
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Just (", 
						BuildCode(new string[]{ "(" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple add - fail", 
						BuildCode(new string[]{ "(", "a", "+", ")" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple substract - missing bracket", 
						BuildCode(new string[]{ "(", "a", "-", "a" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Simple multiply - multiple a", 
						BuildCode(new string[]{ "(", "a", "*", "a", "a", ")" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Three Ops 1 - one missing", 
						BuildCode(new string[]{"(","(","a","*","a",")","(","a","-","a",")",")"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"Three Ops 2 - missing bracket", 
						BuildCode(new string[]{"(","a","*","(","a","+","(","a","-","a",")", ")"})
					)
				};
			}
		}

		#endregion
	}
}

