using System;
using System.Collections.Generic;

using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
	public static class TraceBuilder
	{

		private static LabelNode epilog;
		private static List<Node> trace;
		private static Dictionary<Node, LabelNode> labels;
		private static Dictionary<Node, bool> assigned;


		public static IEnumerable<Node> BuildTrace (Node node)
		{
			trace = new List<Node> ();
			epilog = LabelFactory.NextLabel ();
			labels = new Dictionary<Node, LabelNode> ();
			assigned = new Dictionary<Node, bool> ();

			DfsVisit (node);
			trace.Add (epilog);

			return trace;
		}

		private static LabelNode GetLabel (Node node)
		{
			if (node == null)
				return epilog;
			if (labels.ContainsKey (node))
				return labels [node];
			labels [node] = LabelFactory.NextLabel ();
			return labels [node];
		}

		private static void DfsVisit (Node node)
		{
			assigned [node] = true;
			trace.Add (GetLabel (node));
			
			if (node is ConditionalJumpNode) {
				var conditionalJump = node as ConditionalJumpNode;

				var nextNode = conditionalJump.NextNodeIfTrue;
				var otherNode = conditionalJump.NextNodeIfFalse;
				var condition = conditionalJump.Condition;

				if (conditionalJump.NextNodeIfTrue == null || assigned [conditionalJump.NextNodeIfTrue]) {
					condition = new NegOperatorNode (condition);
					otherNode = conditionalJump.NextNodeIfTrue;
					nextNode = conditionalJump.NextNodeIfFalse;
				}

				trace.Add (new ConditionalJumpToLabelNode (condition, GetLabel (otherNode)));

				if (!assigned [nextNode])
					DfsVisit (nextNode);
				else
					trace.Add (new UnconditionalJumpToLabelNode (GetLabel (nextNode)));

				if (!assigned [otherNode])
					DfsVisit (otherNode);
		
			} else if (node is SequenceNode) {
				var sequenceNode = node as SequenceNode;

				foreach (Node expr in sequenceNode.Sequence)
					trace.Add (expr);

				trace.Add (new UnconditionalJumpToLabelNode (GetLabel (sequenceNode.NextNode)));
				if (!assigned [sequenceNode.NextNode])
					DfsVisit (sequenceNode.NextNode);
			}
		}
	}
}
