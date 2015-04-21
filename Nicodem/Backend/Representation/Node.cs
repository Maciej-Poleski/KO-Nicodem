using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Representation
{
    public abstract class Node
    {
        public Action<AbstractVisitor> Accept { get; private set; } // TODO implementation
    }
}
