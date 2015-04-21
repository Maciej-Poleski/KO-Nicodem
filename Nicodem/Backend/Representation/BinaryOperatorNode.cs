using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nicodem.Backend.Representation
{
    public enum BinaryOperatorType
    {
        // Arithmetic
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        // Shift left, shift right
        Shl,
        Shr,
        // Comparison
        Lt,
        Lte,
        Gt,
        Gte,
        Eq,
        Neq,
        // Bitwise
        BitAnd,
        BitXor,
        BitOr,
        // Logical
        LogAnd,
        LogOr
    }

    public class BinaryOperatorNode : OperatorNode<BinaryOperatorType>
    {
        public Node LeftOperand { get; private set; }

        public Node RightOperand { get; private set; }
    }
}
