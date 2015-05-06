using System.Collections.Generic;

namespace Nicodem.Backend.Representation
{
    public class FunctionCallNode : Node
    {
        public FunctionCallNode(Function function, IReadOnlyList<Node> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        // Function type depends on architecture. Adjust when Target will be in shape.
        public Function Function { get; private set; }
        public IReadOnlyList<Node> Arguments { get; private set; }
    }
}