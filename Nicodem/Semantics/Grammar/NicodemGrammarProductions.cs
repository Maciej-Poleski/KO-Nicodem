﻿using System;
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

        internal struct TokenCategory : IEquatable<TokenCategory>
        {
            public override string ToString()
            {
                return string.Format("{1}: {0}", Regex, Category);
            }

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
                    throw new InvalidOperationException(
                        "Nonterminals are already assigned, cannot make new terminal symbol");
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
                return left*(RegexSymbol) right;
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
            token = EscapeEre(token);
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

        /// <summary>
        /// Remove words with op as infix from language
        /// </summary>
        private static string RemoveInfixes(this string token, params string[] op)
        {
            return "((" + token + ")" + "&(^(" + string.Join("|", op.Select(t => "(.*(" + t + ").*)")) + ")))";
        }

        private static string RemoveCases(this string token, params string[] cases)
        {
            return "((" + token + ")" + "&(^(" + string.Join("|", cases.Select(c => "(" + c + ")")) + ")))";
        }

        private static string RemoveCases(this string token, params TokenCategory[] cases)
        {
            return RemoveCases(token, cases.Select(c => c.Regex).ToArray());
        }

        #endregion

        #region ExplicitTokens

        private const string OperatorsPool =
			@"\( \) \-> , mutable \[ \] = \+= ; \-= \*= /= %= <<= >>= \&= \^= \|= \|\| \&\& \| \^ \& == != < <= > >= << >> \+ \- \* / % \+\+ \-\- ! ~ \.\. { } if else while break continue";

        private static readonly string[] PreparedOperators = OperatorsPool.Split(' ');

        // Comments
        private static readonly TokenCategory LineCommentCStyle = "//[^\n]*\n";
        private static readonly TokenCategory LineCommentShortStyle = "`[^\n`]*\n";
        private static readonly TokenCategory ShortComment = "`[^\n`]*`";

        // Space
        private static readonly TokenCategory Space = "[:space:]+";

        // WhiteSpace set
        private static readonly TokenCategory[] _whitespaceTokenCategories = {LineCommentCStyle, LineCommentShortStyle, ShortComment, Space};

        // Name (for type and objects)
        private const string NameBase = "[^:space:]+";
        private static readonly string NameWithoutOperators = NameBase.RemoveInfixes(PreparedOperators);

        // Literal values (atomic expression)
        internal static readonly TokenCategory DecimalNumberLiteralToken = "[:digit:]+"; // Only decimal number literals for now
        internal static readonly TokenCategory CharacterLiteralToken = "'(\\\\[:print:])|[^']'";
        internal static readonly TokenCategory StringLiteralToken = "\"((\\\\.)|[^\"])*\"";
        // String literal is delimited by not escaped " character
        internal static readonly TokenCategory BooleanLiteralToken = "true|false";

        internal static readonly TokenCategory[] LiteralTokens =
        {
            DecimalNumberLiteralToken, CharacterLiteralToken, StringLiteralToken,
            BooleanLiteralToken
        };

        private static readonly string NameWithoutOperatorsAndValues = RemoveCases(NameWithoutOperators, LiteralTokens);

        internal static readonly TokenCategory TypeName = NameWithoutOperatorsAndValues;
        internal static readonly TokenCategory ObjectName = TypeName;    // Important - get the same symbol
        

        /// <summary>
        /// This function can be used to obtain all operators defined in language
        /// </summary>
        /// <returns></returns>
        internal static string ImplicitTokens()
        {
            var r=new StringBuilder();
            foreach (var key in TokenCategory.ImplicitTokenCategories.Keys)
            {
                r.Append(key).Append(' ');
            }
            return r.ToString();
        }

        #endregion ExplicitTokens

        #region RE tools

        private static IEnumerable<char> EscapeEre1(IEnumerable<char> re)
        {
            const string special = ".[]\\()*+?|^$-&";
            var first = re.Take(1).ToArray();
            if (first.Length == 0)
            {
                return "";
            }
            var prefix = "";
            if (special.Contains(first[0]))
            {
                prefix = "\\";
            }
            prefix += first[0];
            return ((IEnumerable<char>)prefix).Concat(EscapeEre1(re.Skip(1)));
        }

        private static string EscapeEre(string re)
        {
            return new string(EscapeEre1(re).ToArray());
        }

        #endregion


        #region ProductionImplementation

        internal enum SymbolInfo
        {
            None,
            LeftToRight,
            RightToLeft,
        }

        internal struct EofSymbol
        { }

        internal static readonly EofSymbol Eof = new EofSymbol();

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

        internal class UniversalSymbol : IEquatable<UniversalSymbol>
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

            public static implicit operator UniversalSymbol(EofSymbol ignored)
            {
                return new UniversalSymbol(new EofTerminalSymbol());
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

            private class EofTerminalSymbol: ISymbol
            {
                static EofTerminalSymbol()
                {
                    SymbolToName[Symbol.EOF] = "EOF";
                }

                public Symbol ToSymbol()
                {
                    return Symbol.EOF;
                }
            }
        }

        internal class RegexSymbol
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

            public static implicit operator RegexSymbol(EofSymbol eof)
            {
                return (UniversalSymbol) eof;
            }

            public static RegexSymbol operator+(RegexSymbol left, RegexSymbol right)
            {
                return new RegexSymbol(() => RegExFactory.Union<Symbol>(left,right));
            }

            public static RegexSymbol MakeUnion(params string[] tokens)
            {
                return MakeUnion((IEnumerable<string>) tokens);
            }

            public static RegexSymbol MakeUnion(params TokenCategory[] tokens)
            {
                var regexes = tokens.Select(t => ((RegexSymbol) t)._regexSymbol()).ToArray();
                return new RegexSymbol(() => RegExFactory.Union(regexes));
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
                    return new RegexSymbol(() => RegExFactory.Union(_regexSymbol(), RegExFactory.Epsilon<Symbol>()));
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

        internal static void RegisterSymbolName(Symbol symbol, string name)
        {
            SymbolToName[symbol] = name;
        }

        internal static string GetSymbolName(Symbol symbol)
        {
			if(SymbolToName.ContainsKey(symbol)) {
				return SymbolToName[symbol];
			} else {
				return "__no_symbol_name__";
			}
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
            var operatorsRegex = RegexSymbol.MakeUnion(operators);
            return operatorExpression * (operatorsRegex * operatorExpression).Star;
        }

        #endregion

        #region Productions

        internal static readonly UniversalSymbol DecimalNumberLiteral = NewNonterminal();
        internal static readonly UniversalSymbol CharacterLiteral = NewNonterminal();
        internal static readonly UniversalSymbol StringLiteral = NewNonterminal();
        internal static readonly UniversalSymbol BooleanLiteral = NewNonterminal();
        internal static readonly UniversalSymbol Literals = NewNonterminal();

        internal static UniversalSymbol TypeSpecifier = NewNonterminal();
        internal static UniversalSymbol ObjectDeclaration = NewNonterminal();
        internal static UniversalSymbol BlockExpression = NewNonterminal();
        internal static UniversalSymbol ObjectDefinitionExpression = NewNonterminal();   // VariableDefNode
        internal static UniversalSymbol ArrayLiteralExpression = NewNonterminal();       // ArrayNode
        internal static UniversalSymbol ObjectUseExpression = NewNonterminal();          // VariableUseNode __and__ ConstNode
        internal static UniversalSymbol IfExpression = NewNonterminal();                 // IfNode
        internal static UniversalSymbol WhileExpression = NewNonterminal();              // WhileNode
        internal static UniversalSymbol LoopControlExpression = NewNonterminal();        // LoopControlNode
        internal static UniversalSymbol AtomicExpression = NewNonterminal();             // above expressions
        internal static UniversalSymbol Operator0Expression = NewNonterminal();          // atomic expression or parenthesized general expression
        internal static UniversalSymbol Operator1Expression = NewNonterminal(LeftToRight);          // scope resolution (unimplemented)
        internal static UniversalSymbol Operator2Expression = NewNonterminal(LeftToRight);          // ++ -- (postfix) function call, array subscript, slice subscript
        internal static UniversalSymbol Operator3Expression = NewNonterminal(RightToLeft);          // ++ -- + - ! ~ ((de)reference, new not implemented) prefix
        internal static UniversalSymbol Operator4Expression = NewNonterminal(LeftToRight);          // pointer to member, not implemented
        internal static UniversalSymbol Operator5Expression = NewNonterminal(LeftToRight);          // * / %
        internal static UniversalSymbol Operator6Expression = NewNonterminal(LeftToRight);          // + -
        internal static UniversalSymbol Operator7Expression = NewNonterminal(LeftToRight);          // << >>
        internal static UniversalSymbol Operator8Expression = NewNonterminal(LeftToRight);          // < <= > >=
        internal static UniversalSymbol Operator9Expression = NewNonterminal(LeftToRight);          // == !=
        internal static UniversalSymbol Operator10Expression = NewNonterminal(LeftToRight);         // &
        internal static UniversalSymbol Operator11Expression = NewNonterminal(LeftToRight);         // ^
        internal static UniversalSymbol Operator12Expression = NewNonterminal(LeftToRight);         // |
        internal static UniversalSymbol Operator13Expression = NewNonterminal(LeftToRight);         // &&
        internal static UniversalSymbol Operator14Expression = NewNonterminal(LeftToRight);         // ||
        internal static UniversalSymbol Operator15Expression = NewNonterminal(RightToLeft);         // = += -= *= /= %= <<= >>= &= ^= |=
        internal static UniversalSymbol Operator16Expression = NewNonterminal(RightToLeft);         // throw (unimplemented)
        internal static UniversalSymbol Operator17Expression = NewNonterminal(LeftToRight);         // , (N/A)
        internal static UniversalSymbol Operator18Expression = NewNonterminal(RightToLeft);         // if, while
        internal static UniversalSymbol Operator19Expression = NewNonterminal(RightToLeft);         // Object definition
        internal static UniversalSymbol OperatorExpression = NewNonterminal();           // OperationNode
		internal static UniversalSymbol RecordFieldAccessExpression = NewNonterminal();
        internal static UniversalSymbol Expression = NewNonterminal();
        internal static UniversalSymbol ParametersList = NewNonterminal();               // NOTE: There is no such node in AST, flatten this
        internal static UniversalSymbol Function = NewNonterminal();
		internal static UniversalSymbol RecordInitializationList = NewNonterminal(); 
		internal static UniversalSymbol RecordInitializationField = NewNonterminal();
		internal static UniversalSymbol RecordVariableDefinitionExpression = NewNonterminal();
		internal static UniversalSymbol RecordTypeFieldsDeclarationList = NewNonterminal();
		internal static UniversalSymbol RecordTypeFieldDeclaration = NewNonterminal();
		internal static UniversalSymbol RecordTypeDeclaration = NewNonterminal();
        internal static UniversalSymbol Program = NewNonterminal();

        static NicodemGrammarProductions()
        {
            DecimalNumberLiteral.SetProduction(DecimalNumberLiteralToken);
            CharacterLiteral.SetProduction(CharacterLiteralToken);
            StringLiteral.SetProduction(StringLiteralToken);
            BooleanLiteral.SetProduction(BooleanLiteralToken);
            Literals.SetProduction(DecimalNumberLiteral + CharacterLiteral + StringLiteral + BooleanLiteral);

			Program.SetProduction((Function + RecordTypeDeclaration).Star * Eof);
			RecordTypeDeclaration.SetProduction(TypeSpecifier * "{" * RecordTypeFieldsDeclarationList * "}");
			RecordTypeFieldsDeclarationList.SetProduction(RecordTypeFieldDeclaration.Star * RecordTypeFieldDeclaration);
			RecordTypeFieldDeclaration.SetProduction(ObjectDeclaration * ";");
            Function.SetProduction(ObjectName * "("  * ParametersList * ")" * "->" * TypeSpecifier * Expression);
            ParametersList.SetProduction(((ObjectDeclaration * ",").Star * ObjectDeclaration).Optional);
            ObjectDeclaration.SetProduction(TypeSpecifier * ObjectName);
            TypeSpecifier.SetProduction(TypeName * ("mutable".Optional() * "[" * Expression.Optional * "]").Star * "mutable".Optional());
			Expression.SetProduction(RecordVariableDefinitionExpression + RecordFieldAccessExpression + OperatorExpression);
			RecordFieldAccessExpression.SetProduction(ObjectName * "[" * ObjectName * "]");
            OperatorExpression.SetProduction(Operator19Expression);
			Operator19Expression.SetProduction(ObjectDefinitionExpression + Operator18Expression);
            ObjectDefinitionExpression.SetProduction(TypeSpecifier * ObjectName * "=" * Expression);  // NOTE: "=" is _not_ an assignment operator here
			RecordVariableDefinitionExpression.SetProduction(TypeSpecifier * ObjectName * "{" * RecordInitializationList * "}");
			RecordInitializationList.SetProduction((RecordInitializationField * ",").Star * RecordInitializationField);
			RecordInitializationField.SetProduction(ObjectName * "=" * Expression);
			Operator18Expression.SetProduction(IfExpression + WhileExpression + Operator17Expression);
            IfExpression.SetProduction("if" * Expression * Expression * ("else" * Expression).Optional);
            WhileExpression.SetProduction("while" * Expression * Expression * ("else" * Expression).Optional);
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
            Operator2Expression.SetProduction(Operator1Expression * (RegexSymbol.MakeUnion("++", "--") + ("(" * (Expression * ",").Star * Expression.Optional * ")") + ("[" * Expression * "]") + ("[" * Expression.Optional * ".." * Expression.Optional * "]")).Star);
            Operator1Expression.SetProduction(Operator0Expression);
            Operator0Expression.SetProduction(AtomicExpression + ("(" * Expression * ")"));
            AtomicExpression.SetProduction(
                BlockExpression +
                ArrayLiteralExpression +
                ObjectUseExpression +
                LoopControlExpression
                );
			BlockExpression.SetProduction("{" * (Expression * ";").Star * "}");   // No left-recursion thanks to '{'
            ArrayLiteralExpression.SetProduction("[" * (Expression * ",").Star * Expression.Optional * "]");
            ObjectUseExpression.SetProduction(ObjectName + Literals);
            LoopControlExpression.SetProduction(("break".Token() + "continue") * (Expression * DecimalNumberLiteral.Optional).Optional);
        }

        #endregion
    }
}