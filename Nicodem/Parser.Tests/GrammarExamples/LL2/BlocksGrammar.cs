using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class BlocksGrammar : GrammarExample
	{
		public BlocksGrammar()
			: base(
				"S",
				new Tuple<string, string[]>[] {
					new Tuple<string, string[]>("S", new string[]{ "Block", "$" }),
					new Tuple<string, string[]>("S", new string[]{ "(", ")", "$" }),
					new Tuple<string, string[]>("Block", new string[]{ "(", "stmt", ")" })
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
			get { return "Blocks grammar"; }
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] ValidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"()", 
						BuildCode(new string[]{ "(", ")"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"(stmt)", 
						BuildCode(new string[]{ "(", "stmt", ")"})
					)
				};
			}
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"))", 
						BuildCode(new string[]{ ")", ")"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"(stmt", 
						BuildCode(new string[]{ "(", "stmt" })
					)
				};
			}
		}

		#endregion
	}
}

