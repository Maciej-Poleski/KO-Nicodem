namespace Nicodem.Backend.Representation
{
	public class PushNode : Node
	{
		public RegisterNode Register { get; private set; }

		public PushNode( RegisterNode reg ) {
			Register = reg;
		}

		public override Node[] GetChildren() {
			return new[]{ Register };
		}
	}
}

