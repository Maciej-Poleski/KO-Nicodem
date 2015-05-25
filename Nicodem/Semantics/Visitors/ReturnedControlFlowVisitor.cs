using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.ExpressionGraph;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.Visitors
{
    class ReturnedControlFlowVisitor : ReturnedAbstractVisitor<SubExpressionGraph>
    {
        private SubExpressionGraph ConcatSubGraph(SubExpressionGraph current, SubExpressionGraph next)
        {
            if (current.Start == null || current.End == null)
                return next;
            foreach (var prev_vertex in current.currentEnd)
            {
                if (prev_vertex is OneJumpVertex)
                    ((OneJumpVertex)prev_vertex).Jump = next.Start;
                if (prev_vertex is ConditionalJumpVertex)
                    ((ConditionalJumpVertex)prev_vertex).FalseJump = next.Start;
            }
            return new SubExpressionGraph(current.Start, new List<Vertex>(current.Graph.Concat(next.Graph)), next.currentEnd);
        }

        public override SubExpressionGraph Visit(ExpressionNode node)
        {
            var expr_vertex = new OneJumpVertex(null, node);
            return new SubExpressionGraph(expr_vertex, new List<Vertex> { expr_vertex }, new List<Vertex> { expr_vertex });
        }

        public override SubExpressionGraph Visit(FunctionDefinitionNode node)
        {
            base.Visit(node);

            var sub_graph = new SubExpressionGraph();

            if (!ReferenceEquals(node.Parameters, null))
                foreach (var parameter in node.Parameters)
                    if (!ReferenceEquals(parameter, null)){
                        var sub_parameter_graph = parameter.Accept(this);
                        sub_graph = ConcatSubGraph(sub_graph, sub_parameter_graph);
                    }

            if (!ReferenceEquals(node.ResultType, null))
                node.ResultType.Accept(this);

            if (!ReferenceEquals(node.Body, null))
            {
                var sub_body_graph = node.Body.Accept(this);
                sub_graph = ConcatSubGraph(sub_graph, sub_body_graph);
            }

            return sub_graph;
        }
    }
}
