using System;
using Nicodem.Lexer;
using NUnit.Framework;
using System.Collections.Generic;

namespace Nicodem.Parser.Tests
{
	[TestFixture]
	public class GrammarUtilsTests
	{
		ISet<ISymbol> nullable;
		IDictionary<ISymbol, ISet<ISymbol>> first;
		IDictionary<ISymbol, ISet<ISymbol>> follow;

		[TestFixtureSetUp]
		public void initAutomatons(){
			/*
			 * Goal   -> Expr EOF
			 * Expr   -> Term Expr2
			 * Expr2  -> + Term Expr2
			 * 		  |	- Term Expr2
			 * 		  |  epsilon
			 * Term   -> Factor Term2
			 * Term2  -> * Factor Term2
			 * 		  |  / Factor Term2
			 * 		  |  epsilon
			 * Factor -> '(' Expr ')'
			 * 		  |  num
			 *        |  name
			*/

			StringSymbol goal = new StringSymbol ("Goal");
			StringSymbol expr = new StringSymbol ("Expr");
			StringSymbol expr2 = new StringSymbol ("Expr2");
			StringSymbol term = new StringSymbol ("Term");
			StringSymbol term2 = new StringSymbol ("Term2");
			StringSymbol factor = new StringSymbol ("Factor");
			StringSymbol openbracket = new StringSymbol ("(");
			StringSymbol closebracket = new StringSymbol (")");
			StringSymbol add = new StringSymbol ("+");
			StringSymbol subtract = new StringSymbol ("-");
			StringSymbol multiply = new StringSymbol ("*");
			StringSymbol divide = new StringSymbol ("/");
			StringSymbol num = new StringSymbol ("num");
			StringSymbol name = new StringSymbol ("name");
			StringSymbol epsi = new StringSymbol ("epsi");
			StringSymbol eof = new StringSymbol ("EOF");

			// ** Prepare correct nullable, first and follow sets ** 

				// NULLABLE
				// TODO(Jakub Brzeski)

				// FIRST
				first = new Dictionary<ISymbol, ISet<ISymbol>>();
				//terminals
				first [openbracket] = new HashSet<ISymbol> (new ISymbol[]{ openbracket });
				first [closebracket] = new HashSet<ISymbol> (new ISymbol[]{ closebracket }); 
				first [add] = new HashSet<ISymbol> (new ISymbol[]{ add }); 
				first [subtract] = new HashSet<ISymbol> (new ISymbol[]{ subtract }); 
				first [multiply] = new HashSet<ISymbol> (new ISymbol[]{ multiply }); 
				first [divide] = new HashSet<ISymbol> (new ISymbol[]{ divide });
				first [num] = new HashSet<ISymbol> (new ISymbol[]{ num }); 
				first [name] = new HashSet<ISymbol> (new ISymbol[]{ name }); 
				first [epsi] = new HashSet<ISymbol> (new ISymbol[]{ epsi });
				first [eof] = new HashSet<ISymbol> (new ISymbol[]{ eof }); 
				//nonterminals
				first [expr] = new HashSet<ISymbol> (new ISymbol[]{ openbracket, name, num });
				first [expr2] = new HashSet<ISymbol> (new ISymbol[]{ add, subtract, epsi });
				first [term] = new HashSet<ISymbol> (new ISymbol[]{ openbracket, name, num });
				first [term2] = new HashSet<ISymbol> (new ISymbol[]{ multiply, divide, epsi }); 
				first [factor] = new HashSet<ISymbol> (new ISymbol[]{ openbracket, name, num });

				// FOLLOW
				// TODO(Jakub Brzeski)


			// ** Build automatons **
			//TODO(Jakub Brzeski & Patryk Mikos) build automatons

			// Goal -> automaton 
			// (a --Expr--> b)

			var bTransitions = new KeyValuePair<ISymbol, DFAState<ISymbol>>[0];
			var b = new DFAState<ISymbol> (1, bTransitions);
			var aTransitions = new KeyValuePair<ISymbol, DFAState<ISymbol>>[1];
			aTransitions [0] = new KeyValuePair<ISymbol, DFAState<ISymbol>>(expr, b);
			var a = new DFAState<ISymbol> (0, aTransitions);



		}

		[Test]
		public void testNullable()
		{
			//TODO(Jakub Brzeski & Patryk Mikos)
		}

		[Test]
		public void testFirst()
		{
			//TODO(Jakub Brzeski & Patryk Mikos)
		}

		[Test]
		public void testFollow()
		{
			//TODO(Jakub Brzeski & Patryk Mikos)
		}
	}
}

