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
        PLUS,
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
        private static Dictionary<int, Dictionary<string, OperatorType>> opDict = 
            new Dictionary<int, Dictionary<string, OperatorType>>()
        {
                // "= += -= *= /= %= <<= >>= &= ^= |="
            {15, new Dictionary<string, OperatorType>(){
                {"=", OperatorType.ASSIGN},
                {"+=", OperatorType.PLUS_ASSIGN},
                {"-=", OperatorType.MINUS_ASSIGN},
                {"*=", OperatorType.MUL_ASSIGN},
                {"/=", OperatorType.DIV_ASSIGN},
                {"%=", OperatorType.MOD_ASSIGN},
                {"<<=", OperatorType.BIT_SHIFT_UP_ASSIGN},
                {">>=", OperatorType.BIT_SHIFT_DOWN_ASSIGN},
                {"&=", OperatorType.BIT_AND_ASSIGN},
                {"^=", OperatorType.BIT_XOR_ASSIGN},
                {"|=", OperatorType.BIT_OR_ASSIGN},
            }},
                // "||"
            {14, new Dictionary<string, OperatorType>(){
                {"||", OperatorType.OR},
            }},
                // "&&"
            {13, new Dictionary<string, OperatorType>(){
                {"&&", OperatorType.AND},
            }},
                // "|"
            {12, new Dictionary<string, OperatorType>(){
                {"|", OperatorType.BIT_OR},
            }},
                // "^"
            {11, new Dictionary<string, OperatorType>(){
                {"^", OperatorType.BIT_XOR},
                }},
                // "&"
            {10, new Dictionary<string, OperatorType>(){
                {"&", OperatorType.BIT_AND},
            }},
                // "=="}, "!="
            {9, new Dictionary<string, OperatorType>(){
                {"==", OperatorType.EQUAL},
                {"!=", OperatorType.NOT_EQUAL},
            }},
                // "< <= > >="
            {8, new Dictionary<string, OperatorType>(){
                {"<", OperatorType.LESS},
                {"<=", OperatorType.LESS_EQUAL},
                {">", OperatorType.GREATER},
                {">=", OperatorType.GREATER_EQUAL},
            }},
                // "<< >>"
            {7, new Dictionary<string, OperatorType>(){
                {"<<", OperatorType.BIT_SHIFT_UP},
                {">>", OperatorType.BIT_SHIFT_DOWN},
            }},
                // "+ -"
            {6, new Dictionary<string, OperatorType>(){
                {"+", OperatorType.PLUS},
                {"-", OperatorType.MINUS},
            }},
                // "* / %"
            {5, new Dictionary<string, OperatorType>(){
                {"*", OperatorType.MUL},
                {"/", OperatorType.DIV},
                {"%", OperatorType.MOD},
            }},
                // PRE "++ -- + - ! ~"
            {3, new Dictionary<string, OperatorType>(){
                {"++", OperatorType.PRE_INCREMENT},
                {"--", OperatorType.PRE_DECREMENT},
                {"+", OperatorType.UNARY_PLUS},
                {"-", OperatorType.UNARY_MINUS},
                {"!", OperatorType.NOT},
                {"~", OperatorType.BIT_NOT},
            }},
                // POST "++ --"
            {2, new Dictionary<string, OperatorType>(){
                {"++", OperatorType.POST_INCREMENT},
                {"--", OperatorType.POST_DECREMENT},
            }},
        };

        internal static OperatorType GetOperatorType(int level, string text)
        {
            return opDict[level][text];
        }
    }
}

