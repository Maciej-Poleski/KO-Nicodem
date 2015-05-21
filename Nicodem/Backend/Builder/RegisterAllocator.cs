using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;
using System.Linq;

namespace Nicodem.Backend.Builder
{
	public class RegisterAllocator
	{
		private readonly IList<HardwareRegisterNode> registers;
		private HashSet<Vertex> toSpill = new HashSet<Vertex> ();
		private HashSet<Vertex> toSimplify = new HashSet<Vertex> ();
		private HashSet<Vertex> toFreeze = new HashSet<Vertex> ();
		private HashSet<Move> toCoalesce = new HashSet<Move> ();
		private HashSet<Vertex> frozen = new HashSet<Vertex> ();
		private HashSet<Vertex> removed = new HashSet<Vertex> ();
		private HashSet<Vertex> significantHeighbors = new HashSet<Vertex> ();
		private InterferenceGraph interferenceGraph;
		private FindUnion<Vertex> fu;

		private Stack<Vertex> stack = new Stack<Vertex> ();
		private Dictionary<Vertex, Vertex> mapping;


		public RegisterAllocator (IList<HardwareRegisterNode> registers)
		{
			this.registers = registers;
			SpilledRegisters = new List<RegisterNode> ();
			RegistersColoring = new Dictionary<RegisterNode, HardwareRegisterNode> ();
		}

		public void AllocateRegisters (InterferenceGraph graph)
		{		
			SpilledRegisters.Clear ();
			RegistersColoring.Clear ();
			mapping = InterferenceGraph.copyGraph (graph);

			Initialize (graph);

			do {
				if (toSimplify.Count > 0)
					Simplify ();
			//	else if (FindToCoalesce ())
			//		Coalesce ();
				else if (toFreeze.Count > 0)
					Freeze ();
				else if (toSpill.Count > 0)
					Spill ();

				UpdateVertexPartition (graph);

			} while(toSimplify.Count > 0 || toSpill.Count > 0 || toFreeze.Count > 0);

			AssignColors ();
		}

		private void UpdateVertexPartition (InterferenceGraph graph)
		{
			foreach (Vertex vertex in graph.Vertices)
				if (removed.Contains (vertex))
					continue;
				else if (mapping [vertex].NonCopyNeighbors.Count < registers.Count) {
					if (toSpill.Contains (vertex)) {
						toSpill.Remove (vertex);
						if (mapping [vertex].CopyNeighbors.Count > 0)
							toFreeze.Add (vertex);
						else
							toSimplify.Add (vertex);
					}
				}

			foreach (Vertex vertex in graph.Vertices)
				if (removed.Contains (vertex))
					continue;
				else if (mapping [vertex].CopyNeighbors.Count == 0)
				if (toFreeze.Contains (vertex)) {
					toFreeze.Remove (vertex);
					toSimplify.Add (vertex);
				}
		}

		private void Simplify ()
		{
			var v = toSimplify.First ();
			stack.Push (v);
			toSimplify.Remove (v);
			RemoveVertex (v);
		}

		private void RemoveVertex (Vertex v)
		{
			removed.Add (v);
			foreach (Vertex neigh in mapping[v].NonCopyNeighbors)
				neigh.NonCopyNeighbors.Remove (mapping [v]);
			foreach (Vertex neigh in mapping[v].CopyNeighbors)
				neigh.CopyNeighbors.Remove (mapping [v]);	
		}


		private void Spill ()
		{
			var v = toSpill.First ();
			toSpill.Remove (v);
			toSimplify.Add (v);
		}

		private void Freeze ()
		{
			var v = toFreeze.First ();
			toFreeze.Remove (v);
			toSimplify.Add (v);
			frozen.Add (v);
		}

