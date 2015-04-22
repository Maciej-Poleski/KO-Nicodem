using System.Collections.Generic;

namespace Nicodem.Backend.Representation
{
    public class FunctionCallNode<TFunction> : Node
    {
        public FunctionCallNode(TFunction function, IReadOnlyList<Node> arguments)
        {
            Function = function;
            Arguments = arguments;
        }

        // Function type depends on architecture. Adjust when Target will be in shape.
        public TFunction Function { get; private set; }
        public IReadOnlyList<Node> Arguments { get; private set; }
    }
}