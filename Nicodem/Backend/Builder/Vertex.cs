using System;
using Nicodem.Backend.Representation;
using System.Collections.Generic;

namespace Nicodem.Backend.Builder
{
	public class Vertex
	{
		public Vertex (RegisterNode register)
		{
			this.Register = register;
			CopyNeighbors = new HashSet<Vertex> ();
			NonCopyNeighbors = new HashSet<Vertex> ();
		}

		public RegisterNode Register { get; private set; }

		public HashSet<Vertex> NonCopyNeighbors { get; set; }

		public HashSet<Vertex> CopyNeighbors { get; set; }
	}
}
