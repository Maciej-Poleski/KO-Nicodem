using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.ExpressionGraph
{
    class SubExpressionGraph
    {
        public Vertex Start;
        public Vertex End{
            get{
                if(currentEnd.Count == 0)
                    return null;
                return currentEnd[0];
            }
        }
        public List<Vertex> Graph = new List<Vertex>();
        public List<Vertex> currentEnd = new List<Vertex>();

        public SubExpressionGraph(Vertex _start, List<Vertex> _graph, List<Vertex> _currentEnd)
        {
            Start = _start;
            Graph = _graph;
            currentEnd = _currentEnd;
        }

        public SubExpressionGraph(){ }
    }
}
