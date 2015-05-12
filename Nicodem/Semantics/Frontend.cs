using System;
using System.Collections.Generic;

using Nicodem.Parser;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Visitors;
using Nicodem.Backend;

namespace Nicodem.Semantics
{
    interface ControlFlowGraph : IEnumerable<ExpressionGraph.Vertex> {}

    /// <summary>
    /// This is main front-end class. Use it to go through front-end compilation phase.
    /// </summary>
    public class Frontend
    {
        /// <summary>
        /// Build AST structure.
        /// Apply name resolution.
        /// Apply type checking.
        /// Fill nested use flag (for variables inside nested functinsn).
        /// Create backend function objects for each function.
        /// Split AST into functions.
        /// Extract controlflow.
        /// Extract small controlflow (side effects).
        /// Build backend trees.
        /// </summary>
        /// <param name="parseTree">Parse tree returned by parser.</param>
        public void FromParseTreeToBackend<TSymbol>(IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            var ast = new ASTBuilder().BuildAST(parseTree);
            ast.Accept(new NameResolutionVisitor());
            ast.Accept(new TypeCheckVisitor());
            ast.FillInNestedUseFlag();
            ast.Accept(new FunctionLocalVisitor(new Target()));

            // IReadOnlyCollection<FunctionDefinitionNode> funDefs = 
            ast.GetAllFunctionDefinitions();

            // now for each function -> extract controlflow (controlflow graph = expression graph)
            // graph - each vertex holds (AST) ExpressionNode

            // for each function - extract side effects, order
            IEnumerable<ControlFlowGraph> functionsCFGs = new List<ControlFlowGraph>();
            var afterExtract = new List<ControlFlowGraph>();
            foreach(var fungCFG in functionsCFGs){
                afterExtract.Add((ControlFlowGraph)new SideEffectExtractor().Extract(fungCFG));
            }

            //BackendBuilder.BuildBackend()

            // return LIST <Function, BackendTree>
        }
    }
}
