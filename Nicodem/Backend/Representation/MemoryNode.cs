namespace Nicodem.Backend.Representation
{
    public class MemoryNode : LocationNode
    {
        public MemoryNode(Node address)
        {
            Address = address;
        }

        
        public Node Address { get; private set; }

		public override Node[] GetChildren() {
			return new Node[]{ Address };
		}
    }
}