		private void Coalesce ()
		{
			var move = toCoalesce.First ();
			toCoalesce.Remove (move);

			var a = fu.Find (move.en1);
			var b = fu.Find (move.en2);

			toSpill.Remove (b);
			toFreeze.Remove (b);
			toSimplify.Remove (b);
			
			a.CopyNeighbors.Union (b.CopyNeighbors);
			a.NonCopyNeighbors.Union (b.NonCopyNeighbors);

			foreach (Vertex vertex in a.CopyNeighbors) {
				vertex.CopyNeighbors.Remove (b);
				vertex.CopyNeighbors.Add (a);
			}

			foreach (Vertex vertex in a.NonCopyNeighbors) {
				vertex.NonCopyNeighbors.Remove (b);
				vertex.NonCopyNeighbors.Add (a);
			}

			fu.Union (a, b);
		}

		private bool FindToCoalesce ()
		{
			while (toCoalesce.Count > 0 && !ValidateMove (toCoalesce.First ()))
				toCoalesce.Remove (toCoalesce.First ());
			return (toCoalesce.Count > 0);
		}

		private bool ValidateMove (Move move)
		{
			if (frozen.Contains (move.en1) || frozen.Contains (move.en2))
				return false;
			if (fu.Find (move.en1) == fu.Find (move.en2))
				return false;
			return true;
		}

		private void RemoveCopy (Vertex a, Vertex b)
		{
			a.CopyNeighbors.Remove (fu.Find (b));
			b.CopyNeighbors.Remove (fu.Find (a));
			
		}

		private void AssignColors ()
		{
			while (stack.Count > 0) {
				var vertex = stack.Pop ();
				var forbidden = new HashSet<HardwareRegisterNode> ();

				foreach (Vertex neigh in vertex.NonCopyNeighbors)
					if (RegistersColoring.ContainsKey (neigh.Register)) {
						forbidden.Add (RegistersColoring [neigh.Register]);
					}
				if (forbidden.Count >= registers.Count)
					SpilledRegisters.Add (vertex.Register);
				else {
					foreach (HardwareRegisterNode reg in registers)
						if (!forbidden.Contains (reg)) {
							RegistersColoring [vertex.Register] = reg;
							break;
						}
				}
			}
		}

		private void Initialize (InterferenceGraph graph)
		{
			toSpill.Clear ();
			toSimplify.Clear ();
			toFreeze.Clear ();
			toCoalesce.Clear ();
			removed.Clear ();
			frozen.Clear ();
			stack.Clear ();
			fu = new FindUnion<Vertex> (graph.Vertices);

			foreach (Vertex vertex in graph.Vertices) {
				if (vertex.Register is HardwareRegisterNode) {
					RegistersColoring [vertex.Register] = vertex.Register as HardwareRegisterNode;
					continue;
				}
				if (vertex.NonCopyNeighbors.Count < registers.Count) {
					if (vertex.CopyNeighbors.Count > 0)
						toFreeze.Add (vertex);
					else
						toSimplify.Add (vertex);
				} else
					toSpill.Add (vertex);

				/*	foreach (Vertex neigh in vertex.NonCopyNeighbors)
					if (neigh.NonCopyNeighbors.Count >= registers.Count)
						++significantHeighbors [vertex]++;
			*/
			}
		}

		private bool BriggsTest (Vertex a, Vertex b)
		{
			int counter = 0;

			foreach (Vertex neigh in a.NonCopyNeighbors.Intersect(b.NonCopyNeighbors))
				if (neigh.CopyNeighbors.Count >= registers.Count)
					++counter;

			return (counter < registers.Count);
		}


		private struct Move
		{
			public Vertex en1;
			public Vertex en2;

			public Move (Vertex en1, Vertex en2)
			{
				this.en1 = en1;
				this.en2 = en2;
			}
		}

		public Dictionary<RegisterNode, HardwareRegisterNode> RegistersColoring { 
			get;
			private set;
		}

		public List<RegisterNode> SpilledRegisters { 
			get;
			private set;
		}
	}
}
