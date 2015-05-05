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
		private static Dictionary<Node, bool> visited;


		public static List<Node> BuildTrace (Node node)
		{
			trace = new List<Node> ();
			epilog = LabelFactory.Label ();
			labels = new Dictionary<Node, LabelNode> ();
			visited = new Dictionary<Node, bool> ();

			DfsVisit (node);

			trace.Add (epilog);
			return trace;
		}

		private static void DfsVisit (Node node)
		{
		}
	}
}
