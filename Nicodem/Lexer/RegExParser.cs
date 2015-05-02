using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{

	public class ParseError : Exception
	{
		public ParseError (String msg) : base (msg)
		{
		}
	}

	public static class RegExParser
	{

		private static String regEx;
		private static int head;
		private static readonly String specialCharacters = "()[].*+&|^-\\";

		public static RegEx<char> Parse (String regEx)
		{
			RegExParser.head = 0;
			RegExParser.regEx = regEx;

			var result = ParseBinaryOperators ();
			if (HasNext ())
				throw new ParseError ("Cannot parse the whole input string.");

			return Sanitize (result);
		}

		private static RegEx<char> Sanitize (RegEx<char> result)
		{
			return RegExFactory.Intersection (result, RegExFactory.Star (CharactersClasses.print));
		}

		private static RegEx<char> SingleChar (char c)
		{
			return RegExFactory.Range (c, (char)(c + 1));
		}

		private static char Peek ()
		{
			if (!HasNext ())
				throw new ParseError ("Peek error.");
			return regEx [head++];
		}

		private static bool HasNext ()
		{
			return (head < regEx.Length);
		}

		private static void Accept (String s)
		{
			if (!HasPrefix (s))
				throw new ParseError (s + " expected, but not found.");
			Eat (s.Length);
		}

		private static void Eat (int howMany)
		{
			if (head + howMany > regEx.Length)
				throw new ParseError ("Eat error.");
			head += howMany;
		}

		private static bool HasPrefix (String s)
		{
			if (head + s.Length > regEx.Length)
				return false;
			return regEx.Substring (head, s.Length).Equals (s);
		}

		private static RegEx<char> ParseBinaryOperators ()
		{
			var left = ParseConcat ();
			bool union = false, intersection = false;

			union |= HasPrefix ("|");
			intersection |= HasPrefix ("&");

			if (union || intersection) {
				Eat (1);
				var right = ParseBinaryOperators ();

				if (right == null)
					throw new ParseError ("Missed one of the union/intersection argument.");

				return (union ? RegExFactory.Union (left, right) : RegExFactory.Intersection (left, right));
			}

			return left;
		}

		private static RegEx<char> ParseConcat ()
		{
			var left = ParseStar ();

			if (left == null)
				return null;

			var right = ParseConcat ();

			if (right != null)
				return RegExFactory.Concat (left, right);

			return left;
		}

		private static RegEx<char> ParseStar ()
		{
			var left = ParseAtom ();

			if (HasPrefix ("*") || HasPrefix ("+")) {
				bool star = false;

				while (HasPrefix ("*") || HasPrefix ("+")) {
					star |= HasPrefix ("+");
					Eat (1);
				}

				if (left == null)
					throw new ParseError ("Unassigned +/-.");

				return (star ? RegExFactory.Concat (left, RegExFactory.Star (left)) : RegExFactory.Star (left));
			} 

			return left;
		}

		private static RegEx<char> ParseAtom ()
		{
			if (HasPrefix ("[")) {
				Eat (1);

				bool complement = false;

				if (HasPrefix ("^")) {
					Eat (1);
					complement = true;
				}

				RegEx<char> atom = null;

				if (HasPrefix (":")) {	
					if (HasPrefix (":digit:"))
						atom = CharactersClasses.digit;
					if (HasPrefix (":print:"))
						atom = CharactersClasses.print;
					if (HasPrefix (":space:"))
						atom = CharactersClasses.space;
					if (atom == null)
						throw new ParseError ("Invalid class name.");
					Eat (7);
				}

				if (atom == null) {
					List<RegEx<char>> chars = new List<RegEx<char>> ();
					char ch;
					while ((ch = Peek ()) != ']') {
						chars.Add (SingleChar (ch));
					}
					atom = RegExFactory.Union (chars.ToArray ());
				} else
					Accept ("]");

				if (complement)
					return RegExFactory.Intersection (RegExFactory.Range ((char)0), RegExFactory.Complement (atom));
				else
					return atom;
			}

			if (HasPrefix (".")) {
				Eat (1);
				return RegExFactory.Range ((char)0);
			}

			if (HasPrefix ("(")) {
				Eat (1);
				var node = ParseBinaryOperators ();
				if (node == null)
					throw new ParseError ("Parentheses around the null expression.");
				Accept (")");
				return node;
			}

			if (HasPrefix ("^")) {
				Eat (1);
				var node = ParseAtom ();
				if (node == null)
					throw new ParseError ("Unassigned ^.");
				return RegExFactory.Complement (node);
			}

			if (HasPrefix ("\\")) {
				Eat (1);
				char a = Peek ();

				if (!specialCharacters.Contains ("" + a))
					throw new ParseError ("Special character required.");
				return SingleChar (a);
			}

			if (HasNext ()) {
				var a = Peek ();
				if (specialCharacters.Contains ("" + a)) { 
					--head;
					return null;
				}
				return SingleChar (a);
			}

			return null;
		}
	}
}
    