using System;
using Nicodem.Backend.Representation;
using System.Collections.Generic;

namespace Nicodem.Backend.Builder
{
	public class Vertex
	{
		public Vertex ()
		{
		}

		public RegisterNode Register { get; private set; }
		public HashSet<Vertex> NonCopyNeighbors { get; set; }
		public HashSet<Vertex> CopyNeighbors { get; set; }
	}
}

