using System;
using Nicodem.Backend.Representation;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nicodem.Backend.Tests.Representation
{

	[TestFixture]
	public class TraceBuilderTests
	{
		[Test]
		public void SimpleTest ()
		{
			var nodes = new List<Node> ();
			nodes.Add (new TemporaryNode ());
			nodes.Add (new TemporaryNode ());
			nodes.Add (new TemporaryNode ());
			nodes.Add (new TemporaryNode ());
			nodes.Add (new TemporaryNode ());

			var node = new ConditionalJumpNode (nodes [0], new ConditionalJumpNode (nodes [1], nodes [2], nodes [3]), nodes [4]);
			var trace = new HashSet<Node> (TraceBuilder.BuildTrace (node));

			bool fine = true;

			foreach (Node n in trace)
				fine &= !(n is ConditionalJumpNode);

			fine &= trace.Contains (nodes [2]);
			fine &= trace.Contains (nodes [3]);
			fine &= trace.Contains (nodes [4]);
			
			Assert.AreEqual (fine, true);
		}
	}
}