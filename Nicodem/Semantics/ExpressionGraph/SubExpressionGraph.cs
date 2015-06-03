using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.AST;
using System.Diagnostics;

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
        public ExpressionNode Temporary = null;
        /// <summary>
        /// Flag which means that SubGraph contains extracted control flow.
        /// </summary>
        public bool ContainsExtracted = false;
        /// <summary>
        /// Last node added to graph.
        /// </summary>
        public SubExpressionGraph LastNode = null;

        public SubExpressionGraph(){ }

        public SubExpressionGraph(Vertex _start, List<Vertex> _graph, Vertex _end, bool _containsExtracted, SubExpressionGraph _lastNode)
        {
            Start = _start;
            Graph = _graph;
            End = _end;
            ContainsExtracted = _containsExtracted;
            LastNode = _lastNode;
        }

        /// <summary>
        /// Making copy of graph.
        /// </summary>
        /// <returns></returns>
        public SubExpressionGraph MakeCopy()
        {
            return new SubExpressionGraph(Start, Graph, End, ContainsExtracted, LastNode);
        }

        public SubExpressionGraph SetLastNode(SubExpressionGraph _lastNode){
            LastNode = _lastNode;
            return this;
        }

        /// <summary>
        /// Create concatiantion of thwo SubExpressionGraphs
        /// </summary>
        /// <param name="current">First SubExpressionGraph</param>
        /// <param name="next">Second SubExpressionGraph</param>
        /// <returns>SubExpressionGraph as concatination of current and next</returns>
        public static SubExpressionGraph ConcatSubGraph(SubExpressionGraph current, SubExpressionGraph next)
        {
            if (current == null || next == null)
                throw new Exception("One Of Subgraph is null");

            Debug.Assert(current.Graph.Count != 0 || next.Graph.Count != 0);

            if (current.Start == null || current.End == null)
            {
                return next.MakeCopy();
            }
            if (next.Start == null || next.End == null)
            {
                return current.MakeCopy();
            }
            if(current.End is ConditionalJumpVertex)
                throw new Exception("End Conditional Vertex");
            ((OneJumpVertex)current.End).Jump = next.Start;
            
            return new SubExpressionGraph(current.Start, new List<Vertex>(current.Graph.Concat(next.Graph)), next.End, current.ContainsExtracted || next.ContainsExtracted, new SubExpressionGraph());
        }
    }
}
