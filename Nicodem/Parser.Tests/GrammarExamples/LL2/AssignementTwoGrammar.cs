using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class AssignementTwoGrammar : GrammarExample
	{
		public AssignementTwoGrammar()
			: base(
				"S'",
				new Tuple<string, string[]>[] {
					new Tuple<string, string[]>("S'", new string[]{ "S", "$" }),
					new Tuple<string, string[]>("S", new string[]{ "L" }),
					new Tuple<string, string[]>("S", new string[]{ "L", "=", "L" }),
					new Tuple<string, string[]>("L", new string[]{ "*", "L" }),
					new Tuple<string, string[]>("L", new string[]{ "id" }),
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
			get { return "Ambigous Two"; }
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
						"*id=id", 
						BuildCode(new string[]{ "*", "id", "=", "id" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id", 
						BuildCode(new string[]{ "id" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"*id", 
						BuildCode(new string[]{ "*", "id" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"*id=*id", 
						BuildCode(new string[]{ "*", "id", "=", "*", "id" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id=*id", 
						BuildCode(new string[]{ "id", "=", "*", "id" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"**id", 
						BuildCode(new string[]{ "*", "*", "id"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"**id=id", 
						BuildCode(new string[]{ "*", "*", "id", "=", "id"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"***id=****id", 
						BuildCode(new string[]{ "*", "*", "*", "id", "=", "*", "*", "*", "*", "id"})
					),
				};
			}
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {
				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"id*=*id", 
						BuildCode(new string[]{ "id", "*", "=", "*", "id" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"**idid", 
						BuildCode(new string[]{ "*", "*", "id", "id"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"**id*id", 
						BuildCode(new string[]{ "*", "*", "id", "*", "id"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"***id=**id*id", 
						BuildCode(new string[]{ "*", "*", "*", "id", "=", "*", "*", "id", "*", "id"})
					),
				};
			}
		}

		#endregion
	}
}

