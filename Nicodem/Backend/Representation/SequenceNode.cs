using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Backend.Representation
{
    public class SequenceNode : Node
    {
		// value is the value of the whole sequence
		public SequenceNode(IReadOnlyList<Node> sequence, Node nextNode, TemporaryNode value = null)
        {
            Sequence = sequence;
            NextNode = nextNode;
			_value = value;
			if(_value == null) {
				_value = sequence.Last().Value as TemporaryNode;
			}
        }

        /// <summary>
        ///     This constructor allows to postpone initialization of non-tree edge (target of unconditional jump after this
        ///     seqence).
        /// </summary>
		public SequenceNode(IReadOnlyList<Node> sequence, out Action<Node> nextNodeSetter, TemporaryNode value = null)
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
			_value = value;
			if(value == null) {
				_value = sequence.Last().Value as TemporaryNode;
			}
        }

        public IReadOnlyList<Node> Sequence { get; private set; }
        // This is an unconditional jump after end of this sequence
        // This is NOT in-tree parent-child connection (this link makes cycle)
        public Node NextNode { get; private set; }
    }
}