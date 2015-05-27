namespace Nicodem.Backend.Representation
{
	public class PopNode : Node
	{
		public Node Value { get; private set; }

		public PopNode( Node val ) {
			Value = val;
		}

		public override Node[] GetChildren() {
			return new[]{ Value };
		}
	}
}

