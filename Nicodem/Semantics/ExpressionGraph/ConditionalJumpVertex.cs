using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.ExpressionGraph
{
    class ConditionalJumpVertex
    {
        public Vertex TrueJump { get; set; }
        public Vertex FalseJump { get; set; }
    }
}
