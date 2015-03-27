using Nicodem.Lexer;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections.Immutable;
using System;

namespace Nicodem.Parser.Tests
{
	[TestFixture]
	public class GrammarUtilsTests
	{
		ISet<ISymbol> nullable;
		IDictionary<ISymbol, ISet<ISymbol>> first;
		IDictionary<ISymbol, ISet<ISymbol>> follow;
		IDictionary<ISymbol, Dfa<ISymbol>> automatons;

		static ISet<ISymbol> CreateSet( params ISymbol[] args )
		{
			return new HashSet<ISymbol>(args);
		}

		static Dfa<ISymbol> CreateDummyAcceptingDfa() 
		{
			var a = new DfaState<ISymbol> ();
			var aTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			a.Initialize (0, aTransitionList);
			return new Dfa<ISymbol> (a);
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

			automatons = new Dictionary<ISymbol, Dfa<ISymbol>> ();

			// ** Prepare correct nullable, first and follow sets ** 

			// NULLABLE
			// TODO(Jakub Brzeski)
			nullable = CreateSet ();
			nullable.Add (expr2);
			nullable.Add (term2);


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
			first [goal] = first [expr];

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

			// for terminals - nulls
			automatons [openbracket] = CreateDummyAcceptingDfa ();
			automatons [closebracket] = CreateDummyAcceptingDfa ();
			automatons [add] = CreateDummyAcceptingDfa ();
			automatons [subtract] = CreateDummyAcceptingDfa ();
			automatons [multiply] = CreateDummyAcceptingDfa ();
			automatons [divide] = CreateDummyAcceptingDfa ();
			automatons [num] = CreateDummyAcceptingDfa ();
			automatons [name] = CreateDummyAcceptingDfa ();
			automatons [epsi] = CreateDummyAcceptingDfa ();
			automatons [eof] = CreateDummyAcceptingDfa ();

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

			// Goal -> Expr
			// (a --Expr--> b)
			var a = new DfaState<ISymbol> ();
			var b = new DfaState<ISymbol> ();
			var aTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			var bTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			aTransitionList.Add (new KeyValuePair<ISymbol, DfaState<ISymbol>>(expr, b));
			a.Initialize (0, aTransitionList);
			b.Initialize (1, bTransitionList);
			automatons [goal] = new Dfa<ISymbol> (a);

			// Expr -> Term Expr2
			// a --Term--> b --Expr2--> c
			a = new DfaState<ISymbol> ();
			b = new DfaState<ISymbol> ();
			var c = new DfaState<ISymbol> ();
			aTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			bTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			var cTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(term, b));
			bTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(expr2, c));
			a.Initialize (0, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (1, cTransitionList);
			automatons [expr] = new Dfa<ISymbol> (a);

			// Expr2  -> + Term Expr2
			// 		  |	- Term Expr2
			// 		  |  epsilon
			// a --'+'--> b --Term--> c --Expr2--> d
			// a --'-'--> b
			// a = accepting (because of epsilon)
			a = new DfaState<ISymbol> ();
			b = new DfaState<ISymbol> ();
			c = new DfaState<ISymbol> ();
			var d = new DfaState<ISymbol> ();
			aTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			bTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			cTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			var dTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(add, b));
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(subtract, b));
			bTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(term, c));
			cTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(expr2, d));
			a.Initialize (1, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (0, cTransitionList);
			d.Initialize (1, dTransitionList);
			automatons [expr2] = new Dfa<ISymbol> (a);

			// Term   -> Factor Term2
			// a --Factor--> b --Term2--> c
			a = new DfaState<ISymbol> ();
			b = new DfaState<ISymbol> ();
			c = new DfaState<ISymbol> ();
			aTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			bTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			cTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(factor, b));
			bTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(term2, c));
			a.Initialize (0, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (1, cTransitionList);
			automatons [term] = new Dfa<ISymbol> (a);

			// Term2  -> * Factor Term2
			// 		  |  / Factor Term2
			// 		  |  epsilon
			// a --'*'--> b --Factor--> c --Term2--> d
			// a --'/'--> b
			// a = accepting (because of epsilon)
			a = new DfaState<ISymbol> ();
			b = new DfaState<ISymbol> ();
			c = new DfaState<ISymbol> ();
			d = new DfaState<ISymbol> ();
			aTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			bTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			cTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			dTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(multiply, b));
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(divide, b));
			bTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(factor, c));
			cTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(term2, d));
			a.Initialize (1, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (0, cTransitionList);
			d.Initialize (1, dTransitionList);
			automatons [term2] = new Dfa<ISymbol> (a);

			// Factor -> '(' Expr ')'
			// 		  |  num
			//        |  name
			// a --'('--> b --Expr--> c --')'--> d
			// a --num--> d
			// a --name-> d
			a = new DfaState<ISymbol> ();
			b = new DfaState<ISymbol> ();
			c = new DfaState<ISymbol> ();
			d = new DfaState<ISymbol> ();
			aTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			bTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			cTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			dTransitionList = new List<KeyValuePair<ISymbol, DfaState<ISymbol>>> ();
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(openbracket, b));
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(num, d));
			aTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(name, d));
			bTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(expr, c));
			cTransitionList.Add(new KeyValuePair<ISymbol, DfaState<ISymbol>>(closebracket, d));
			a.Initialize (0, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (0, cTransitionList);
			d.Initialize (1, dTransitionList);
			automatons [factor] = new Dfa<ISymbol> (a);

		}

		[Test]
		public void testNullable()
		{
			var computedNullable = GrammarUtils.ComputeNullable (automatons);
			foreach (var symbol in computedNullable) {
				Console.Write (((StringSymbol)symbol).S);
			}
			Assert.IsTrue (nullable.SetEquals (computedNullable));
		}

		static void writeSet<T>( ISet<T> set )
		{
			Console.Write ("[ ");
			foreach (var s in set)
				Console.Write (s + " ");
			Console.WriteLine ("]");
		}

		[Test]
		public void testFirst()
		{
			var computedFirst = GrammarUtils.ComputeFirst (automatons, nullable);
			Assert.AreEqual (first.Keys.Count, computedFirst.Keys.Count);

			foreach (var key in first.Keys) {

				Assert.IsTrue (computedFirst.ContainsKey (key));

				var set1 = first [key];
				var set2 = computedFirst [key];
				writeSet (set1);
				writeSet (set2);
				Console.WriteLine ();
				Assert.IsTrue (set1.SetEquals (set2));
			}
		}

		[Test]
		public void testFollow()
		{
			var computedFollow = GrammarUtils.ComputeFollow (automatons, nullable, first);
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

