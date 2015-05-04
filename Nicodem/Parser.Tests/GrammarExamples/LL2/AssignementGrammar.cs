using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class AssignementGrammar : GrammarExample
	{
		public AssignementGrammar()
			: base(
				"S",
				new Tuple<string, string[]>[] {
					new Tuple<string, string[]>("S", new string[]{ "Assign", "$" }),
					new Tuple<string, string[]>("S", new string[]{ "Inc", "$" }),
					new Tuple<string, string[]>("Assign", new string[]{ "Lv", "=", "Rv" }),
					new Tuple<string, string[]>("Inc", new string[]{ "Lv", "++" }),
					new Tuple<string, string[]>("Rv", new string[]{ "Lv" }),
					new Tuple<string, string[]>("Rv", new string[]{ "num" }),
					new Tuple<string, string[]>("Lv", new string[]{ "id" }),
				}
			)
		{
		}

		#region GrammarExample implementation

		public override GrammarType Type
		{
			get { return GrammarType.LL2; }
		}

		public override string Name
		{
			get { return "Simple Syntax"; }
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] ValidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id=id", 
						BuildCode(new string[]{ "id", "=", "id"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id=num", 
						BuildCode(new string[]{ "id", "=", "num" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id++", 
						BuildCode(new string[]{ "id", "++" })
					)
				};
			}
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
				};
			}
		}

		#endregion
	}
}

