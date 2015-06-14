using System;
using NUnit.Framework;
using Nicodem.Backend.Builder;
using Nicodem.Backend.Representation;
using System.Collections.Generic;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class RegisterAllocatorTest
	{

		[Test]
		public void SmallColorableGraph ()
		{
			var registers = new List<HardwareRegisterNode> ();
			registers.Add (new HardwareRegisterNode ("a"));
			registers.Add (new HardwareRegisterNode ("b"));

			var v1 = new Vertex (registers [1]);
			var v2 = new Vertex (new TemporaryNode ());
			var v3 = new Vertex (new TemporaryNode ());
			var v4 = new Vertex (registers [0]);

			v1.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v4, v2 }));
			v2.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v1, v3 }));
			v3.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v2, v4 }));
			v4.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v3, v1 }));

			var graph = new InterferenceGraph (new List<Vertex> (new Vertex[4]{ v1, v2, v3, v4 }));
			var allocator = new RegisterAllocator (registers);
			allocator.AllocateRegisters (graph);
			Assert.AreEqual (IsValid (graph, allocator.RegistersColoring), true);
			Assert.IsEmpty (allocator.SpilledRegisters);
		}

		[Test]
		public void NonTrivialThreeColorableGraph (){
			var registers = new List<HardwareRegisterNode> ();
			registers.Add (new HardwareRegisterNode ("a"));
			registers.Add (new HardwareRegisterNode ("b"));
			registers.Add (new HardwareRegisterNode ("c"));

			var v1 = new Vertex (new TemporaryNode ());
			var v2 = new Vertex (new TemporaryNode ());
			var v3 = new Vertex (new TemporaryNode ());
			var v4 = new Vertex (new TemporaryNode ());
			var v5 = new Vertex (new TemporaryNode ());
			var v6 = new Vertex (new TemporaryNode ());
			var v7 = new Vertex (registers[1]);
			var v8 = new Vertex (new TemporaryNode ());
			var v9 = new Vertex (new TemporaryNode ());

			v1.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v2, v3 }));
			v2.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v1, v3 }));
			v3.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v1, v2 }));
			v4.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v5, v6 }));
			v5.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v4, v6 }));
			v6.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v4, v5 }));
			v7.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v3, v4 }));
			v8.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v7, v9 }));
			v9.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v7, v8 }));

			var graph = new InterferenceGraph (new List<Vertex> (new Vertex[9]{ v1, v2, v3, v4, v5, v6, v7, v8, v9 }));
			var allocator = new RegisterAllocator (registers);
			allocator.AllocateRegisters (graph);
			Assert.AreEqual (IsValid (graph, allocator.RegistersColoring), true);
			Assert.IsEmpty (allocator.SpilledRegisters);			
		}

		[Test]
		public void SpillNeeded ()
		{
			var registers = new List<HardwareRegisterNode> ();
			registers.Add (new HardwareRegisterNode ("a"));
			registers.Add (new HardwareRegisterNode ("b"));

			var v1 = new Vertex (new TemporaryNode ());
			var v2 = new Vertex (new TemporaryNode ());
			var v3 = new Vertex (new TemporaryNode ());

			v1.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v3, v2 }));
			v2.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v1, v3 }));
			v3.NonCopyNeighbors.UnionWith (new HashSet<Vertex> (new Vertex[]{ v2, v1 }));

			var graph = new InterferenceGraph (new List<Vertex> (new Vertex[3]{ v1, v2, v3 }));
			var allocator = new RegisterAllocator (registers);
			allocator.AllocateRegisters (graph);
			Assert.AreEqual (allocator.SpilledRegisters.Count, 1);
		}

		private bool IsValid (InterferenceGraph graph, Dictionary<RegisterNode,HardwareRegisterNode> coloring)
		{
			foreach (Vertex vertex in graph.Vertices)
				foreach (Vertex neigh in vertex.NonCopyNeighbors)
					if (coloring [vertex.Register] == coloring [neigh.Register])
						return false;
			return true;
		}

	}

}

