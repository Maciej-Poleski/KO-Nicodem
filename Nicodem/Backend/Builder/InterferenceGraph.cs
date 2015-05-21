using System;
using System.Collections.Generic;

namespace Nicodem.Backend.Builder
{
	public class InterferenceGraph
	{
		public InterferenceGraph (List<Vertex> vertices)
		{
			this.Vertices = vertices;
		}

		public List<Vertex> Vertices { get; private set; }

		public static Dictionary<Vertex,Vertex> copyGraph (InterferenceGraph graph)
		{
			var mapping = new Dictionary<Vertex, Vertex> ();
			foreach (Vertex vertex in graph.Vertices)
				mapping [vertex] = new Vertex (vertex.Register);

			foreach (Vertex vertex in graph.Vertices) {
				foreach (Vertex neigh in vertex.CopyNeighbors)
					mapping [vertex].CopyNeighbors.Add (mapping [neigh]);
				foreach (Vertex neigh in vertex.NonCopyNeighbors)
					mapping [vertex].NonCopyNeighbors.Add (mapping [neigh]);
			}

			return mapping;
		}
	}
}

