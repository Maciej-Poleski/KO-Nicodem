using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.ExpressionGraph;
using Nicodem.Semantics.AST;
using Nicodem.Semantics.Visitors;

namespace Nicodem.Semantics.Extractors
{
    class ControlFlowExtractor
    {
        /// <summary>
        /// Function which extract control flow from root.
        /// In IEnumerable returned vertex there are rules:
        /// 1. First Vertex it's begining of graph.
        /// 2. Last Vertex has returned value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="root">Root of AST tree.</param>
        /// <returns>Vertex in transformed expression graph.</returns>
        public IEnumerable<Vertex> Extract<T>(T root) where T : ExpressionNode
        {
            var visitor = new ReturnedControlFlowVisitor();
            var graph = root.Accept(visitor);
            return SubExpressionGraph.ConcatSubGraph(graph, graph.LastNode).Graph;
        }
    }
}
