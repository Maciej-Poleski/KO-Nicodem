using System;
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

        /// <summary>
        ///     This constructor allows to postpone initialization of non-tree edge (target of unconditional jump after this
        ///     seqence).
        /// </summary>
        public SequenceNode(IReadOnlyList<Node> sequence, out Action<Node> nextNodeSetter)
        {
            Sequence = sequence;
            nextNodeSetter = node =>
            {
                if (NextNode == null)
                {
                    NextNode = node;
                }
                else
                {
                    throw new InvalidOperationException("NextNode property is already initialized");
                }
            };
        }

        public IReadOnlyList<Node> Sequence { get; private set; }
        // This is an unconditional jump after end of this sequence
        // This is NOT in-tree parent-child connection (this link makes cycle)
        public Node NextNode { get; private set; }
    }
}