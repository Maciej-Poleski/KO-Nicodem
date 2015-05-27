namespace Nicodem.Backend.Representation
{
	public class PushNode : Node
	{
		public Node Value { get; private set; }

		public PushNode( Node val ) {
			Value = val;
		}

		public override Node[] GetChildren() {
			return new[]{ Value };
		}
	}
}

