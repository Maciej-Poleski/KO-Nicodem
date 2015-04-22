namespace Nicodem.Backend.Representation
{
    public class MemoryNode<TAddress> : LocationNode
    {
        public MemoryNode(TAddress address)
        {
            Address = address;
        }

        // Address representation depends on architecture. Adjust when Target will be in shape.
        public TAddress Address { get; private set; }
    }
}