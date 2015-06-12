﻿using System;
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
			IEnumerable<RegisterNode> defined)
		{
			instructionBuilder = builder;
			RegistersUsed = new HashSet<RegisterNode> ();
			foreach (var node in used)
				RegistersUsed.Add (node);
			RegistersDefined = new HashSet<RegisterNode> ();
			foreach(var node in defined)
				RegistersDefined.Add(node);
			IsCopyInstruction = false;
			IsJumpInstruction = false;
			IsLabel = false;
			Label = null;
		}

		public static Instruction CopyInstruction(
			Func<IReadOnlyDictionary<RegisterNode, HardwareRegisterNode>, String> builder,
			IEnumerable<RegisterNode> used,
			IEnumerable<RegisterNode> defined) 
		{
			return new Instruction (builder, used, defined)
			{ IsCopyInstruction = true };
		}

		public static Instruction JumpInstruction(
			Func<IReadOnlyDictionary<RegisterNode, HardwareRegisterNode>, String> builder,
			IEnumerable<RegisterNode> used,
			IEnumerable<RegisterNode> defined,
			string label) 
		{
			return new Instruction (builder, used, defined)
			{ IsJumpInstruction = true, Label = label };
		}

		public static Instruction LabelInstruction(
			Func<IReadOnlyDictionary<RegisterNode, HardwareRegisterNode>, String> builder,
			string label) 
		{
			return new Instruction (builder, new RegisterNode[]{ }, new RegisterNode[]{ })
			{ IsLabel = true, Label = label };
		}

		public ICollection<RegisterNode> RegistersUsed { get; private set; }
		public ICollection<RegisterNode> RegistersDefined { get; private set; }
		public bool IsCopyInstruction { get; private set; }
		public bool IsJumpInstruction { get; private set; }
		public bool IsLabel { get; private set; }
		public string Label { get; private set; }
		public ICollection<Instruction> PrevInstructions { get; set; }

		public string ToString(IReadOnlyDictionary<RegisterNode, HardwareRegisterNode> registerMapping) {
			var str = instructionBuilder (registerMapping);
			return str != "" ? (str + "\n") : str; 
		}
	}
}

