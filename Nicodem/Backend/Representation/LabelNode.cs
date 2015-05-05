using System;

namespace Nicodem.Backend.Representation
{
	public class LabelNode : Node
	{
		public string Label { get; private set; }

		public LabelNode (string label)
		{
			Label = label;
		}
	}
}
