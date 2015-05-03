using System;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	internal class SimpleSyntax : GrammarExample
	{
		public SimpleSyntax()
			: base(
				"Prog",
				new Tuple<string, string[]>[] {
					new Tuple<string, string[]>("Prog", new string[]{ "{", "Stmts", "}", "$" }),
					new Tuple<string, string[]>("Prog", new string[]{ "Stmts", "$" }),
					new Tuple<string, string[]>("Stmts", new string[]{ "Stmt", "Stmts" }),
					new Tuple<string, string[]>("Stmts", new string[]{ "Stmt" }),
					new Tuple<string, string[]>("Stmt", new string[]{ "{", "Stmt", "}" }),
					new Tuple<string, string[]>("Stmt", new string[]{ "id", "=", "Expr", ";" }),
					new Tuple<string, string[]>("Stmt", new string[]{ "if", "(", "Expr", ")", "Stmt" }),
					new Tuple<string, string[]>("Expr", new string[]{ "id", "Etail" }),
					new Tuple<string, string[]>("Expr", new string[]{ "id" }),
					new Tuple<string, string[]>("Expr", new string[]{ "{", "Expr", "}" }),
					new Tuple<string, string[]>("Etail", new string[]{ "+", "Expr" }),
					new Tuple<string, string[]>("Etail", new string[]{ "-", "Expr" })
				}
			)
		{
		}

		#region GrammarExample implementation

		public override GrammarType Type
		{
			get { return GrammarType.OTHER; }
		}

		public override string Name
		{
			get { return "Simple Syntax"; }
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] ValidPrograms
		{
			get {
				var prog1 = "{ { { id = id - id ; } } }";
				var prog2 = 
					"{ " +
						"id = { { id + { id } } } ; " +
						"if ( { id - { id } } ) " +
							"if ( id - id ) { id = id ; } " +
						"{ id = id - id + id - id + id - { { { id } } } ; } " + 
					"}";

				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"{ id = { { id } }; }", 
						BuildCode(new string[]{ "{", "id", "=", "{", "{", "id", "}", "}", ";", "}"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"{ id = { id  + id }; }", 
						BuildCode(new string[]{ "{", "id", "=", "{", "id", "+", "id", "}", ";", "}" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"{ id = id + id; id = id + id; }", 
						BuildCode(new string[]{ "{", "id", "=", "id", "+", "id", ";", "id", "=", "id", "+", "id", ";", "}" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"{ id = id + id; id = id - id; id = id; }", 
						BuildCode(new string[]{ "{", "id", "=", "id", "+", "id", ";", "id", "=", "id", "-", "id", ";", "id", "=", "id", ";" ,"}" })
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"{ if ( id  ) id = { id }; }", 
						BuildCode(new string[]{ "{", "if", "(", "id", ")", "id", "=", "{", "id", "}", ";", "}"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"{ " +
							"if ( id  ) id = id; " +
							"id = id + id + id; " +
							"{ if ( { id - id }  ) { id = id - id; } }" +
						"}", 
						BuildCode(new string[]{ 
							"{", 
								"if", "(", "id", ")", "id", "=", "id", ";", 
								"id", "=", "id", "+", "id", "+", "id", ";",	
								"{", "if", "(", "{", "id", "-", "id", "}", ")", "{", "id", "=", "id", "-", "id", ";", "}", "}", 
							"}"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						"{ if ( id  ) if ( id ) if ( id ) = id; " +
						"if (id + id) id = id - id; }", 
						BuildCode(new string[]{ "{", 
								"if", "(", "id", ")", "if", "(", "id", ")", "if", "(", "id", ")", "id", "=", "id", ";", 
								"if", "(", "id", "+", "id", ")", "id", "=", "id", "-", "id", ";", 
							"}"})
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog1,
						BuildCode(prog1.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog2,
						BuildCode(prog2.Split())
					)
				};
			}
		}

		public override Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] InvalidPrograms
		{
			get {

				var prog1 = "{ { { id = id - id } ; } }";
				var prog2 = "{ { { id = id - ; } } }";
				var prog3 = "{ { { id = id - id } } }";
				var prog4 = "{ { { id = { id } - id ; } } }";
				var prog5 = "{ { { id = id - id ; } }";
				var prog6 = 
					"{ " +
						"id = { { id + { id } } } ; " +
						"if ( { id - { id } } ) " +
						"if ( id - id ; ) { id = id ; } " +
						"{ id = id - id + id - id + id - { { { id } } } ; } " + 
					"}";
				var prog7 = 
					"{ " +
						"id = { { id + { id } } } ; " +
						"if ( { id - { id } } ) " +
						"if id - id ) { id = id ; } " +
						"{ id = id - id + id - id + id - { { { id } } } ; } " + 
					"}";

				var prog8 = 
					"{ " +
						"id = { { id + { id } } } ; " +
						"if ( { id - { id } } ) " +
						"if ( id - id ) { id = id ; } " +
						"{ id = id - id + id - id + id { { { id } } } ; } " + 
					"}";

				return new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>[] {
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog1,
						BuildCode(prog1.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog2,
						BuildCode(prog2.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog2,
						BuildCode(prog2.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog3,
						BuildCode(prog3.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog4,
						BuildCode(prog4.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog5,
						BuildCode(prog5.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog6,
						BuildCode(prog6.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog7,
						BuildCode(prog7.Split())
					),
					new Tuple<String, IEnumerable<ParseLeaf<CharSymbol>>>(
						prog8,
						BuildCode(prog8.Split())
					),
				};
			}
		}

		#endregion
	}
}

