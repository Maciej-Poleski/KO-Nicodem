namespace Nicodem.Backend.Representation
{
    public abstract class UnaryOperatorNode : OperatorNode
    {
        protected UnaryOperatorNode(Node operand)
        {
            Operand = operand;
        }

        public Node Operand { get; private set; }

		public override Node[] GetChildren() {
			return new Node[]{ Operand };
		}
    }

    public class LogNotOperatorNode : UnaryOperatorNode
    {
        public LogNotOperatorNode(Node operand) : base(operand)
        {
        }
    }

    public class BinNotOperatorNode : UnaryOperatorNode
    {
        public BinNotOperatorNode(Node operand) : base(operand)
        {
        }
    }

    public class NegOperatorNode : UnaryOperatorNode
    {
        public NegOperatorNode(Node operand) : base(operand)
        {
        }
    }

    public class UnaryPlusOperatorNode : UnaryOperatorNode
    {
        public UnaryPlusOperatorNode(Node operand) : base(operand)
        {
        }
    }

    public class UnaryMinusOperatorNode : UnaryOperatorNode
    {
        public UnaryMinusOperatorNode(Node operand) : base(operand)
        {
        }
    }

    public class IncOperatorNode : UnaryOperatorNode
    {
        public IncOperatorNode(Node operand) : base(operand)
        {
        }
    }

    public class DecOperatorNode : UnaryOperatorNode
    {
        public DecOperatorNode(Node operand) : base(operand)
        {
        }
    }
}