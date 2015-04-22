namespace Nicodem.Backend.Representation
{
    public enum UnaryOperatorType
    {
        // Arithmetic negation neg(a) -> -a
        Neg,

        BitNot,
        LogNot
    }

    public class UnaryOperatorNode : OperatorNode<UnaryOperatorType>
    {
        public UnaryOperatorNode(UnaryOperatorType @operator, Node operand) : base(@operator)
        {
            Operand = operand;
        }

        public Node Operand { get; private set; }
    }
}