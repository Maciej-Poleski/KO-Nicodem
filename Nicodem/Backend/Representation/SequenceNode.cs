using System.Collections.Generic;

namespace Nicodem.Backend.Representation
{
    public class SequenceNode : Node
    {
        public SequenceNode(IReadOnlyList<Node> sequence, Node nextNode)
        {
            Sequence = sequence;
            NextNode = nextNode;
        }

        public IReadOnlyList<Node> Sequence { get; private set; }
        // This is an unconditional jump after end of this sequence
        // This is NOT in-tree parent-child connection (this link makes cycle)
        public Node NextNode { get; private set; }
    }
}