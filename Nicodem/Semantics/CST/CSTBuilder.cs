using System;
using Nicodem.Lexer;
using Nicodem.Parser;
using Nicodem.Semantics.Grammar;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Nicodem.Source;

namespace Nicodem.Semantics.CST
{
    public static class CSTBuilder
    {
        internal static IParseTree<Symbol> build(Source.IOrigin origin)
        {
            var sanitizedTokens = SanitizedTokens(origin);

			// Assume every list from tupple is of size 1 !!!
			var leafs = sanitizedTokens.Select(r => {
				Debug.Assert(r.Item2.Count() == 1);
				return new ParseLeaf<Symbol>(r.Item1, new Symbol(r.Item2.ElementAt(0)));
			});

            var grammar = new Grammar<Symbol>(
                              NicodemGrammarProductions.StartSymbol(),
                              NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());

            var parser = new LLStarParser<Symbol>(grammar);
            var cst = parser.Parse(leafs);

            return cst;
        }

        public static IEnumerable<Tuple<IFragment, IEnumerable<int>>> SanitizedTokens(IOrigin origin)
        {
            var categories = NicodemGrammarProductions.TokenCategories;
            var whiteSpaceCategories = NicodemGrammarProductions.WhiteSpaceTokenCategories;

            var forbidden = new List<int>(whiteSpaceCategories.Count);

            for (int i = 0; i < categories.Count; ++i)
                if (whiteSpaceCategories.Contains(categories[i]))
                    forbidden.Add(i);

            var regExCategories = categories.Select(c => RegExParser.Parse(c));

            var lexer = new Lexer.Lexer(regExCategories.ToArray());

            var tokens = lexer.ProcessBare(origin).ToList();
            var sanitizedTokens = tokens.Where(a => a.Item2.All(c => !forbidden.Contains(c)));
            return sanitizedTokens;
        }
    }
}
