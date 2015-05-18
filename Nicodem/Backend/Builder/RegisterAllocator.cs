using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;
using System.Linq;

namespace Nicodem.Backend.Builder
{
	public class RegisterAllocator
	{
		private readonly IList<HardwareRegisterNode> registers;
		private Dictionary<Vertex,int> degree = new Dictionary<Vertex,int> ();
		private HashSet<Vertex> toSpill = new HashSet<Vertex> ();
		private HashSet<Vertex> toSimplify = new HashSet<Vertex> ();
		private Stack<Vertex> stack = new Stack<Vertex> ();

		public RegisterAllocator (IList<HardwareRegisterNode> registers)
		{
			this.registers = registers;
		}

		public void AllocateRegisters (InterferenceGraph graph)
		{		
			SpilledRegisters.Clear ();
			RegistersColoring.Clear();

			Initialize(graph);

			do {
				if (toSimplify.Count > 0)
					Simplify ();
				else if (toSpill.Count > 0) Spill ();
			} while(toSimplify.Count > 0 || toSpill.Count > 0);

			AssignColors ();
		}

		private void Simplify ()
		{
			var v = toSimplify.First ();
			stack.Push (v);
			toSimplify.Remove (v);

			foreach (Vertex vertex in v.NonCopyNeighbors)
				DecrementDegree (vertex);
		}

		private void Spill ()
		{
			var v = toSpill.First ();
			toSpill.Remove (v);
			toSimplify.Add (v);
		}

		private void AssignColors ()
		{
			while(stack.Count > 0) {
				var vertex = stack.Pop();
				var  forbidden = new HashSet<HardwareRegisterNode> ();

				foreach (Vertex neigh in vertex.NonCopyNeighbors)
					if (RegistersColoring.ContainsKey (neigh.Register))
						forbidden.Add (RegistersColoring [neigh.Register]);

				if (forbidden.Count >= registers.Count)
					SpilledRegisters.Add (vertex.Register);
				else {
					foreach (HardwareRegisterNode reg in registers)
						if (!forbidden.Contains (reg)){
							RegistersColoring [vertex.Register] = reg;
							break;
						}
				}
			}
		}

		private void Initialize(InterferenceGraph graph){
			toSpill.Clear();
			toSimplify.Clear();
			stack.Clear();

			foreach(Vertex vertex in graph.Vertices){
				degree[vertex] = vertex.NonCopyNeighbors.Count;
				if(vertex.Register is HardwareRegisterNode) continue;
				if(degree[vertex] < registers.Count) toSimplify.Add(vertex);
				else
					toSpill.Add(vertex);
			}
		}

		private void DecrementDegree (Vertex vertex)
		{
			--degree [vertex];

			if (degree [vertex] == registers.Count - 1) {
				toSpill.Remove (vertex);
				toSimplify.Add(vertex);
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
