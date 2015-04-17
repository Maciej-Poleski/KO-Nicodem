using System;
using System.Collections.Generic;
using Nicodem.Parser;

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

        // ----- methods -----

        public Node BuildAST<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var programNode = new ProgramNode();
            programNode.BuildNode(parseTree);
            return programNode;
        }
    }
}

