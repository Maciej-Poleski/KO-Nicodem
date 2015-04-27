using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
	public class InsnTile
	{
		readonly RegisterNode temporaryRegister = new TemporaryNode ();
		readonly Func<RegisterNode, Node, IEnumerable<Instruction>> instructionBuilder;

		Type Type { get; private set; }
		IEnumerable<InsnTile> Children { get; private set; }

		public InsnTile(Type type, IEnumerable<InsnTile> children, Func<RegisterNode, Node, IEnumerable<Instruction>> instructionBuilder) {
			Type = type;
			Children = children;
			this.instructionBuilder = instructionBuilder;
		}

		public IEnumerable<Instruction> Cover(Node node) {
			throw new NotImplementedException ();
		}
	}
}

