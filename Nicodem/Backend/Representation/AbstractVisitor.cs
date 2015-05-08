namespace Nicodem.Backend.Representation
{
    public abstract class AbstractVisitor
    {
        public virtual void Visit(Node node)
        {
        }

        #region Node subclasses

        public virtual void Visit(LocationNode node)
        {
            Visit(node as Node);
        }

        public virtual void Visit(ConditionalJumpNode node)
        {
            Visit(node as Node);
        }

        public virtual void Visit(SequenceNode node)
        {
            Visit(node as Node);
        }

        public virtual void Visit(AssignmentNode node)
        {
            Visit(node as Node);
        }

        public virtual void Visit(FunctionCallNode node)
        {
            Visit(node as Node);
        }

        public virtual void Visit<TConstant>(ConstantNode<TConstant> node)
        {
            Visit(node as Node);
        }

        public virtual void Visit(OperatorNode node)
        {
            Visit(node as Node);
        }

        #region OperatorNode subclasses

        public virtual void Visit(UnaryOperatorNode node)
        {
            Visit(node as OperatorNode);
        }

        public virtual void Visit(BinaryOperatorNode node)
        {
            Visit(node as OperatorNode);
        }

        #region UnaryOperatorNode subclasses

        public virtual void Visit(NegOperatorNode node)
        {
            Visit(node as UnaryOperatorNode);
        }

        public virtual void Visit(BinNotOperatorNode node)
        {
            Visit(node as UnaryOperatorNode);
        }

        public virtual void Visit(LogNotOperatorNode node)
        {
            Visit(node as UnaryOperatorNode);
        }

        #endregion

        #region BinaryOperatorNode subclasses

        public virtual void Visit(AddOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(SubOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(MulOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(DivOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(ModOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(ShlOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(ShrOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(LtOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(LteOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(GtOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(GteOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(EqOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(NeqOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(BitAndOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(BitXorOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(BitOrOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(LogAndOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        public virtual void Visit(LogOrOperatorNode node)
        {
            Visit(node as BinaryOperatorNode);
        }

        #endregion

        #endregion

        #region LocationNode subclasses

        public virtual void Visit(RegisterNode node)
        {
            Visit(node as LocationNode);
        }

        public virtual void Visit(MemoryNode node)
        {
            Visit(node as LocationNode);
        }

        #region RegisterNode subclasses

        public virtual void Visit(HardwareRegisterNode node)
        {
            Visit(node as RegisterNode);
        }

        public virtual void Visit(TemporaryNode node)
        {
            Visit(node as RegisterNode);
        }

        #endregion

        #endregion

        #endregion
    }
}