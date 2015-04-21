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

        public virtual void Visit(ConstantNode node)
        {
            Visit(node as Node);
        }

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