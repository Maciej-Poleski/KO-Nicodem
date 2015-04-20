using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public abstract class OperatorNode<TOperator>
    {
        public TOperator Operator { get; private set; }
    }
}
