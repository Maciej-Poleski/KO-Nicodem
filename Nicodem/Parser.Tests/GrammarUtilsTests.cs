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
		ISet<StringSymbol> nullable;
		IDictionary<StringSymbol, ISet<StringSymbol>> first;
        FirstSet<StringSymbol> firstSet;
		IDictionary<StringSymbol, ISet<StringSymbol>> follow;
        IDictionary<StringSymbol, Dfa<StringSymbol>> automatons; 
        ISet<StringSymbol> nullable_lr;
        IDictionary<StringSymbol, Dfa<StringSymbol>> automatons_lr;
        ISet<StringSymbol> nullable_lr_nullable;
        IDictionary<StringSymbol, Dfa<StringSymbol>> automatons_lr_nullable;


		static ISet<StringSymbol> CreateSet( params StringSymbol[] args )
		{
			return new HashSet<StringSymbol>(args);
		}

		static Dfa<StringSymbol> CreateDummyAcceptingDfa() 
		{
			var a = new DfaState<StringSymbol> ();
			var aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			a.Initialize (0, aTransitionList);
			return new Dfa<StringSymbol> (a);
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
			var eof = new StringSymbol ("EOF");

			automatons = new Dictionary<StringSymbol, Dfa<StringSymbol>> ();

			// NULLABLE
			nullable = CreateSet ();
			nullable.Add (expr2);
			nullable.Add (term2);

			// FIRST
			first = new Dictionary<StringSymbol, ISet<StringSymbol>>();
			first [openbracket] = CreateSet (openbracket);
			first [closebracket] = CreateSet (closebracket);
			first [add] = CreateSet (add);
			first [subtract] = CreateSet (subtract);
			first [multiply] = CreateSet (multiply);
			first [divide] = CreateSet (divide);
			first [num] = CreateSet (num);
			first [name] = CreateSet (name);
			first [eof] = CreateSet (eof);
			//nonterminals
			first [goal] = CreateSet (goal, expr, term, factor, openbracket, name, num);
			first [expr] = CreateSet (expr, term, factor, openbracket, name, num);
			first [expr2] = CreateSet (expr2, add, subtract);
			first [term] = CreateSet (term, factor, openbracket, name, num);
			first [term2] = CreateSet (term2, multiply, divide);
			first [factor] = CreateSet (factor, openbracket, name, num);

            firstSet = new FirstSet<StringSymbol>(first);

			// FOLLOW
			follow = new Dictionary<StringSymbol, ISet<StringSymbol>>();
			follow [openbracket] = CreateSet (expr, term, factor, openbracket, name, num);
			follow [closebracket] = CreateSet (eof, add, subtract, multiply, divide, closebracket, term2, expr2);
			follow [add] = CreateSet (term, openbracket, num, name, factor);
			follow [subtract] = CreateSet (term, openbracket, num, name, factor);
			follow [multiply] = CreateSet (factor, openbracket, name, num);
			follow [divide] = CreateSet (factor, openbracket, name, num);
			follow [num] = follow [closebracket];
			follow [name] = follow [closebracket];
			follow [eof] = CreateSet ();
			//nonterminals
			follow [expr] = CreateSet (eof, closebracket);
			follow [expr2] = CreateSet (eof, closebracket);
			follow [term] = CreateSet (eof, add, subtract, closebracket, expr2);
			follow [term2] = CreateSet (eof, add, subtract, closebracket, expr2);
			follow [factor] = CreateSet (eof, add, subtract, multiply, divide, closebracket, term2, expr2);

			// ** Build automatons **

            /*
			// for terminals - nulls
			automatons [openbracket] = CreateDummyAcceptingDfa ();
			automatons [closebracket] = CreateDummyAcceptingDfa ();
			automatons [add] = CreateDummyAcceptingDfa ();
			automatons [subtract] = CreateDummyAcceptingDfa ();
			automatons [multiply] = CreateDummyAcceptingDfa ();
			automatons [divide] = CreateDummyAcceptingDfa ();
			automatons [num] = CreateDummyAcceptingDfa ();
			automatons [name] = CreateDummyAcceptingDfa ();
			automatons [eof] = CreateDummyAcceptingDfa ();
            */

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

			// Goal -> Expr EOF
			// (a --Expr--> b --EOF--> c)
			var a = new DfaState<StringSymbol> ();
			var b = new DfaState<StringSymbol> ();
			var c = new DfaState<StringSymbol> ();
			var aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			var bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			var cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			aTransitionList.Add (new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, b));
			bTransitionList.Add (new KeyValuePair<StringSymbol, DfaState<StringSymbol>> (eof, c));
			a.Initialize (0, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (1, cTransitionList);
			automatons [goal] = new Dfa<StringSymbol> (a);

			// Expr -> Term Expr2
			// a --Term--> b --Expr2--> c
			a = new DfaState<StringSymbol> ();
			b = new DfaState<StringSymbol> ();
			c = new DfaState<StringSymbol> ();
			aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term, b));
			bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr2, c));
			a.Initialize (0, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (1, cTransitionList);
			automatons [expr] = new Dfa<StringSymbol> (a);

			// Expr2  -> + Term Expr2
			// 		  |	- Term Expr2
			// 		  |  epsilon
			// a --'+'--> b --Term--> c --Expr2--> d
			// a --'-'--> b
			// a = accepting (because of epsilon)
			a = new DfaState<StringSymbol> ();
			b = new DfaState<StringSymbol> ();
			c = new DfaState<StringSymbol> ();
			var d = new DfaState<StringSymbol> ();
			aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			var dTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(add, b));
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(subtract, b));
			bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term, c));
			cTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr2, d));
			a.Initialize (1, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (0, cTransitionList);
			d.Initialize (1, dTransitionList);
			automatons [expr2] = new Dfa<StringSymbol> (a);

			// Term   -> Factor Term2
			// a --Factor--> b --Term2--> c
			a = new DfaState<StringSymbol> ();
			b = new DfaState<StringSymbol> ();
			c = new DfaState<StringSymbol> ();
			aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(factor, b));
			bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term2, c));
			a.Initialize (0, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (1, cTransitionList);
			automatons [term] = new Dfa<StringSymbol> (a);

			// Term2  -> * Factor Term2
			// 		  |  / Factor Term2
			// 		  |  epsilon
			// a --'*'--> b --Factor--> c --Term2--> d
			// a --'/'--> b
			// a = accepting (because of epsilon)
			a = new DfaState<StringSymbol> ();
			b = new DfaState<StringSymbol> ();
			c = new DfaState<StringSymbol> ();
			d = new DfaState<StringSymbol> ();
			aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			dTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(multiply, b));
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(divide, b));
			bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(factor, c));
			cTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term2, d));
			a.Initialize (1, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (0, cTransitionList);
			d.Initialize (1, dTransitionList);
			automatons [term2] = new Dfa<StringSymbol> (a);

			// Factor -> '(' Expr ')'
			// 		  |  num
			//        |  name
			// a --'('--> b --Expr--> c --')'--> d
			// a --num--> d
			// a --name-> d
			a = new DfaState<StringSymbol> ();
			b = new DfaState<StringSymbol> ();
			c = new DfaState<StringSymbol> ();
			d = new DfaState<StringSymbol> ();
			aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			dTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>> ();
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(openbracket, b));
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(num, d));
			aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(name, d));
			bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, c));
			cTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(closebracket, d));
			a.Initialize (0, aTransitionList);
			b.Initialize (0, bTransitionList);
			c.Initialize (0, cTransitionList);
			d.Initialize (1, dTransitionList);
			automatons [factor] = new Dfa<StringSymbol> (a);


            /*---------------------Automatons for LeftRecursion
             * Goal -> Expr '(' Expr ')'
             * Expr -> Expr2 '+' Expr
             * Expr2 -> Term '-' Term
             * Term -> Expr '*'
             */
            automatons_lr = new Dictionary<StringSymbol, Dfa<StringSymbol>>();

            //init automatons
            automatons_lr[openbracket] = CreateDummyAcceptingDfa();
            automatons_lr[closebracket] = CreateDummyAcceptingDfa();
            automatons_lr[add] = CreateDummyAcceptingDfa();
            automatons_lr[subtract] = CreateDummyAcceptingDfa();
            automatons_lr[multiply] = CreateDummyAcceptingDfa();

            //nullable
            nullable_lr = CreateSet();

            //Goal -> Expr '(' Expr ')'
            // a --Expr--> b --'('--> c --Expr--> d --')'--> e

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            d = new DfaState<StringSymbol>();
            var e = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            dTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            var eTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(openbracket, c));
            cTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, d));
            dTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(closebracket, e));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(0, cTransitionList);
            d.Initialize(0, dTransitionList);
            e.Initialize(1, eTransitionList);

            automatons_lr[goal] = new Dfa<StringSymbol>(a);

            //Expr -> Expr2 '+' Expr
            // a --Expr2--> b --'+'--> c --Expr--> d

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            d = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            dTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr2, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(add, c));
            cTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, d));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(0, cTransitionList);
            d.Initialize(1, dTransitionList);

            automatons_lr[expr] = new Dfa<StringSymbol>(a);

            //Expr2 -> Term '-' Term
            // a --Term--> b --'-'--> c --Term--> d

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            d = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            dTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(subtract, c));
            cTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term, d));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(0, cTransitionList);
            d.Initialize(1, dTransitionList);

            automatons_lr[expr2] = new Dfa<StringSymbol>(a);

            //Term -> Expr '*'
            // a --Expr--> b --'*'--> c

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(multiply, c));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(1, cTransitionList);

            automatons_lr[term] = new Dfa<StringSymbol>(a);

            /*---------------------Automatons for LeftRecursion with Nullable edges
             * Goal -> Expr 
             *         | Expr2
             * Expr -> Term '-'
             * Expr2 -> Term2 '+'
             * Term -> Factor Expr2
             * Term2 -> Term '*'
             * Factor -> '('Expr')'
             *         | epsilon
             */
            automatons_lr_nullable = new Dictionary<StringSymbol, Dfa<StringSymbol>>();

            //init automatons
            automatons_lr_nullable[openbracket] = CreateDummyAcceptingDfa();
            automatons_lr_nullable[closebracket] = CreateDummyAcceptingDfa();
            automatons_lr_nullable[add] = CreateDummyAcceptingDfa();
            automatons_lr_nullable[subtract] = CreateDummyAcceptingDfa();
            automatons_lr_nullable[multiply] = CreateDummyAcceptingDfa();

            //nullable
            nullable_lr_nullable = CreateSet();
            nullable_lr_nullable.Add(factor);

            // Goal -> Expr 
            //         | Expr2
            // a --Expr--> b 
            // a --Expr2--> c

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, b));
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr2, c));
            a.Initialize(0, aTransitionList);
            b.Initialize(1, bTransitionList);
            c.Initialize(1, cTransitionList);

            automatons_lr_nullable[goal] = new Dfa<StringSymbol>(a);

            // Expr -> Term '-'
            // a --Term--> b --'-'--> c

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(subtract, c));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(1, cTransitionList);

            automatons_lr_nullable[expr] = new Dfa<StringSymbol>(a);

            // Expr2 -> Term2 '+'
            // a --Term2--> b --'+'--> c

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term2, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(add, c));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(1, cTransitionList);

            automatons_lr_nullable[expr2] = new Dfa<StringSymbol>(a);

            // Term -> Factor Expr2
            // a --Factor--> b --Expr2--> c

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(factor, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr2, c));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(1, cTransitionList);

            automatons_lr_nullable[term] = new Dfa<StringSymbol>(a);

            // Term2 -> Term '*'
            // a --Term--> b --'*'--> c

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(term, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(multiply, c));
            a.Initialize(0, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(1, cTransitionList);

            automatons_lr_nullable[term2] = new Dfa<StringSymbol>(a);

            // Factor -> '('Expr')'
            //         | epsilon
            // a --'('--> b --Expr--> c --')'--> d
            // a accepting

            a = new DfaState<StringSymbol>();
            b = new DfaState<StringSymbol>();
            c = new DfaState<StringSymbol>();
            d = new DfaState<StringSymbol>();
            aTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            bTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            cTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            dTransitionList = new List<KeyValuePair<StringSymbol, DfaState<StringSymbol>>>();
            aTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(openbracket, b));
            bTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(expr, c));
            cTransitionList.Add(new KeyValuePair<StringSymbol, DfaState<StringSymbol>>(closebracket, d));
            a.Initialize(1, aTransitionList);
            b.Initialize(0, bTransitionList);
            c.Initialize(0, cTransitionList);
            d.Initialize(1, dTransitionList);

            automatons_lr_nullable[factor] = new Dfa<StringSymbol>(a);
		}

		[Test]
		public void testNullable()
		{
			var computedNullable = GrammarUtils<StringSymbol>.ComputeNullable (automatons);
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
			var computedFirst = GrammarUtils<StringSymbol>.ComputeFirst (automatons, nullable);
            //Assert.AreEqual (first.Keys.Count, computedFirst.Keys.Count);
			foreach (var key in first.Keys) {
				//Assert.IsTrue (computedFirst.ContainsKey (key));
				var set1 = first [key];
				var set2 = computedFirst [key];
				Assert.IsTrue (set1.SetEquals (set2));
			}
		}

		[Test]
		public void testFollow()
		{
			var computedFollow = GrammarUtils<StringSymbol>.ComputeFollow (automatons, nullable, firstSet);
			Assert.AreEqual (follow.Keys.Count, computedFollow.Keys.Count);
			foreach (var key in follow.Keys) {
				Assert.IsTrue (computedFollow.ContainsKey (key));
				var set1 = follow [key];
				var set2 = computedFollow [key];
				Assert.IsTrue (set1.SetEquals (set2));
			}
		}

		[Test]
		public void testTransitiveComplement_DiagonalRelation()
		{
			var relation = new Dictionary<int, ISet<int>> ();
			for (int i = 0; i < 4; ++i)
				relation.Add (i, new HashSet<int> { i });

			GrammarUtils<StringSymbol>.ComputeTransitiveComplement (relation);

			Assert.AreEqual (4, relation.Keys.Count);
			for (int i = 0; i < 4; ++i)
				Assert.IsTrue (relation [i].SetEquals (new HashSet<int>{ i }));
		}

		[Test]
		public void testTransitiveComplement_UpperTriangleRelation()
		{
			var relation = new Dictionary<int, ISet<int>> ();

			// 0 > 1, 1 > 2, 2 > 3, 3 > 4
			for (int i = 0; i < 4; ++i)
				relation.Add (i, new HashSet<int> { i + 1 });
			relation.Add (4, new HashSet<int> ());

			GrammarUtils<StringSymbol>.ComputeTransitiveComplement (relation);

			Assert.AreEqual (5, relation.Keys.Count);
			for (int i = 0; i < 5; ++i) {
				var s = new HashSet<int> ();
				for (int j = i + 1; j < 5; ++j)
					s.Add (j);
				Assert.IsTrue (relation [i].SetEquals (s));
			}
		}

		[Test]
		public void testTransitiveComplement_FullRelation()
		{
			var relation = new Dictionary<int, ISet<int>> ();

			// 0 > 1, 1 > 2, 2 > 3, 3 > 4, 4 > 0
			for (int i = 0; i < 5; ++i)
				relation.Add (i, new HashSet<int> { (i + 1) % 5 });

			GrammarUtils<StringSymbol>.ComputeTransitiveComplement (relation);

			Assert.AreEqual (5, relation.Keys.Count);
			var s = new HashSet<int> { 0, 1, 2, 3, 4 };
			for (int i = 0; i < 5; ++i)
				Assert.IsTrue (relation [i].SetEquals (s));
		}

		[Test]
		public void testTransitiveComplement_EmptyRelation()
		{
			var relation = new Dictionary<int, ISet<int>> ();

			for (int i = 0; i < 5; ++i)
				relation.Add (i, new HashSet<int> ());

			GrammarUtils<StringSymbol>.ComputeTransitiveComplement (relation);

			Assert.AreEqual (5, relation.Keys.Count);
			for (int i = 0; i < 5; ++i)
				Assert.IsTrue (relation [i].SetEquals (new HashSet<int> ()));
		}

		[Test]
		public void testTransitiveComplement_3ClassesRelation()
		{
			var relation = new Dictionary<int, ISet<int>> ();

			// first class { 0, 1 }
			relation.Add (0, new HashSet<int> { 1 });
			relation.Add (1, new HashSet<int> { 0 });

			// second class { 2, 3, 4, 5 }
			relation.Add (2, new HashSet<int> { 5 });
			relation.Add (3, new HashSet<int> { 4 });
			relation.Add (4, new HashSet<int> { 2 });
			relation.Add (5, new HashSet<int> { 3 });

			// third class { 6 }
			relation.Add (6, new HashSet<int> { 6 });

			GrammarUtils<StringSymbol>.ComputeTransitiveComplement (relation);

			Assert.AreEqual (7, relation.Keys.Count);

			Assert.IsTrue (relation [0].SetEquals (new HashSet<int> { 0, 1 }));
			Assert.IsTrue (relation [1].SetEquals (new HashSet<int> { 0, 1 }));

			Assert.IsTrue (relation [2].SetEquals (new HashSet<int> { 2, 3, 4, 5 }));
			Assert.IsTrue (relation [3].SetEquals (new HashSet<int> { 2, 3, 4, 5 }));
			Assert.IsTrue (relation [4].SetEquals (new HashSet<int> { 2, 3, 4, 5 }));
			Assert.IsTrue (relation [5].SetEquals (new HashSet<int> { 2, 3, 4, 5 }));

			Assert.IsTrue (relation [6].SetEquals (new HashSet<int> { 6 }));
		}

		[Test]
		public void testTransitiveComplement_MixedRelation()
		{
			var relation = new Dictionary<int, ISet<int>> ();
			relation.Add (0, new HashSet<int> { 1, 3, 4 });
			relation.Add (1, new HashSet<int> ());
			relation.Add (2, new HashSet<int> { 0 });
			relation.Add (3, new HashSet<int> ());
			relation.Add (4, new HashSet<int> { 2 });

			GrammarUtils<StringSymbol>.ComputeTransitiveComplement (relation);

			Assert.AreEqual (5, relation.Keys.Count);

			Assert.IsTrue (relation [0].SetEquals (new HashSet<int> { 0, 1, 2, 3, 4 }));
			Assert.IsTrue (relation [1].SetEquals (new HashSet<int> ()));
			Assert.IsTrue (relation [2].SetEquals (new HashSet<int> { 0, 1, 2, 3, 4 }));
			Assert.IsTrue (relation [3].SetEquals (new HashSet<int> ()));
			Assert.IsTrue (relation [4].SetEquals (new HashSet<int> { 0, 1, 2, 3, 4 }));
		}

        [Test]
        public void hasLeftRecursionNoRecursionTest()
        {
            Assert.IsFalse(GrammarUtils<StringSymbol>.HasLeftRecursion(automatons, nullable));
        }

        [Test]
        public void hasLeftRecursionHasRecursionTest()
        {
            Assert.IsTrue(GrammarUtils<StringSymbol>.HasLeftRecursion(automatons_lr, nullable_lr));
        }

        [Test]
        public void hasLeftRecursionOnNullableEdgesTest()
        {
            Assert.IsTrue(GrammarUtils<StringSymbol>.HasLeftRecursion(automatons_lr_nullable, nullable_lr_nullable));
        }
	}
}

