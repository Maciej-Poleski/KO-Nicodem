using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Builder
{
	public class RegisterAllocator
	{
		private IReadOnlyList<HardwareRegisterNode> registers;

		public RegisterAllocator(IReadOnlyList<HardwareRegisterNode> registers)
		{
			this.registers = registers;
		}

		public bool AllocateRegisters (InterferenceGraph graph, IReadOnlyDictionary<RegisterNode, HardwareRegisterNode> initial)
		{		
			throw new NotImplementedException ();
		}

		public IReadOnlyDictionary<RegisterNode, HardwareRegisterNode> RegistersColoring { 
			get;
			private set;
		}

		public IReadOnlyList<RegisterNode> SpilledRegisters { 
			get;
			private set;
		}
	}
}
