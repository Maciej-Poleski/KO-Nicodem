namespace Nicodem.Backend.Representation
{
    public abstract class AbstractRecursiveVisitor : AbstractVisitor
    {
        public override void Visit(Node node)
        {
        }

        #region Node subclasses

        public override void Visit(LocationNode node)
        {
            Visit(node as Node);
        }

        public override void Visit(ConditionalJumpNode node)
        {
            Visit(node as Node);
            node.Condition.Accept(this);
        }

        public override void Visit(SequenceNode node)
        {
            Visit(node as Node);
            foreach (var chld in node.Sequence)
            {
                chld.Accept(this);
            }
        }

        public override void Visit(AssignmentNode node)
        {
            Visit(node as Node);
            node.Source.Accept(this);
            node.Target.Accept(this);
        }

        public override void Visit<TFunction>(FunctionCallNode<TFunction> node)
        {
            Visit(node as Node);
            foreach (var argument in node.Arguments)
            {
                argument.Accept(this);
            }
        }

        public override void Visit<TConstant>(ConstantNode<TConstant> node)
        {
            Visit(node as Node);
        }

        public override void Visit(OperatorNode node)
        {
            Visit(node as Node);
        }

        #region OperatorNode subclasses

        public override void Visit(UnaryOperatorNode node)
        {
            Visit(node as OperatorNode);
            node.Operand.Accept(this);
        }

        public override void Visit(BinaryOperatorNode node)
        {
            Visit(node as OperatorNode);
            node.LeftOperand.Accept(this);
            node.RightOperand.Accept(this);
        }

        #region UnaryOperatorNode subclasses

        public override void Visit(NegOperatorNode node)
        {
            Visit(node as UnaryOperatorNode);
        }

        public override void Visit(BinNotOperatorNode node)
        {
            Visit(node as UnaryOperatorNode);
        }

        public override void Visit(LogNotOperatorNode node)
        {
            Visit(node as UnaryOperatorNode);
        }

        #endregion

        #region BinaryOperatorNode subclasses

        public override void Visit(AddOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(SubOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(MulOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(DivOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(ModOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(ShlOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(ShrOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(LtOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(LteOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(GtOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(GteOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(EqOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(NeqOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(BitAndOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(BitXorOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(BitOrOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(LogAndOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public override void Visit(LogOrOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        #endregion

        #endregion

        #region LocationNode subclasses

        public override void Visit(RegisterNode node)
        {
            Visit(node as LocationNode);
        }

        public override void Visit(MemoryNode node)
        {
            Visit(node as LocationNode);
        }

        #region RegisterNode subclasses

        public override void Visit(HardwareRegisterNode node)
        {
            Visit(node as RegisterNode);
        }

        public override void Visit(TemporaryNode node)
        {
            Visit(node as RegisterNode);
        }

        #endregion

        #endregion

        #endregion
    }
}