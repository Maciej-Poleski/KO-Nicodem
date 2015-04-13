using System;

namespace Nicodem.Lexer
{

    public class ParseError : Exception
    {
        public ParseError() : base()
        {
        }
    }

    public static class RegExParser
    {

        private static String regEx;
        private static int head;
        private static readonly String specialCharacters = "()[].*+|^-\\";

        public static RegEx<char> Parse(String regEx)
        {
            RegExParser.head = 0;
            RegExParser.regEx = regEx;

            var result = ParseUnion();
            if (HasNext())
                throw new ParseError();

            return Sanitize(result);
        }

        private static RegEx<char> Sanitize(RegEx<char> result)
        {
            return RegExFactory.Intersection(result, RegExFactory.Star(CharactersClasses.print));
        }

        private static RegEx<char> SingleChar(char c)
        {
            return RegExFactory.Range(c, (char)(c + 1));
        }

        private static char Peek()
        {
            if (!HasNext())
                throw new ParseError();
            return regEx[head++];
        }

        private static bool HasNext()
        {
            return (head < regEx.Length);
        }

        private static void Accept(String s)
        {
            if (!HasPrefix(s))
                throw new ParseError();
            Eat(s.Length);
        }

        private static void Eat(int howMany)
        {
            if (head + howMany > regEx.Length)
                throw new ParseError();
            head += howMany;
        }

        private static bool HasPrefix(String s)
        {
            if (head + s.Length > regEx.Length)
                return false;
            return regEx.Substring(head, s.Length).Equals(s);
        }

        private static RegEx<char> ParseUnion()
        {
            var left = ParseConcat();
            if (HasPrefix("|")) {
                Eat(1);

                var right = ParseUnion();

                if (right == null)
                    throw new ParseError();

                return RegExFactory.Union(left, right);
            }

            return left;
        }

        private static RegEx<char> ParseConcat()
        {
            var left = ParseStar();

            if (left == null)
                return null;

            var right = ParseConcat();

            if (right != null)
                return RegExFactory.Concat(left, right);

            return left;
        }

        private static RegEx<char> ParseStar()
        {
            var left = ParseAtom();

            if (HasPrefix("*") || HasPrefix("+")) {
                bool star = false;

                while (HasPrefix("*") || HasPrefix("+")) {
                    star |= HasPrefix("+");
                    Eat(1);
                }

                if (left == null)
                    throw new ParseError();

                return (star ? RegExFactory.Concat(left, RegExFactory.Star(left)) : RegExFactory.Star(left));
            } 

            return left;
        }

        private static RegEx<char> ParseAtom()
        {
            if (HasPrefix("[")) {
                Eat(1);

                if (HasPrefix(":digit:]")) {
                    Eat(8);
                    return CharactersClasses.digit;
                } else {
                    if (HasPrefix(":print:]")) {
                        Eat(8);
                        return CharactersClasses.print;
                    } else if (HasPrefix(":space:]")) {
                        Eat(8);
                        return CharactersClasses.space;
                    }
                }
				    
                var a = Peek();
                Accept("-");
                var b = Peek();
                Accept("]");
                return RegExFactory.Range(a, (char)((int)b + 1));
            }

            if (HasPrefix(".")) {
                Eat(1);
                return RegExFactory.All<char>();
            }

            if (HasPrefix("(")) {
                Eat(1);
                var node = ParseUnion();
                if (node == null)
                    throw new ParseError();
                Accept(")");
                return node;
            }

            if (HasPrefix("^")) {
                Eat(1);
                var node = ParseAtom();
                if (node == null)
                    throw new ParseError();
                return RegExFactory.Complement(node);
            }

            if (HasPrefix("\\")) {
                Eat(1);
                char a = Peek();

                if (!specialCharacters.Contains("" + a))
                    throw new ParseError();
                return SingleChar(a);
            }

            if (HasNext()) {
                var a = Peek();
                if (specialCharacters.Contains("" + a)) { 
                    --head;
                    return null;
                }
                return SingleChar(a);
            }

            return null;
        }
    }
}
    