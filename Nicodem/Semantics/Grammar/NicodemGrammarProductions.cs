using System;
using System.Collections.Generic;
using System.Diagnostics;
using Nicodem.Lexer;

namespace Nicodem.Semantics.Grammar
{
    internal static class NicodemGrammarProductions
    {
        internal static int TokenCategoriesCount
        {
            get { return NextSymbolId; }
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

        private static int NextSymbolId { get; set; }

        #region TokenCategoryImplementation

        private static bool _tokenCategoryAttributionLock = false;

        private struct TokenCategory : IEquatable<TokenCategory>
        {
            public bool Equals(TokenCategory other)
            {
                return Category == other.Category && string.Equals(Regex, other.Regex);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is TokenCategory && Equals((TokenCategory) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Category*397) ^ Regex.GetHashCode();
                }
            }

            public static bool operator ==(TokenCategory left, TokenCategory right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(TokenCategory left, TokenCategory right)
            {
                return !left.Equals(right);
            }

            public int Category { get; private set; }
            public string Regex { get; private set; }

            internal static Dictionary<string, TokenCategory> ImplicitTokenCategories { get; private set; }

            internal static List<TokenCategory> Categories { get; private set; }

            public TokenCategory(int category, string regex) : this()
            {
                Category = category;
                Regex = regex;
            }

            static TokenCategory()
            {
                NextSymbolId = 0;
                ImplicitTokenCategories = new Dictionary<string, TokenCategory>();
                Categories = new List<TokenCategory>();
            }

            public static implicit operator TokenCategory(string regex)
            {
                if (_tokenCategoryAttributionLock)
                {
                    throw new InvalidOperationException("Nonterminals are already assigned, cannot make new terminal symbol");
                }
                var result = new TokenCategory(NextSymbolId++, regex);
                Categories.Add(result);
                return result;
            }

            public static RegexSymbol operator +(UniversalSymbol left, TokenCategory right)
            {
                return left + (UniversalSymbol)right;
            }

            public static RegexSymbol operator +(TokenCategory left, UniversalSymbol right)
            {
                return (UniversalSymbol) left + right;
            }

            public static RegexSymbol operator +(TokenCategory left, TokenCategory right)
            {
                return (UniversalSymbol) left + (UniversalSymbol)right;
            }

            public static RegexSymbol operator +(TokenCategory left, string right)
            {
                return (UniversalSymbol)left + (UniversalSymbol)right;
            }

            public static RegexSymbol operator +(string left, TokenCategory right)
            {
                return (UniversalSymbol)left + (UniversalSymbol)right;
            }
        }

        /// <summary>
        /// For implicit tokens
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static TokenCategory Token(this string token)
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
        // NOTE Done using implicit categories

        // Literal values (atomic expression)
        private static TokenCategory DecimalNumberLiteral = "[:digit:]+"; // Only decimal number literals for now
        private static TokenCategory CharacterLiteral = "'(\\[:print:])|(^')'";
        private static TokenCategory StringLiteral = "\"(^((^\\)\"))*(^\\)\"";
            // String literal is delimited by not escaped " character
        private static TokenCategory BooleanLiteral = "true|false";
        // Inject 'name resolution' functionality with above literals families to avoid ambiguity
        private static TokenCategory ObjectName = "(^[:space:])+";

        #endregion ExplicitTokens



        #region ProductionImplementation

        private class UniversalSymbol : IEquatable<UniversalSymbol>
        {
            public bool Equals(UniversalSymbol other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return _symbol.Equals(other._symbol);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((UniversalSymbol) obj);
            }

            public override int GetHashCode()
            {
                return _symbol.GetHashCode();
            }

            public static bool operator ==(UniversalSymbol left, UniversalSymbol right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(UniversalSymbol left, UniversalSymbol right)
            {
                return !Equals(left, right);
            }

            public static explicit operator Symbol(UniversalSymbol universalSymbol)
            {
                return universalSymbol._symbol.ToSymbol();
            }

            public static implicit operator UniversalSymbol(TokenCategory tokenCategory)
            {
                return new UniversalSymbol(new TokenCategorySymbol(tokenCategory));
            }

            public static implicit operator UniversalSymbol(string implicitToken)
            {
                return implicitToken.Token();
            }

            public static RegexSymbol operator +(UniversalSymbol left, UniversalSymbol right)
            {
                return (RegexSymbol)left + (RegexSymbol)right;
            }

            public static RegexSymbol operator +(UniversalSymbol left, string right)
            {
                return left + (UniversalSymbol)right;
            }

            public static RegexSymbol operator +(string left, UniversalSymbol right)
            {
                return (UniversalSymbol)left + right;
            }

            public RegexSymbol Star
            {
                get { return ((RegexSymbol) this).Star; }
            }

            public UniversalSymbol()
            {
                _symbol = new UnknownNonterminalSymbol();
            }

            private interface ISymbol
            {
                Symbol ToSymbol();
            }

            private readonly ISymbol _symbol;

            private UniversalSymbol(ISymbol symbol)
            {
                _symbol = symbol;
            }

            private class TokenCategorySymbol : ISymbol, IEquatable<TokenCategorySymbol>
            {
                public bool Equals(TokenCategorySymbol other)
                {
                    if (ReferenceEquals(null, other)) return false;
                    if (ReferenceEquals(this, other)) return true;
                    return _tokenCategory.Equals(other._tokenCategory);
                }

                public override bool Equals(object obj)
                {
                    if (ReferenceEquals(null, obj)) return false;
                    if (ReferenceEquals(this, obj)) return true;
                    if (obj.GetType() != this.GetType()) return false;
                    return Equals((TokenCategorySymbol) obj);
                }

                public override int GetHashCode()
                {
                    return _tokenCategory.GetHashCode();
                }

                public static bool operator ==(TokenCategorySymbol left, TokenCategorySymbol right)
                {
                    return Equals(left, right);
                }

                public static bool operator !=(TokenCategorySymbol left, TokenCategorySymbol right)
                {
                    return !Equals(left, right);
                }

                private readonly TokenCategory _tokenCategory;

                public TokenCategorySymbol(TokenCategory tokenCategory)
                {
                    _tokenCategory = tokenCategory;
                }

                public Symbol ToSymbol()
                {
                    return new Symbol(_tokenCategory.Category);
                }
            }

            private class UnknownNonterminalSymbol : ISymbol
            {
                private Symbol? _attributedSymbol = null;

                public Symbol ToSymbol()
                {
                    if (_attributedSymbol != null)
                    {
                        return _attributedSymbol.GetValueOrDefault();
                    }
                    _tokenCategoryAttributionLock = true;
                    _attributedSymbol = new Symbol(NextSymbolId++);
                    return _attributedSymbol.GetValueOrDefault();
                }
            }
        }

        private class RegexSymbol
        {
            private delegate RegEx<Symbol> SuspendedRegexSymbol();

            private readonly SuspendedRegexSymbol _regexSymbol;

            private RegexSymbol(SuspendedRegexSymbol regexSymbol)
            {
                _regexSymbol = regexSymbol;
            }

            public static implicit operator RegEx<Symbol>(RegexSymbol regexSymbol)
            {
                return regexSymbol._regexSymbol();
            }

            public static implicit operator RegexSymbol(UniversalSymbol universalSymbol)
            {
                return new RegexSymbol(() =>
                {
                    var symbol = (Symbol) universalSymbol;
                    var nextSymbol = symbol.Next();
                    return RegExFactory.Intersection(RegExFactory.Range(symbol),
                        RegExFactory.Complement(RegExFactory.Range(nextSymbol)));
                });
            }

            public static implicit operator RegexSymbol(TokenCategory tokenCategory)
            {
                return (UniversalSymbol) tokenCategory;
            }

            public static implicit operator RegexSymbol(string implicitToken)
            {
                return (UniversalSymbol) implicitToken;
            }

            public static RegexSymbol operator+(RegexSymbol left, RegexSymbol right)
            {
                return new RegexSymbol(() => RegExFactory.Union<Symbol>(left,right));
            }

            public RegexSymbol Star
            {
                get
                {
                    return new RegexSymbol(() => RegExFactory.Star(_regexSymbol()));
                }
            }

            public static RegexSymbol operator *(RegexSymbol left, RegexSymbol right)
            {
                return new RegexSymbol(() => RegExFactory.Concat(left._regexSymbol(), right._regexSymbol()));
            }

            public RegexSymbol Optional
            {
                get
                {
                    return new RegexSymbol(() => RegExFactory.Union(_regexSymbol(), RegExFactory.Empty<Symbol>()));
                }
            }
        }

        private static RegexSymbol Optional(this string implicitToken)
        {
            return ((RegexSymbol) implicitToken).Optional;
        }

        private static readonly Dictionary<UniversalSymbol,RegexSymbol> _productions=new Dictionary<UniversalSymbol, RegexSymbol>(); 

        private static UniversalSymbol NewNonterminal()
        {
            return new UniversalSymbol();
        }

        private static void SetProduction(this UniversalSymbol that, RegexSymbol regexSymbol)
        {
            _productions.Add(that,regexSymbol);
        }

        #endregion

        #region Productions

        private static UniversalSymbol WhiteSpace = NewNonterminal();
        private static UniversalSymbol TypeSpecifier = NewNonterminal();
        private static UniversalSymbol ObjectDeclaration = NewNonterminal();
        private static UniversalSymbol BlockExpression = NewNonterminal();
        private static UniversalSymbol ObjectDefinitionExpression = NewNonterminal();   // VariableDefNode
        private static UniversalSymbol ArrayLiteralExpression = NewNonterminal();       // ArrayNode
        private static UniversalSymbol ObjectUseExpression = NewNonterminal();          // VariableUseNode __and__ ConstNode
        private static UniversalSymbol IfExpression = NewNonterminal();                 // IfNode
        private static UniversalSymbol WhileExpression = NewNonterminal();              // WhileNode
        private static UniversalSymbol LoopControlExpression = NewNonterminal();        // LoopControlNode

        private static UniversalSymbol OperatorExpression = NewNonterminal();           // OperationNode
        private static UniversalSymbol Expression = NewNonterminal();
        private static UniversalSymbol ParametersList = NewNonterminal();               // NOTE: There is no such node in AST, flatten this
        private static UniversalSymbol Function = NewNonterminal();
        private static UniversalSymbol Program = NewNonterminal();

        static NicodemGrammarProductions()
        {
            Program.SetProduction((WhiteSpace.Star + Function).Star * WhiteSpace.Star);
            WhiteSpace.SetProduction(LineCommentCStyle+LineCommentShortStyle+ShortComment+"[:space:]*");
            Function.SetProduction(ObjectName * WhiteSpace.Star * "(" * WhiteSpace.Star * ParametersList * WhiteSpace.Star * ")" * WhiteSpace.Star * "->" * WhiteSpace.Star * TypeSpecifier * WhiteSpace.Star * Expression);
            ParametersList.SetProduction(((ObjectDeclaration * WhiteSpace.Star * "," * WhiteSpace.Star).Star * ObjectDeclaration).Optional);
            ObjectDeclaration.SetProduction(TypeSpecifier * WhiteSpace.Star * ObjectName);
            TypeSpecifier.SetProduction(TypeName * WhiteSpace.Star * ("mutable".Optional() * WhiteSpace.Star * "[" * Expression * "]" * WhiteSpace.Star).Star  * "mutable".Optional());
            Expression.SetProduction(
                BlockExpression +
                ObjectDefinitionExpression +
                ArrayLiteralExpression +
                ObjectUseExpression +
                IfExpression +
                WhileExpression +
                LoopControlExpression +
                OperatorExpression  // TODO all operators including subscript, slices, call, operator precedence
                );
            BlockExpression.SetProduction("{" * (WhiteSpace.Star * Expression).Star * WhiteSpace.Star * "}");   // No left-recursion thanks to '{'
            ObjectDefinitionExpression.SetProduction(TypeSpecifier * WhiteSpace.Star * ObjectName * WhiteSpace.Star * "=" * WhiteSpace.Star * Expression);  // NOTE: "=" is _not_ an assignment operator here
            ArrayLiteralExpression.SetProduction("[" * (WhiteSpace.Star * Expression).Star * WhiteSpace.Star * "]");
            ObjectUseExpression.SetProduction(ObjectName);    // Literals are handled by 'name resolution'
            IfExpression.SetProduction("if" * WhiteSpace.Star * Expression * WhiteSpace.Star * Expression * (WhiteSpace.Star * "else" * Expression).Optional);  // FIXME: if should be an operator
            WhileExpression.SetProduction("while" * WhiteSpace.Star * Expression * WhiteSpace.Star * Expression * (WhiteSpace.Star * "else" * Expression).Optional);    // the same here?
            LoopControlExpression.SetProduction(("break".Token() + "continue") * (WhiteSpace.Star * Expression * (WhiteSpace.Star * DecimalNumberLiteral).Optional).Optional);
        }

        #endregion
    }
}