using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public enum UnaryOperatorType
    {
        // Arithmetic negation neg(a) -> -a
        Neg,

        BitNot,
        LogNot,
    }

    public class UnaryOperatorNode : OperatorNode<UnaryOperatorType>
    {
        public Node Operand { get; private set; }
    }
}
