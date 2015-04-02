using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.Grammar
{
    internal static class NicodemGrammarProductions
    {
        internal static int TokenCategoriesCount { get { return TokenCategory.NextCategoryId; } }
        private struct TokenCategory
        {
            public int Category { get; private set; }
            public string Regex { get; private set; }

            internal static int NextCategoryId { get; private set; }

            public TokenCategory(int category, string regex) : this()
            {
                Category = category;
                Regex = regex;
            }

            static TokenCategory()
            {
                NextCategoryId = 0;
            }

            public static implicit operator TokenCategory(string regex)
            {
                return new TokenCategory(NextCategoryId++,regex);
            }
        }

        // Comments
        private static TokenCategory LineCommentCStyle = "//(^\n)*\n";
        private static TokenCategory LineCommentShortStyle = "`(^(\n|`))*\n";
        private static TokenCategory ShortComment = "`(^(\n|`))*`";

        // Type
        private static TokenCategory TypeName = "^[:space:]";

        // TODO operators (remember - left recursion is forbidden)

        // Literal values (atomic expression)
        private static TokenCategory DecimalNumberLiteral = "[:digit:]+"; // Only decimal number literals for now
        private static TokenCategory CharacterLiteral = "'[:print:]'";
        private static TokenCategory StringLiteral = "\"(^((^\\)\"))*(^\\)\"";  // String literal is delimited by not escaped " character
    }
}
