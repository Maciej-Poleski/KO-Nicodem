using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Cover
{
	public class Instruction
	{
		readonly Func<IReadOnlyDictionary<RegisterNode, HardwareRegisterNode>, String> instructionBuilder;

		public Instruction (
			Func<IReadOnlyDictionary<RegisterNode, HardwareRegisterNode>, String> builder,
			IEnumerable<RegisterNode> used,
			IEnumerable<RegisterNode> defined) : this (builder, used, defined, false) 
		{}

		public Instruction (
			Func<IReadOnlyDictionary<RegisterNode, HardwareRegisterNode>, String> builder,
			IEnumerable<RegisterNode> used,
			IEnumerable<RegisterNode> defined,
			bool copyInstruction)
		{
			instructionBuilder = builder;
			RegistersUsed = used;
			RegistersDefined = defined;
			IsCopyInstruction = copyInstruction;
		}

		public IEnumerable<RegisterNode> RegistersUsed { get; private set; }
		public IEnumerable<RegisterNode> RegistersDefined { get; private set; }
		public bool IsCopyInstruction { get; private set; }

		public string ToString(IReadOnlyDictionary<RegisterNode, HardwareRegisterNode> registerMapping) {
			return instructionBuilder (registerMapping) + "\n";
		}
	}
}

