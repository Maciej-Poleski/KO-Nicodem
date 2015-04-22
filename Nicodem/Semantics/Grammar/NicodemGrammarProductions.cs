using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Nicodem.Lexer;
using Nicodem.Parser;

namespace Nicodem.Semantics.Grammar
{
    internal static class NicodemGrammarProductions
    {
        /// <summary>
        /// All (ordered) token categories Lexer should be aware of.
        /// </summary>
        internal static IReadOnlyList<string> TokenCategories
        {
            get
            {
                return TokenCategory.Categories.ConvertAll(tc => tc.Regex);
            }
        }

        internal static IReadOnlyList<string> WhiteSpaceTokenCategories
        {
            get { return _whitespaceTokenCategories.Select(tc => tc.Regex).ToArray(); }
        }

        private static int NextSymbolId { get; set; }

        #region TokenCategoryImplementation

        private static bool _tokenCategoryAttributionLock = false;

        private static string EscapeRawStringForRegex(string s)
        {
            var result=new StringBuilder();
            foreach (var c in s)
            {
                result.Append('\\').Append(c);
            }
            return result.ToString();
        }

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

            #region +

            public static RegexSymbol operator +(UniversalSymbol left, TokenCategory right)
            {
                return left + (UniversalSymbol) right;
            }

            public static RegexSymbol operator +(TokenCategory left, UniversalSymbol right)
            {
                return (UniversalSymbol) left + right;
            }

            public static RegexSymbol operator +(TokenCategory left, TokenCategory right)
            {
                return (UniversalSymbol) left + (UniversalSymbol) right;
            }

            public static RegexSymbol operator +(TokenCategory left, string right)
            {
                return (UniversalSymbol) left + (UniversalSymbol) right;
            }

            public static RegexSymbol operator +(string left, TokenCategory right)
            {
                return (UniversalSymbol) left + (UniversalSymbol) right;
            }

            #endregion +

            #region *

            public static RegexSymbol operator *(TokenCategory left, string right)
            {
                return (RegexSymbol) left * (RegexSymbol) right;
            }

            public RegexSymbol Optional
            {
                get { return ((RegexSymbol) this).Optional; }
            }

            #endregion
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

        // Space
        private static TokenCategory Space = "[:space:]+";

        // WhiteSpace set
        private static TokenCategory[] _whitespaceTokenCategories = {LineCommentCStyle, LineCommentShortStyle, ShortComment, Space};

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

        private enum SymbolInfo
        {
            None,
            LeftToRight,
            RightToLeft,
        }

        private static string DescribeSymbolInfo(SymbolInfo info)
        {
            switch (info)
            {
                case SymbolInfo.None:
                    return "";
                case SymbolInfo.LeftToRight:
                    return "-Operator-left";
                case SymbolInfo.RightToLeft:
                    return "-Operator-right";
            }
            Debug.Assert(false);
            return null;
        }

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
                var symbol = universalSymbol._symbol.ToSymbol();
                if (universalSymbol._name == null)
                {
                    FixUniversalSymbolNames();
                }
                Debug.Assert(universalSymbol._name != null || symbol.IsTerminal);
                if (!SymbolToName.ContainsKey(symbol))
                {
                    SymbolToName.Add(symbol, universalSymbol._name + DescribeSymbolInfo(universalSymbol._info));
                }
                return symbol;
            }

            public static implicit operator UniversalSymbol(TokenCategory tokenCategory)
            {
                return new UniversalSymbol(new TokenCategorySymbol(tokenCategory));
            }

            public static implicit operator UniversalSymbol(string implicitToken)
            {
                return implicitToken.Token();
            }

            #region +

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
            
            #endregion

            #region *

            public static RegexSymbol operator *(UniversalSymbol left, string right)
            {
                return (RegexSymbol) left * (RegexSymbol) right;
            }

            public static RegexSymbol operator *(UniversalSymbol left, TokenCategory right)
            {
                return (RegexSymbol) left * (RegexSymbol) right;
            }

            public static RegexSymbol operator *(string left, UniversalSymbol right)
            {
                return (RegexSymbol) left * (RegexSymbol) right;
            }

            #endregion

            public RegexSymbol Star
            {
                get { return ((RegexSymbol) this).Star; }
            }

            public RegexSymbol Optional
            {
                get { return ((RegexSymbol) this).Optional; }
            }

            public UniversalSymbol(SymbolInfo info)
            {
                _symbol = new UnknownNonterminalSymbol();
                _info = info;
            }

            private interface ISymbol
            {
                Symbol ToSymbol();
            }

            private readonly ISymbol _symbol;
            private readonly SymbolInfo _info;
            private string _name;

            private UniversalSymbol(ISymbol symbol)
            {
                _symbol = symbol;
            }

