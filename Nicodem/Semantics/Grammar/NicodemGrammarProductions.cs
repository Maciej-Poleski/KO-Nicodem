using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.Grammar
{
    internal static class NicodemGrammarProductions
    {
        internal static int TokenCategoriesCount
        {
            get { return TokenCategory.NextCategoryId; }
        }

        /// <summary>
        /// All (ordered) token categories Lexer should be aware of.
        /// </summary>
        internal static IReadOnlyList<string> TokenCategories
        {
            get
            {
                Debug.Assert(TokenCategory.Categories.Count == TokenCategoriesCount);
                return TokenCategory.Categories.ConvertAll(tc => tc.Regex);
            }
        }

        #region TokenCategoryImplementation

        private struct TokenCategory
        {
            public int Category { get; private set; }
            public string Regex { get; private set; }

            internal static int NextCategoryId { get; private set; }
            internal static Dictionary<string, TokenCategory> ImplicitTokenCategories { get; private set; }

            internal static List<TokenCategory> Categories { get; private set; }

            public TokenCategory(int category, string regex) : this()
            {
                Category = category;
                Regex = regex;
            }

            static TokenCategory()
            {
                NextCategoryId = 0;
                ImplicitTokenCategories = new Dictionary<string, TokenCategory>();
                Categories = new List<TokenCategory>();
            }

            public static implicit operator TokenCategory(string regex)
            {
                var result = new TokenCategory(NextCategoryId++, regex);
                Categories.Add(result);
                return result;
            }
        }

        private static TokenCategory T(this string token)
        {
            if (TokenCategory.ImplicitTokenCategories.ContainsKey(token))
            {
                return TokenCategory.ImplicitTokenCategories[token];
            }
            else
            {
                TokenCategory result = token;
                TokenCategory.ImplicitTokenCategories.Add(token, result);
                return result;
            }
        }

        #endregion

        #region ExplicitTokens

        // Comments
        private static TokenCategory LineCommentCStyle = "//(^\n)*\n";
        private static TokenCategory LineCommentShortStyle = "`(^(\n|`))*\n";
        private static TokenCategory ShortComment = "`(^(\n|`))*`";

        // Type
        private static TokenCategory TypeName = "(^[:space:])+";

        // TODO operators (remember - left recursion is forbidden)
        // using T(this string) extension to shorten code

        // Literal values (atomic expression)
        private static TokenCategory DecimalNumberLiteral = "[:digit:]+"; // Only decimal number literals for now
        private static TokenCategory CharacterLiteral = "'(\\[:print:])|(^')'";

        private static TokenCategory StringLiteral = "\"(^((^\\)\"))*(^\\)\"";
            // String literal is delimited by not escaped " character

        private static TokenCategory BooleanLiteral = "true|false";
        // Inject 'name resolution' functionality with above literals families to avoid ambiguity
        private static TokenCategory ObjectName = "(^[:space:])+";

        #endregion ExplicitTokens
    }
}