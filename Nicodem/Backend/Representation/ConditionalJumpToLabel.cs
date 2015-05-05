using System;

namespace Nicodem.Backend.Representation
{
	public class ConditionalJumpToLabelNode : Node
	{
	
		public Node Condition { get; private set; }

		public Node NextNode { get; private set; }

		public ConditionalJumpToLabelNode (Node condition, Node nextNode)
		{
			Condition = condition;
			NextNode = nextNode;
		}
	}
}
