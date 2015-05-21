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
        public IEnumerable<Vertex> Extract(ExpressionNode root)
        {
            var visitor = new ControlFlowVisitor();
            visitor.Visit(root);
            return visitor.Graph;
        }
    }
}
