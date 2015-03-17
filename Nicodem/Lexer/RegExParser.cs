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

        public static RegEx<char> Parse(String regEx)
        {
            RegExParser.head = 0;
            RegExParser.regEx = regEx;

            var node = ParseUnion();
            if(HasNext())
                throw new ParseError();
            return node;
        }

        private static char Peek()
        {
            if(!HasNext()) throw new ParseError();
            return regEx[head++];
        }

        private static bool HasNext()
        {
            return (head < regEx.Length);
        }

        private static void Accept(String s)
        {
            if(!HasPrefix(s))
                throw new ParseError();
            Eat(s.Length);
        }

        private static void Eat(int howMany)
        {
            if(head + howMany > regEx.Length) throw new ParseError();
            head += howMany;
        }

        private static bool HasPrefix(String s)
        {
            if(head + s.Length > regEx.Length) return false;
            return regEx.Substring(head, s.Length).Equals(s);
        }

        private static RegEx<char> ParseUnion()
        {
            var left = ParseConcat();
            if(HasPrefix("\\|"))
            {
                Eat(2);

                var right = ParseUnion();

                if(right == null)
                    throw new ParseError();
                return RegExFactory.Union(left, right);
            }

            return left;
        }

        private static RegEx<char> ParseConcat()
        {
            var left = ParseStar();

            if(left == null)
                return null;

            var right = ParseConcat();

            if(right != null)
                return RegExFactory.Concat(left,right);

            return left;
        }

        private static RegEx<char> ParseStar()
        {
            var left = ParseAtom();

            if(HasPrefix("\\*"))
            {
                while(HasPrefix("\\*"))
                    Eat(2);
                return RegExFactory.Star(left);
            }

            return left;
        }

        private static RegEx<char> ParseAtom()
        {
            if(HasPrefix("\\["))
            {
                Eat(2);
                var a = Peek();
                Accept("-");
                var b = Peek();
                Accept("\\]");
                return RegExFactory.Range(a, (char)((int)b + 1));
            }

            if(HasNext() && !HasPrefix("\\"))
            {
                var a = Peek();
                return RegExFactory.Range(a,(char)((int)a + 1));
            }

            if(HasPrefix("\\^"))
            {
                Eat(2);
                var node = ParseAtom();
                if(node == null)
                    throw new ParseError();
                return RegExFactory.Complement(node);
            }

            if(HasPrefix("\\("))
            {
                Eat(2);
                var node = ParseUnion();
                if(node == null)
                    throw new ParseError();
                Accept("\\)");
                return node;
            }

            return null;
        }
    }
}
    