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
		private HashSet<Vertex> frozen = new HashSet<Vertex> ();
		private HashSet<Vertex> removed = new HashSet<Vertex> ();
		private HashSet<Vertex> significantHeighbors = new HashSet<Vertex> ();

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

		private void AssignColors ()
		{
			while (stack.Count > 0) {
				var vertex = stack.Pop ();
				var forbidden = new HashSet<HardwareRegisterNode> ();

				foreach (Vertex neigh in vertex.NonCopyNeighbors)
					if (RegistersColoring.ContainsKey (neigh.Register)) {
						forbidden.Add (RegistersColoring [neigh.Register]);
					}

				var occurs = new Dictionary<HardwareRegisterNode,int> ();

				foreach (Vertex neigh in vertex.CopyNeighbors)
					if (RegistersColoring.ContainsKey (neigh.Register)) {
						if (!occurs.ContainsKey (RegistersColoring [neigh.Register]))
							occurs [RegistersColoring [neigh.Register]] = 1;
						else
							occurs [RegistersColoring [neigh.Register]]++;
					}

				if (forbidden.Count >= registers.Count)
					SpilledRegisters.Add (vertex.Register);
				else {
					int mostOccurs = 0;
					HardwareRegisterNode choice = null;

					foreach (HardwareRegisterNode reg in registers)
						if (!forbidden.Contains (reg)) {
							var tmp = occurs.ContainsKey(reg) ? occurs[reg] : 0;
							if(tmp >= mostOccurs){
								mostOccurs = tmp;
								choice = reg;
							}
						}
					RegistersColoring [vertex.Register] = choice;
				}
			}
		}

		private void Initialize (InterferenceGraph graph)
		{
			toSpill.Clear ();
			toSimplify.Clear ();
			toFreeze.Clear ();
			removed.Clear ();
			frozen.Clear ();
			stack.Clear ();

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
