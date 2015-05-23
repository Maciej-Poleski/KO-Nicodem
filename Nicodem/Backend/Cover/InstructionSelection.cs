using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
    public class InstructionSelection
    {
        /// <summary>
        /// Selects instructions covering given backend trees in the given order.
        /// </summary>
        public static IEnumerable<Instruction> SelectInstructions(IEnumerable<Node> backendTrees)
        {
			var instructions = new List<Instruction> ();
			foreach (var tree in backendTrees)
				instructions.AddRange (tree.CoverWithInstructions ());
			return instructions;
        }
    }
}
