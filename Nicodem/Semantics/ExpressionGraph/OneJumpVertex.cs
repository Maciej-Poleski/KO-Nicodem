using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.ExpressionGraph
{
    class OneJumpVertex : Vertex
    {
        public Vertex Jump { get; set; }

        public OneJumpVertex() { }

        public OneJumpVertex(Vertex v_jump, ExpressionNode expression)
            : base(expression)
        {
            Jump = v_jump;
        }
    }
}
