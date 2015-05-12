using System;
using System.Collections.Generic;
using Nicodem.Parser;
using System.Linq;

namespace Nicodem.Semantics.AST
{
    class ASTBuilder
    {
        // ----- static methods -----

        // ----- terminal methods

        /// <summary>
        /// Check if current symbol in the given enumerator is a terminal with a text which 
        /// is equal to the given string. If yes - advance enumerator and return true.
        /// </summary>
        public static bool EatSymbol<TSymbol>(string symbol, IEnumerator<IParseTree<TSymbol>> iter) 
            where TSymbol:ISymbol<TSymbol>
        {
            if (iter.Current.Symbol.IsTerminal && 
                iter.Current.Fragment.GetOriginText().Equals(symbol))
            {
                iter.MoveNext();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return text of the current terminal symbol and advance enumerator. If symbol is not terminal - null is returned.
        /// </summary>
        public static string EatTerminal<TSymbol>(IEnumerator<IParseTree<TSymbol>> iter) where TSymbol : ISymbol<TSymbol>
        {
            string res = null;
            if (iter.Current.Symbol.IsTerminal)
            {
                res = iter.Current.Fragment.GetOriginText();
                iter.MoveNext();
            }
            return res;
        }

        /// <summary>
        /// Return text of the current terminal symbol and advance enumerator. If symbol is not terminal - null is returned.
        /// </summary>
        public static IParseTree<TSymbol> EatNonTerminal<TSymbol>(IEnumerator<IParseTree<TSymbol>> iter) where TSymbol : ISymbol<TSymbol>
        {
            IParseTree<TSymbol> res = null;
            if (!iter.Current.Symbol.IsTerminal)
            {
                res = iter.Current;
                iter.MoveNext();
            }
            return res;
        }

        // ----- symbol methods

        public static string GetName<TSymbol>(ISymbol<TSymbol> symbol)
        {
            return symbol.Description.Split(new char[] { '-' }, 2)[0];
        }

        public static string GetInformation<TSymbol>(ISymbol<TSymbol> symbol)
        {
            Console.WriteLine("GET: " + symbol.Description+ " -> " + string.Join(",", symbol.Description.Split(new char[] { '-' }, 2)));
            return symbol.Description.Split(new char[] { '-' }, 2)[1];
        }

        public static bool IsLeftOperator<TSymbol>(ISymbol<TSymbol> symbol)
        {
            return GetInformation(symbol) == "Operator-left"; // == is equivalent to equals
        }

        // ----- parse tree methods

        /// <returns>This parseTree node as a ParseBranch.</returns>
        public static ParseBranch<TSymbol> AsBranch<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            return (ParseBranch<TSymbol>)parseTree;
        }

        /// <returns>This parseTree children enumeration (treating tree as a ParseBranch).</returns>
        public static IEnumerable<IParseTree<TSymbol>> Children<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            return AsBranch(parseTree).Children;
        }

        /// <returns>This parseTree children array (treating tree as a ParseBranch).</returns>
        public static IParseTree<TSymbol>[] ChildrenArray<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol : ISymbol<TSymbol>
        {
            return Children(parseTree).ToArray();
        }

        /// <summary>
        /// Get the first child of this branch. If given ParseTree is not a branch or the children list is empty -
        /// behaviour is unspecified (expect an Exception).
        /// </summary>
        public static IParseTree<TSymbol> FirstChild<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            return Children(parseTree).First();
        }

        /// <returns>Get enumerator over children of this node (assuming it is a ParseBranch).</returns>
        public static IEnumerator<IParseTree<TSymbol>> ChildrenEnumerator<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol : ISymbol<TSymbol>
        {
            return Children(parseTree).GetEnumerator();
        }

        /// <returns>Get reversed enumerator over children of this node (assuming it is a ParseBranch).</returns>
        public static IEnumerator<IParseTree<TSymbol>> RevChildrenEnumerator<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol : ISymbol<TSymbol>
        {
            return Children(parseTree).Reverse().GetEnumerator();
        }

        // ----- methods -----

        /// <summary>
        /// Build AST tree from the given parse tree.
        /// </summary>
        public ProgramNode BuildAST<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var programNode = new ProgramNode();
            programNode.BuildNode(parseTree);
            return programNode;
        }
    }
}

