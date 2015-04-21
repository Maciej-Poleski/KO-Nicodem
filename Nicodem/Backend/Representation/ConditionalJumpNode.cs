using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public class ConditionalJumpNode : Node
    {
        // Conditional jump will depend on result of this computation
        public Node Condition { get; private set; }


        // These are NOT in-tree parent-child connections (these links make cycles)
        public Node NextNodeIfTrue { get; private set; }

        public Node NextNodeIfFalse { get; private set; }
    }
}
