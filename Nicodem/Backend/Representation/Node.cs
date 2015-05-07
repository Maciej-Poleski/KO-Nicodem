using System;

namespace Nicodem.Backend.Representation
{
    public abstract class Node : IValuable
    {
        protected TemporaryNode _value;

        protected Node()
        {
            Accept = NodeMixin.MakeAcceptThunk(this);
        }

        public Action<AbstractVisitor> Accept { get; private set; }

        // I can move it down inheritance hierarchy
        public virtual RegisterNode Value
        {
            get { return _value ?? (_value = new TemporaryNode()); }
        }
    }
}