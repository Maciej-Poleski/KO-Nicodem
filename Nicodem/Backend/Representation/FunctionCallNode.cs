using System.Collections.Generic;

namespace Nicodem.Backend.Representation
{
    public class FunctionCallNode : Node
    {
        public FunctionCallNode(Function function, SequenceNode body)
        {
            Function = function;
            Body = body;
        }

        // Function type depends on architecture. Adjust when Target will be in shape.
        public Function Function { get; private set; }
        public SequenceNode Body { get; private set; }
    }
}