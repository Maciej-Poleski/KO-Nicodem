using System;

namespace Nicodem.Backend.Representation
{
	public class UnconditionalJumpToLabelNode : Node
	{

		public Node NextNode { get; private set; }

		public UnconditionalJumpToLabelNode (Node nextNode)
		{
			NextNode = nextNode;
		}
	}
}
