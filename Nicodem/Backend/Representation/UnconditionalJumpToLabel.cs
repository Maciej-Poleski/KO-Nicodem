namespace Nicodem.Backend.Representation
{
	public class UnconditionalJumpToLabelNode : Node
	{
		public LabelNode NextNode { get; private set; }

		public UnconditionalJumpToLabelNode (LabelNode nextNode)
		{
			NextNode = nextNode;
		}
	}
}
