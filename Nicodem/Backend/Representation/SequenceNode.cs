using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public class SequenceNode : Node
    {
        public IReadOnlyList<Node> Sequence { get; private set; }

        // This is an unconditional jump after end of this sequence
        public Node NextNode { get; private set; }
    }
}