            private static void FixUniversalSymbolNames()
            {
                foreach (var field in typeof (NicodemGrammarProductions).GetFields(BindingFlags.Static | BindingFlags.NonPublic))
                {
                    if (field.FieldType != typeof (UniversalSymbol)) continue;
                    var universalSymbol = (UniversalSymbol) field.GetValue(null);
                    universalSymbol._name = field.Name;
                }
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

            public static RegexSymbol MakeUnion(params string[] tokens)
            {
                return MakeUnion((IEnumerable<string>) tokens);
            }

            public static RegexSymbol MakeUnion(IEnumerable<string> tokens)
            {
                var enumerable = tokens as string[] ?? tokens.ToArray();
                foreach (var token in enumerable)
                {
                    var ignored = (RegexSymbol) token;
                }
                return new RegexSymbol(() =>
                {
                    return RegExFactory.Union(enumerable.Select(t => ((RegexSymbol) t)._regexSymbol()).ToArray());
                });
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

        private static readonly Dictionary<UniversalSymbol, RegexSymbol> Productions =
            new Dictionary<UniversalSymbol, RegexSymbol>();

        /// <summary>
        /// Returns left-hand-side symbol of root production
        /// </summary>
        /// <returns></returns>
        internal static Symbol StartSymbol()
        {
            return (Symbol) Program;
        }

        /// <summary>
        /// Creates dictionary instance specifically for <see cref="Grammar{TProduction}"/> constructor.
        /// </summary>
        /// <returns></returns>
        internal static IDictionary<Symbol, IProduction<Symbol>[]> MakeProductionsDictionaryForGrammarConstructor()
        {
            return Productions.ToDictionary(production => (Symbol) production.Key,
                production => new IProduction<Symbol>[] {new Production((Symbol) production.Key, production.Value)});
        }

        private static readonly Dictionary<Symbol, string> SymbolToName=new Dictionary<Symbol, string>();

        internal static string GetSymbolName(Symbol symbol)
        {
            return SymbolToName[symbol];
        }

        #region ProductionStuff

        private static UniversalSymbol NewNonterminal(SymbolInfo info = SymbolInfo.None)
        {
            return new UniversalSymbol(info);
        }

        private const SymbolInfo LeftToRight = SymbolInfo.LeftToRight;
        private const SymbolInfo RightToLeft = SymbolInfo.RightToLeft;

        private static void SetProduction(this UniversalSymbol that, RegexSymbol regexSymbol)
        {
            Productions.Add(that, regexSymbol);
        }

        #endregion

        private static RegexSymbol MakeInfixOperatorExpressionRegex(UniversalSymbol operatorExpression, params string[] operators)
        {
            var operatorsRegex = RegexSymbol.MakeUnion(operators.Select(EscapeRawStringForRegex));
            return operatorExpression * (operatorsRegex * operatorExpression).Star;
        }

        #endregion

        #region Productions

        private static UniversalSymbol TypeSpecifier = NewNonterminal();
        private static UniversalSymbol ObjectDeclaration = NewNonterminal();
        private static UniversalSymbol BlockExpression = NewNonterminal();
        private static UniversalSymbol ObjectDefinitionExpression = NewNonterminal();   // VariableDefNode
        private static UniversalSymbol ArrayLiteralExpression = NewNonterminal();       // ArrayNode
        private static UniversalSymbol ObjectUseExpression = NewNonterminal();          // VariableUseNode __and__ ConstNode
        private static UniversalSymbol IfExpression = NewNonterminal();                 // IfNode
        private static UniversalSymbol WhileExpression = NewNonterminal();              // WhileNode
        private static UniversalSymbol LoopControlExpression = NewNonterminal();        // LoopControlNode
        private static UniversalSymbol AtomicExpression = NewNonterminal();             // above expressions
        private static UniversalSymbol Operator0Expression = NewNonterminal();          // atomic expression or parenthesized general expression
        private static UniversalSymbol Operator1Expression = NewNonterminal(LeftToRight);          // scope resolution (unimplemented)
        private static UniversalSymbol Operator2Expression = NewNonterminal(LeftToRight);          // ++ -- (postfix) function call, array subscript, slice subscript
        private static UniversalSymbol Operator3Expression = NewNonterminal(RightToLeft);          // ++ -- + - ! ~ ((de)reference, new not implemented) prefix
        private static UniversalSymbol Operator4Expression = NewNonterminal(LeftToRight);          // pointer to member, not implemented
        private static UniversalSymbol Operator5Expression = NewNonterminal(LeftToRight);          // * / %
        private static UniversalSymbol Operator6Expression = NewNonterminal(LeftToRight);          // + -
        private static UniversalSymbol Operator7Expression = NewNonterminal(LeftToRight);          // << >>
        private static UniversalSymbol Operator8Expression = NewNonterminal(LeftToRight);          // < <= > >=
        private static UniversalSymbol Operator9Expression = NewNonterminal(LeftToRight);          // == !=
        private static UniversalSymbol Operator10Expression = NewNonterminal(LeftToRight);         // &
        private static UniversalSymbol Operator11Expression = NewNonterminal(LeftToRight);         // ^
        private static UniversalSymbol Operator12Expression = NewNonterminal(LeftToRight);         // |
        private static UniversalSymbol Operator13Expression = NewNonterminal(LeftToRight);         // &&
        private static UniversalSymbol Operator14Expression = NewNonterminal(LeftToRight);         // ||
        private static UniversalSymbol Operator15Expression = NewNonterminal(RightToLeft);         // = += -= *= /= %= <<= >>= &= ^= |=
        private static UniversalSymbol Operator16Expression = NewNonterminal(RightToLeft);         // throw (unimplemented)
        private static UniversalSymbol Operator17Expression = NewNonterminal(LeftToRight);         // , (N/A)
        private static UniversalSymbol OperatorExpression = NewNonterminal();           // OperationNode
        private static UniversalSymbol Expression = NewNonterminal();
        private static UniversalSymbol ParametersList = NewNonterminal();               // NOTE: There is no such node in AST, flatten this
        private static UniversalSymbol Function = NewNonterminal();
        private static UniversalSymbol Program = NewNonterminal();

        static NicodemGrammarProductions()
        {
            Program.SetProduction(Function.Star);
            Function.SetProduction(ObjectName * "\\("  * ParametersList * "\\)" * "\\-\\>" * TypeSpecifier * Expression);
            ParametersList.SetProduction(((ObjectDeclaration * ",").Star * ObjectDeclaration).Optional);
            ObjectDeclaration.SetProduction(TypeSpecifier * ObjectName);
            TypeSpecifier.SetProduction(TypeName * ("mutable".Optional() * "\\[" * Expression.Optional * "\\]").Star * "mutable".Optional());
            Expression.SetProduction(OperatorExpression);
            OperatorExpression.SetProduction(Operator17Expression);
            Operator17Expression.SetProduction(Operator16Expression);
            Operator16Expression.SetProduction(Operator15Expression);
            Operator15Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator14Expression, "= += -= *= /= %= <<= >>= &= ^= |=".Split(' ')));
            Operator14Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator13Expression, "||"));
            Operator13Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator12Expression, "&&"));
            Operator12Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator11Expression, "|"));
            Operator11Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator10Expression, "^"));
            Operator10Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator9Expression, "&"));
            Operator9Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator8Expression, "==", "!="));
            Operator8Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator7Expression, "< <= > >=".Split(' ')));
            Operator7Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator6Expression, "<< >>".Split(' ')));
            Operator6Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator5Expression, "+ -".Split(' ')));
            Operator5Expression.SetProduction(MakeInfixOperatorExpressionRegex(Operator4Expression, "* / %".Split(' ')));
            Operator4Expression.SetProduction(Operator3Expression);
            Operator3Expression.SetProduction(RegexSymbol.MakeUnion("++ -- + - ! ~".Split(' ')).Star * Operator2Expression);
            Operator2Expression.SetProduction(Operator1Expression * (RegexSymbol.MakeUnion("\\+\\+", "\\-\\-") + ("(" * Expression.Star * ")") + ("\\[" * Expression * "\\]") + ("\\[" * Expression.Optional * "\\.\\." * Expression.Optional * "\\]")).Star);
            Operator1Expression.SetProduction(Operator0Expression);
            Operator0Expression.SetProduction(AtomicExpression + ("\\(" * Expression * "\\)"));
            AtomicExpression.SetProduction(
                BlockExpression +
                ObjectDefinitionExpression +
                ArrayLiteralExpression +
                ObjectUseExpression +
                IfExpression +
                WhileExpression +
                LoopControlExpression
                );
            BlockExpression.SetProduction("{" * Expression.Star * "}");   // No left-recursion thanks to '{'
            ObjectDefinitionExpression.SetProduction(TypeSpecifier * ObjectName * "=" * Expression);  // NOTE: "=" is _not_ an assignment operator here
            ArrayLiteralExpression.SetProduction("\\[" * (Expression * ",").Star * Expression.Optional * "\\]");
            ObjectUseExpression.SetProduction(ObjectName);    // Literals are handled by 'name resolution'
            IfExpression.SetProduction("if" * Expression * Expression * ("else" * Expression).Optional);  // FIXME: if should be an operator
            WhileExpression.SetProduction("while" * Expression * Expression * ("else" * Expression).Optional);    // the same here?
            LoopControlExpression.SetProduction(("break".Token() + "continue") * (Expression * DecimalNumberLiteral.Optional).Optional);
        }

        #endregion
    }
}