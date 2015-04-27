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

        public virtual void Visit<TFunction>(FunctionCallNode<TFunction> node)
        {
            Visit(node as Node);
        }

        public virtual void Visit<TConstant>(ConstantNode<TConstant> node)
        {
            Visit(node as Node);
        }

        public virtual void Visit<TOperator>(OperatorNode<TOperator> node)
        {
            Visit(node as Node);
        }

        #region OperatorNode subclasses

        public virtual void Visit(UnaryOperatorNode node)
        {
            Visit(node as OperatorNode<UnaryOperatorType>);
        }

        public virtual void Visit(BinaryOperatorNode node)
        {
            Visit(node as OperatorNode<BinaryOperatorType>);
        }

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