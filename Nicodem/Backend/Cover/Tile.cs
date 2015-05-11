using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public class Tile
	{
		readonly Func<RegisterNode, Node, IEnumerable<Instruction>> instructionBuilder;

		public int Cost { get; private set; }
		public Type Type { get; private set; }
		public Tile[] Children { get; private set; }

		public Tile(Type type, Tile[] children, Func<RegisterNode, Node, IEnumerable<Instruction>> instructionBuilder) {
			Type = type;
			Children = children;
			this.instructionBuilder = instructionBuilder;
		}

		public IEnumerable<Instruction> Cover(Node node) {
			return instructionBuilder (node.TemporaryRegister, node);
		}
	}
}

