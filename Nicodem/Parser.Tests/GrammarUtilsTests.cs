using Nicodem.Lexer;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace Nicodem.Parser.Tests
{
	[TestFixture]
	public class GrammarUtilsTests
	{
		ISet<ISymbol> nullable;
		IDictionary<ISymbol, ISet<ISymbol>> first;
		IDictionary<ISymbol, ISet<ISymbol>> follow;

		static ISet<ISymbol> CreateSet( params ISymbol[] args )
		{
			return new HashSet<ISymbol>(args);
		}

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

			var goal = new StringSymbol ("Goal");
			var expr = new StringSymbol ("Expr");
			var expr2 = new StringSymbol ("Expr2");
			var term = new StringSymbol ("Term");
			var term2 = new StringSymbol ("Term2");
			var factor = new StringSymbol ("Factor");
			var openbracket = new StringSymbol ("(");
			var closebracket = new StringSymbol (")");
			var add = new StringSymbol ("+");
			var subtract = new StringSymbol ("-");
			var multiply = new StringSymbol ("*");
			var divide = new StringSymbol ("/");
			var num = new StringSymbol ("num");
			var name = new StringSymbol ("name");
			var epsi = new StringSymbol ("epsi");
			var eof = new StringSymbol ("EOF");

			// ** Prepare correct nullable, first and follow sets ** 

			// NULLABLE
			// TODO(Jakub Brzeski)
			nullable = CreateSet ();

			// FIRST
			first = new Dictionary<ISymbol, ISet<ISymbol>>();

			//terminals
			first [openbracket] = CreateSet (openbracket);
			first [closebracket] = CreateSet (closebracket);
			first [add] = CreateSet (add);
			first [subtract] = CreateSet (subtract);
			first [multiply] = CreateSet (multiply);
			first [divide] = CreateSet (divide);
			first [num] = CreateSet (num);
			first [name] = CreateSet (name);
			first [epsi] = CreateSet (epsi);
			first [eof] = CreateSet (eof);

			//nonterminals
			first [expr] = CreateSet (openbracket, name, num);
			first [expr2] = CreateSet (add, subtract, epsi);
			first [term] = CreateSet (openbracket, name, num);
			first [term2] = CreateSet (multiply, divide, epsi);
			first [factor] = CreateSet (openbracket, name, num);

			// FOLLOW
			// TODO(Jakub Brzeski)
			follow = new Dictionary<ISymbol, ISet<ISymbol>>();

			//terminals
			follow [openbracket] = CreateSet ();
			follow [closebracket] = CreateSet ();
			follow [add] = CreateSet ();
			follow [subtract] = CreateSet ();
			follow [multiply] = CreateSet ();
			follow [divide] = CreateSet ();
			follow [num] = CreateSet ();
			follow [name] = CreateSet ();
			follow [epsi] = CreateSet ();
			follow [eof] = CreateSet ();

			//nonterminals
			follow [expr] = CreateSet (eof, closebracket);
			follow [expr2] = CreateSet (eof, closebracket);
			follow [term] = CreateSet (eof, add, subtract, closebracket);
			follow [term2] = CreateSet (eof, add, subtract, closebracket);
			follow [factor] = CreateSet (eof, add, subtract, multiply, divide, closebracket);

			// ** Build automatons **
			//TODO(Jakub Brzeski & Patryk Mikos) build automatons
			// var state_goal = new DfaState<StringSymbol> ();

			// Goal -> automaton 
			// (a --Expr--> b)

			// Goal   -> Expr EOF
			// Expr   -> Term Expr2
			// Expr2  -> + Term Expr2
			// 		  |	- Term Expr2
			// 		  |  epsilon
			// Term   -> Factor Term2
			// Term2  -> * Factor Term2
			// 		  |  / Factor Term2
			// 		  |  epsilon
			// Factor -> '(' Expr ')'
			// 		  |  num
			//        |  name

			var bTransitions = new KeyValuePair<ISymbol, DFAState<ISymbol>>[0];
			var b = new DFAState<ISymbol> (1, bTransitions);
			var aTransitions = new KeyValuePair<ISymbol, DFAState<ISymbol>>[1];
			aTransitions [0] = new KeyValuePair<ISymbol, DFAState<ISymbol>>(expr, b);
			var a = new DFAState<ISymbol> (0, aTransitions);
		}

		[Test]
		public void testNullable()
		{
			//TODO(set automatons)
			var computedNullable = GrammarUtils.ComputeNullable (null);
			Assert.IsTrue (nullable.SetEquals (computedNullable));
		}

		[Test]
		public void testFirst()
		{
			//TODO(set automatons)
			var computedFirst = GrammarUtils.ComputeFirst (null, nullable);
			Assert.AreEqual (first.Keys.Count, computedFirst.Keys.Count);

			foreach (var key in first.Keys) {

				Assert.IsTrue (computedFirst.ContainsKey (key));

				var set1 = first [key];
				var set2 = computedFirst [key];
				Assert.IsTrue (set1.SetEquals (set2));
			}
		}

		[Test]
		public void testFollow()
		{
			//TODO(set automatons)
			var computedFollow = GrammarUtils.ComputeFollow (null, nullable, first);
			Assert.AreEqual (follow.Keys.Count, computedFollow.Keys.Count);

			foreach (var key in follow.Keys) {

				Assert.IsTrue (computedFollow.ContainsKey (key));

				var set1 = follow [key];
				var set2 = computedFollow [key];
				Assert.IsTrue (set1.SetEquals (set2));
			}
		}
	}
}

