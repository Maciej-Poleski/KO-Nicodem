using System;
using System.Collections.Generic;

namespace Nicodem.Lexer
{

    public struct CharactersClasses
    {
        public static readonly RegEx<char> digit = RegExFactory.Range('0', (char)('9' + 1));
        public static readonly RegEx<char> print = PrintableCharacters();
        public static readonly RegEx<char> space = RegExFactory.Union(RegExFactory.Range('\t', (char)('\t' + 1)),
                                                       RegExFactory.Range('\r', (char)('\r' + 1)),
                                                       RegExFactory.Range('\n', (char)('\n' + 1)),
                                                       RegExFactory.Range('\v', (char)('\v' + 1)),
                                                       RegExFactory.Range('\f', (char)('\f' + 1)));

        private static RegEx<char> PrintableCharacters()
        {
            var lastValid = char.MaxValue;
            var ranges = new List<RegEx<char>>();

            for (var c = char.MinValue; c != char.MaxValue; ++c)
                if (char.IsControl(c)) {
                    if (!char.IsControl(lastValid)) {
                        ranges.Add(RegExFactory.Range(lastValid, c));
                        lastValid = char.MaxValue;
                    }
                } else if (lastValid == char.MaxValue)
                    lastValid = c;

            ranges.Add(RegExFactory.Range(lastValid, char.MaxValue));
            return RegExFactory.Union(ranges.ToArray());
        }
    }
}
