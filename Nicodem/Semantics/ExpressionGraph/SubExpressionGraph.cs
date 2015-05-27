using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.ExpressionGraph
{
    class SubExpressionGraph
    {
        /// <summary>
        /// Start of Graph
        /// </summary>
        public Vertex Start;
        /// <summary>
        /// End of Graph
        /// </summary>
        public Vertex End;
        /// <summary>
        /// List of Vertex in Graph
        /// </summary>
        public List<Vertex> Graph = new List<Vertex>();
        /// <summary>
        /// Flag which means if there is "fresh" if in this graph.
        /// null - means that there isn't
        /// not null - temporary use in if
        /// </summary>
        public ExpressionNode If = null;

        public SubExpressionGraph(Vertex _start, List<Vertex> _graph, Vertex _end)
        {
            Start = _start;
            Graph = _graph;
            End = _end;
        }

        public SubExpressionGraph(){ }
    }
}
