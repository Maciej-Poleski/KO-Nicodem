using System;
using System.Collections.Generic;

using Nicodem.Parser;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Visitors;
using Nicodem.Backend;

namespace Nicodem.Semantics
{
    /// <summary>
    /// This is main front-end class. Use it to go through front-end phase.
    /// </summary>
    public class Frontend
    {
        /// <summary>
        /// Build AST structure.
        /// Apply Nameresolution.
        /// </summary>
        /// <param name="parseTree">Parse tree returned by parser.</param>
        public void FromParseTreeToBackend<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var ast = new ASTBuilder().BuildAST(parseTree);
            ast.Accept(new NameResolutionVisitor());
            ast.Accept(new TypeCheckVisitor());
            ast.FillInNestedUseFlag();
            ast.Accept(new FunctionLocalVisitor(new Target()));
            IReadOnlyCollection<FunctionDefinitionNode> funDefs = ast.GetAllFunctionDefinitions();
        }
    }
}
