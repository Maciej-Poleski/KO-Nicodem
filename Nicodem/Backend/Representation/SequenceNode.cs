using System;
using System.Collections.Generic;
using System.Linq;

namespace Nicodem.Backend.Representation
{
    public class SequenceNode : Node
    {
		// value is the value of the whole sequence
		public SequenceNode(IReadOnlyList<Node> sequence, Node nextNode, RegisterNode value = null)
        {
            Sequence = sequence;
            NextNode = nextNode;
			_register = value ?? sequence.Last().ResultRegister;
        }

        /// <summary>
        ///     This constructor allows to postpone initialization of non-tree edge (target of unconditional jump after this
        ///     seqence).
        /// </summary>
		public SequenceNode(IReadOnlyList<Node> sequence, out Action<Node> nextNodeSetter, RegisterNode value = null)
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
			_register = value;
			if(value == null) {
				_register = sequence.Last().ResultRegister;
			}
        }

        public IReadOnlyList<Node> Sequence { get; private set; }
        // This is an unconditional jump after end of this sequence
        // This is NOT in-tree parent-child connection (this link makes cycle)
        public Node NextNode { get; private set; }

		public override Node[] GetChildren() {
			var nodes = new Node[Sequence.Count];
			int i = 0;
			foreach (var node in Sequence)
				nodes [i++] = node;
			return nodes;
		}
    }
}