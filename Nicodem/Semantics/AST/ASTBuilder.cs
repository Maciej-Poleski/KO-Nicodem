using System;
using Nicodem.Parser;

namespace Nicodem.Semantics.AST
{
    class ASTBuilder
    {
        public ASTBuilder()
        {
        }

        public Node BuildAST<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var programNode = new ProgramNode();
            programNode.BuildNode(parseTree);
            return programNode;
        }
    }
}

