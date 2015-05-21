using Nicodem.Semantics.AST;
using Nicodem.Semantics.ExpressionGraph;
using Nicodem.Semantics.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.Extractors
{
    public class SideEffectExtractor
    {
        IEnumerable<ExpressionNode> Extract(ExpressionNode root)
        {
            var visitor = new SideEffectExtractorVisitor();
            visitor.Visit(root);
            return visitor.Trees;
        }

        internal IEnumerable<Vertex> Extract(IEnumerable<Vertex> graph)
        {
            var vertexLists = new Dictionary<Vertex, List<Vertex>>();
            foreach (var vertex in graph) {
                var vertexList = new List<Vertex>();
                foreach (var tree in Extract(vertex.Expression)) {
                    vertexList.Add(new OneJumpVertex());
                    vertexList.Last().Expression = tree;
                }
                vertexLists[vertex].Add(vertex);
                for (int i = 0; i < vertexList.Count - 1; i++) {
                    (vertexList[i] as OneJumpVertex).Jump = vertexList[i + 1];
                }
                vertexLists[vertex] = vertexList;
            }
            foreach (var vertex in graph) {
                var last = vertexLists[vertex].Last();
                if (last is OneJumpVertex) {
                    var cast = last as OneJumpVertex;
                    cast.Jump = vertexLists[cast.Jump].First();
                } else if (last is ConditionalJumpVertex) {
                    var cast = last as ConditionalJumpVertex;
                    cast.FalseJump = vertexLists[cast.FalseJump].First();
                    cast.TrueJump = vertexLists[cast.TrueJump].First();
                }
            }
            var newVertices = new List<Vertex>();
            foreach (var vertex in graph) {
                newVertices.AddRange(vertexLists[vertex]);
            }
            return newVertices;
        }
    }
}
