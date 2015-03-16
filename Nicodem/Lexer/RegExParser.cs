using System;

namespace Nicodem.Lexer
{

    public class ParseError : Exception
    {
        public ParseError() : base()
        {
        }
    }

    public class RegExParser
    {
        private String regEx;
        private int head;

        public RegEx<char> Parse(String regEx)
        {
            this.head = 0;
            this.regEx = regEx;

            var node = ParseUnion();
            if(HasNext())
                throw new ParseError();
            return node;
        }

        private char Peek()
        {
            if(!HasNext()) throw new ParseError();
            return regEx[head++];
        }

        private bool HasNext()
        {
            return (head < regEx.Length);
        }

        private void Accept(String s)
        {
            for(int i = 0; i < s.Length; ++i) 
                if(!HasNext() || !Peek().Equals(s[i]))
                    throw new ParseError();
        }

        private void Eat(int howMany)
        {
            if(head + howMany > regEx.Length) throw new ParseError();
            head += howMany;
        }

        private bool HasPrefix(String s)
        {
            if(head + s.Length > regEx.Length) return false;
            return regEx.Substring(head, s.Length).Equals(s);
        }

        private RegEx<char> ParseUnion()
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

        private RegEx<char> ParseConcat()
        {
            var left = ParseStar();

            if(left == null)
                return null;

            var right = ParseConcat();

            if(right != null)
                return RegExFactory.Concat(left,right);

            return left;
        }

        private RegEx<char> ParseStar()
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

        private RegEx<char> ParseAtom()
        {
            if(HasPrefix("\\["))
            {
                Eat(2);
                var a = Peek();
                Accept("-");
                var b = Peek();
                Accept("\\]");
                return RegExFactory.Range(a, b);
            }

            if(HasNext() && !HasPrefix("\\"))
            {
                var a = Peek();
                return RegExFactory.Range(a,(char)((int)a+1));
            }

            if(HasPrefix("\\^"))
            {
                Eat(2);
                var node = ParseUnion();
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
    