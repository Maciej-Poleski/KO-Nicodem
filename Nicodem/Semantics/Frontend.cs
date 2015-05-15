using System;
using System.Linq;
using System.Collections.Generic;

using Nicodem.Parser;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics
{
    using ControlFlowGraph = IEnumerable<ExpressionGraph.Vertex>;

    /// <summary>
    /// This is main front-end class. Use it to go through front-end compilation phase.
    /// </summary>
    public class Frontend
    {
        /// <summary>
        ///     Build AST structure.
        ///     Apply name resolution.
        ///     Apply type checking.
        ///     Fill nested use flag (for variables inside nested functinsn).
        ///     Create backend function objects for each function.
        ///     Split AST into functions.
        ///     Extract controlflow.
        ///     Extract small controlflow (side effects).
        ///     Build backend trees.
        /// </summary>
        /// <param name="parseTree">Parse tree returned by parser.</param>
        /// <returns>List of pairs: (function, backend tree).</returns>
        public IEnumerable<Tuple<Backend.Function,Backend.Representation.Node>>
            FromParseTreeToBackend<TSymbol> (IParseTree<TSymbol> parseTree) where TSymbol:ISymbol<TSymbol>
        {
            // build AST
            var ast = new ASTBuilder().BuildAST(parseTree);
            // apply visitors
            ast.Accept(new NameResolutionVisitor());
            ast.Accept(new TypeCheckVisitor());
            ast.FillInNestedUseFlag();
            ast.Accept(new FunctionLocalVisitor(new Backend.Target())); // create backend function objects
            // split into functions
            IEnumerable<Tuple<Backend.Function,ExpressionNode>> funcList = ast.SplitIntoFunctions();

            // for each function -> extract controlflow (controlflow graph = expression graph)
            // each vertex of this graph holds (AST) ExpressionNode
            IEnumerable<Tuple<Backend.Function,ControlFlowGraph>> funcCFGs = funcList.Select(
                t => Tuple.Create(t.Item1, new ControlFlowExtractor().Extract(t.Item2))
            );
            // for each function - extract side effects
            funcCFGs = funcCFGs.Select(
                t => Tuple.Create(t.Item1, new SideEffectExtractor().Extract(t.Item2))
            );

            // finally build and return list of functions with its backend trees representation
            return funcCFGs.Select(
                t => Tuple.Create(t.Item1, BackendBuilder.BuildBackend(t.Item2, t.Item1))
            );
        }
    }
}
