using System;
using System.Collections.Generic;
using Nicodem.Parser;
using System.Linq;

namespace Nicodem.Semantics.AST
{
    class ASTBuilder
    {
        // ----- static methods -----

        public static bool EatSymbol<TSymbol>(string symbol, IEnumerator<IParseTree<TSymbol>> iter) 
            where TSymbol:ISymbol<TSymbol>
        {
            //TODO: implement this!
            throw new NotImplementedException();
        }

        /// <returns>This parseTree node as a ParseBranch.</returns>
        public static ParseBranch<TSymbol> AsBranch<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            return (ParseBranch<TSymbol>)parseTree;
        }

        /// <returns>This parseTree node as a ParseBranch.</returns>
        public static IEnumerable<IParseTree<TSymbol>> Children<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            return AsBranch(parseTree).Children;
        }

        /// <summary>
        /// Get the first child of this branch. If given ParseTree is not a branch or the children list is empty -
        /// behaviour is unspecified (expect an Exception).
        /// </summary>
        public static IParseTree<TSymbol> FirstChild<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            return Children(parseTree).First();
        }

        // ----- methods -----

        public Node BuildAST<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var programNode = new ProgramNode();
            programNode.BuildNode(parseTree);
            return programNode;
        }
    }
}

