using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Semantics.AST;

namespace Nicodem.Semantics.ExpressionGraph
{
    class ConditionalJumpVertex : Vertex
    {
        public Vertex TrueJump { get; set; }
        public Vertex FalseJump { get; set; }

        public ConditionalJumpVertex(Vertex v_true, Vertex v_false, ExpressionNode expression) : base(expression)
        {
            TrueJump = v_true;
            FalseJump = v_false;
        }

        public ConditionalJumpVertex() { }
    }
}
