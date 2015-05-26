namespace Nicodem.Backend.Representation
{
	public class PopNode : Node
	{
		public RegisterNode Register { get; private set; }

		public PopNode( RegisterNode reg ) {
			Register = reg;
		}

		public override Node[] GetChildren() {
			return new[]{ Register };
		}
	}
}

