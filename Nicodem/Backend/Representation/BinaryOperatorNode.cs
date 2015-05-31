namespace Nicodem.Backend.Representation
{
    public abstract class BinaryOperatorNode : OperatorNode
    {
        protected BinaryOperatorNode(Node leftOperand, Node rightOperand)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public Node LeftOperand { get; private set; }
        public Node RightOperand { get; private set; }

		public override Node[] GetChildren() {
			return new Node[]{ LeftOperand, RightOperand };
		}

        #region implemented ReplaceRegisterWithLocal
        internal override Node ReplaceRegisterWithLocal(
            System.Collections.Generic.IReadOnlyDictionary<RegisterNode, Local> map, 
            System.Collections.Generic.List<Node> newTrees, 
            Function f)
        {
            LeftOperand = LeftOperand.ReplaceRegisterWithLocal(map, newTrees, f);
            RightOperand = RightOperand.ReplaceRegisterWithLocal(map, newTrees, f);
            return base.ReplaceRegisterWithLocal(map, newTrees, f);
        }
        #endregion
    }

    public class AddOperatorNode : BinaryOperatorNode
    {
        public AddOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class SubOperatorNode : BinaryOperatorNode
    {
        public SubOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class MulOperatorNode : BinaryOperatorNode
    {
        public MulOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class DivOperatorNode : BinaryOperatorNode
    {
        public DivOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class ModOperatorNode : BinaryOperatorNode
    {
        public ModOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class ShlOperatorNode : BinaryOperatorNode
    {
        public ShlOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class ShrOperatorNode : BinaryOperatorNode
    {
        public ShrOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class LtOperatorNode : BinaryOperatorNode
    {
        public LtOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class LteOperatorNode : BinaryOperatorNode
    {
        public LteOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class GtOperatorNode : BinaryOperatorNode
    {
        public GtOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class GteOperatorNode : BinaryOperatorNode
    {
        public GteOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class EqOperatorNode : BinaryOperatorNode
    {
        public EqOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class NeqOperatorNode : BinaryOperatorNode
    {
        public NeqOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class BitAndOperatorNode : BinaryOperatorNode
    {
        public BitAndOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class BitXorOperatorNode : BinaryOperatorNode
    {
        public BitXorOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class BitOrOperatorNode : BinaryOperatorNode
    {
        public BitOrOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class LogAndOperatorNode : BinaryOperatorNode
    {
        public LogAndOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }

    public class LogOrOperatorNode : BinaryOperatorNode
    {
        public LogOrOperatorNode(Node leftOperand, Node rightOperand)
            : base(leftOperand, rightOperand)
        {
        }
    }
}