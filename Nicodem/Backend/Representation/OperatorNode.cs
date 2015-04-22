using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public abstract class OperatorNode<TOperator> : Node
    {
        protected OperatorNode(TOperator @operator)
        {
            Operator = @operator;
        }

        public TOperator Operator { get; private set; }
    }
}
