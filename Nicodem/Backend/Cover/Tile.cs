using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public class Tile
	{
		readonly RegisterNode temporaryRegister = new TemporaryNode ();
		readonly Func<RegisterNode, Node, IEnumerable<Instruction>> instructionBuilder;

		public Type Type { get; private set; }
		public IEnumerable<Tile> Children { get; private set; }

		public Tile(Type type, IEnumerable<Tile> children, Func<RegisterNode, Node, IEnumerable<Instruction>> instructionBuilder) {
			Type = type;
			Children = children;
			this.instructionBuilder = instructionBuilder;
		}

		public IEnumerable<Instruction> Cover(Node node) {
			return instructionBuilder (temporaryRegister, node);
		}
	}
}

