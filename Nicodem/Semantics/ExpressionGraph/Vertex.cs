using Nicodem.Semantics.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Semantics.ExpressionGraph
{
    // IMO immutability would be too much trouble here.
    abstract class Vertex
    {
        public ExpressionNode Expression { get; set; }
    }
}
