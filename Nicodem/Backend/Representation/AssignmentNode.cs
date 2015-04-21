using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public class AssignmentNode : Node
    {
        public LocationNode Target { get; private set; }
        public LocationNode Source { get; private set; }
    }
}
