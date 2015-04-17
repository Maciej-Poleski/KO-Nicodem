using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{

    public struct CharactersClasses
    {
        public static readonly RegEx<char> digit = RegExFactory.Range('0', (char)('9' + 1));
        public static readonly RegEx<char> print = RegExFactory.Union(RegExFactory.Range((char)0, (char)32), RegExFactory.Range((char)127, (char)160));
        public static readonly RegEx<char> space = RegExFactory.Union(RegExFactory.Range('\t', (char)('\t' + 1)),
                                                       RegExFactory.Range('\r', (char)('\r' + 1)),
                                                       RegExFactory.Range('\n', (char)('\n' + 1)),
                                                       RegExFactory.Range('\v', (char)('\v' + 1)),
                                                       RegExFactory.Range('\f', (char)('\f' + 1)));

    }
}
