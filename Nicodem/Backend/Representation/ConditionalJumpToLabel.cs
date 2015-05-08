namespace Nicodem.Backend.Representation
{
	public class ConditionalJumpToLabelNode : Node
	{
		public Node Condition { get; private set; }

		public LabelNode NextNode { get; private set; }

		public ConditionalJumpToLabelNode (Node condition, LabelNode nextNode)
		{
			Condition = condition;
			NextNode = nextNode;
		}
	}
}
