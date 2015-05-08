using System.Collections.Generic;

namespace Nicodem.Semantics.AST
{
	// Binary operators first!
	enum OperatorType
	{
        // "= += -= *= /= %= <<= >>= &= ^= |="
        ASSIGN = 0,
        PLUS_ASSIGN,
        MINUS_ASSIGN,
        MUL_ASSIGN,
        DIV_ASSIGN,
        MOD_ASSIGN,
        BIT_SHIFT_UP_ASSIGN,
        BIT_SHIFT_DOWN_ASSIGN,
        BIT_AND_ASSIGN,
        BIT_XOR_ASSIGN,
        BIT_OR_ASSIGN,
        // "||"
        OR,
        // "&&"
        AND,
        // "|"
        BIT_OR,
        // "^"
        BIT_XOR,
        // "&"
        BIT_AND,
        // "==", "!="
        EQUAL,
        NOT_EQUAL,
        // "< <= > >="
        LESS,
        LESS_EQUAL,
        GREATER,
        GREATER_EQUAL,
        // "<< >>"
        BIT_SHIFT_UP,
        BIT_SHIFT_DOWN,
        // "+ -"
        PLUS, // TODO: PLUS
        MINUS,
        // "* / %"
        MUL,
        DIV,
        MOD,
        // PRE "++ -- + - ! ~"
        PRE_INCREMENT,
        PRE_DECREMENT,
        UNARY_PLUS,
        UNARY_MINUS,
        NOT,
        BIT_NOT,
        // POST "++", "--"
        POST_INCREMENT,
        POST_DECREMENT,
    }

    static class OperatorTypeHelper
    {
        private static Dictionary<string, OperatorType> opDict = new Dictionary<string, OperatorType>()
        {
            {"=", OperatorType.ASSIGN},
            {"+", OperatorType.PLUS},
        };

        internal static OperatorType GetOperatorType(string text){
            return opDict[text];
        }
    }
}

