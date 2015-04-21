using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public class ConstantNode : Node
    {
        // Value representation depends on architecture (and kind). Adjust when Target will be in shape.
        public object Value { get; private set; }
    }
}
