using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public abstract class RegisterNode : LocationNode
    {
        public abstract string Id { get; }
    }
}
