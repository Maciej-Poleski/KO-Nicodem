using System;

namespace Nicodem.Backend.Representation
{
    public abstract class Node
    {
        protected Node()
        {
            Accept = NodeMixin.MakeAcceptThunk(this);
        }

        public Action<AbstractVisitor> Accept { get; private set; }
    }
}