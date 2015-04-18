using Nicodem.Lexer;
using Nicodem.Parser;
using Nicodem.Semantics.Grammar;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Semantics
{
    internal static class CSTBuilder
    {
        internal static IParseTree<Symbol> buildCST(Source.IOrigin origin)
        {
            var categories = NicodemGrammarProductions.TokenCategories;
            var whiteSpaceCategories = NicodemGrammarProductions.WhiteSpaceTokenCategories;

            var forbidden = new List<int>(whiteSpaceCategories.Count);
            
            for(int i = 0, idx = 0; i < categories.Count; ++i)
                if(whiteSpaceCategories.Contains(categories[i])) forbidden[idx++] = i;

            var regExCategories = categories.Select(c => RegExParser.Parse(c));

            var lexer = new Nicodem.Lexer.Lexer(regExCategories.ToArray());

            var tokens = lexer.Process(origin).ToList();
            var sanitizedTokens = tokens.Where(a => a.Item2.All(c => ! forbidden.Contains(c)));

            var leafs = tokens.Select(r => r.Item2.Select(s => new ParseLeaf<Symbol>(r.Item1, new Symbol(s))));

            var grammar = new Grammar<Symbol>(
                NicodemGrammarProductions.StartSymbol(),
                NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());

            var parser = new LLStarParser<Symbol>(grammar);
            var cst = parser.Parse(leafs);

            return cst;
        }
    }
}
