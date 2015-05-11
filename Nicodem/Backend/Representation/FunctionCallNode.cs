using System.Collections.Generic;

namespace Nicodem.Backend.Representation
{
    public class FunctionCallNode : Node
    {
        public FunctionCallNode(SequenceNode body)
        {
            Body = body;
        }

        public SequenceNode Body { get; private set; }
    }
}