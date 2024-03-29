﻿using System;
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
        public static IParseTree<Symbol> Build(Source.IOrigin origin)
        {
            var sanitizedTokens = SanitizedTokens(origin);

			// Assume every list from tupple is of size 1 !!!
			var leafs = sanitizedTokens.Select(r => {
				Debug.Assert(r.Item2.Count() == 1);
				return new ParseLeaf<Symbol>(r.Item1, new Symbol(r.Item2.First()));
			});

            var grammar = new Grammar<Symbol>(
                              NicodemGrammarProductions.StartSymbol(),
                              NicodemGrammarProductions.MakeProductionsDictionaryForGrammarConstructor());

            var parser = new LlParser<Symbol>(grammar);
            var parseRes = parser.Parse(leafs);
			if(parseRes is OK<Symbol>) {
				return (parseRes as OK<Symbol>).Tree;
			} else {
				var err = parseRes as Error<Symbol>;
				Console.WriteLine(ParserUtils<Symbol>.PrepareErrorMessage(err));
				return null;
			}
        }

        public static IEnumerable<Tuple<IFragment, IEnumerable<int>>> SanitizedTokens(IOrigin origin)
        {
            var categories = NicodemGrammarProductions.TokenCategories;
            var whiteSpaceCategories = NicodemGrammarProductions.WhiteSpaceTokenCategories;

            var forbidden = new List<int>(whiteSpaceCategories.Count);

            for (int i = 0; i < categories.Count; ++i)
                if (whiteSpaceCategories.Contains(categories[i]))
                    forbidden.Add(i);

            var lexer = Lexer.Lexer.GetCompiledLexer("NicodemLexer", categories);

            var tokenizerResult = lexer.Process(origin);
            // TODO improve error handling
            if (!tokenizerResult.LastParsedLocation.EqualsLocation(tokenizerResult.FailedAtLocation))
            {
                throw new LexerFailure(origin.MakeFragment(tokenizerResult.LastParsedLocation,tokenizerResult.FailedAtLocation));
            }
            var tokens = tokenizerResult.Tokens.ToArray();
            var sanitizedTokens = tokens.Where(a => a.Item2.All(c => !forbidden.Contains(c)));
            return sanitizedTokens;
        }

        public class LexerFailure : Exception
        {
            private readonly IFragment _fragment;

            public IFragment Fragment
            {
                get { return _fragment; }
            }

            public LexerFailure(IFragment fragment)
            {
                this._fragment = fragment;
            }
        }

        private static bool EqualsLocation(this ILocation loc1, ILocation loc2)
        {
            return loc1.GetOriginPosition().Equals(loc2.GetOriginPosition());
        }
    }
}
