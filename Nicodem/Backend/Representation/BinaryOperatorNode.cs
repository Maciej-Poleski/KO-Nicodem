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
        public BinaryOperatorNode(BinaryOperatorType @operator, Node leftOperand, Node rightOperand) : base(@operator)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public Node LeftOperand { get; private set; }
        public Node RightOperand { get; private set; }
    }
}