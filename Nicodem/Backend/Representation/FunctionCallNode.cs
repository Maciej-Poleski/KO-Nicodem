using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public class FunctionCallNode : Node
    {
        // Function type depends on architecture. Adjust when Target will be in shape.
        public object Function { get; private set; }

        public IReadOnlyList<Node> Arguments { get; private set; }
    }
}
