using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class AddSeqGrammar : GrammarExample
	{
		public AddSeqGrammar()
			: base(
				"Prog",
				new Tuple<string, string[]>[] {
					new Tuple<string, string[]>("Prog", new string[]{ "A", "$" }),
					new Tuple<string, string[]>("A", new string[]{ "id" }),
					new Tuple<string, string[]>("A", new string[]{ "id", "Etail" }),
					new Tuple<string, string[]>("Etail", new string[]{ "+", "A" }),
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
			get { return "Add Sequence"; }
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] ValidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id", 
						BuildCode(new string[]{ "id" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id+id", 
						BuildCode(new string[]{ "id", "+", "id"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id+id+id", 
						BuildCode(new string[]{ "id", "+", "id", "+", "id"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id+id+id+id+id", 
						BuildCode(new string[]{ "id", "+", "id", "+", "id", "+", "id"})
					),
				};
			}
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id+id+id+id+", 
						BuildCode(new string[]{ "id", "+", "id", "+", "id", "+"})
					)
				};
			}
		}

		#endregion
	}
}

