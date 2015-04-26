using Nicodem.Semantics.AST;
using Nicodem.Semantics.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics
{
    class SideEffectExtractor
    {
        private List<ExpressionNode> trees = new List<ExpressionNode>();

        IEnumerable<ExpressionNode> Extract(ExpressionNode root)
        {
            var visitor = new SideEffectExtractorVisitor();
            visitor.Visit(root);
            return visitor.Trees;
        }
    }
}
