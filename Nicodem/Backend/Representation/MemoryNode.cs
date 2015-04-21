using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public class MemoryNode : LocationNode
    {
        // Address representation depends on architecture. Adjust when Target will be in shape.
        public object Address { get; private set; }
    }
}
