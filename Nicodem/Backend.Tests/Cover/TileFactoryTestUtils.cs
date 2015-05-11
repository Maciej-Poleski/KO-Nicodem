using Nicodem.Backend.Cover;
using Nicodem.Backend.Representation;
using System.Collections.Generic;
using System.Text;

namespace Nicodem.Backend.Tests
{
	public static class TileFactoryTestUtils
	{
		static readonly HardwareRegisterNode rspecial = new HardwareRegisterNode("xxx");

		static readonly HardwareRegisterNode rax = new HardwareRegisterNode("rax");
		static readonly HardwareRegisterNode rbx = new HardwareRegisterNode("rbx");
		static readonly HardwareRegisterNode rcx = new HardwareRegisterNode("rcx");
		static readonly HardwareRegisterNode r9 = new HardwareRegisterNode("r9");
		static readonly HardwareRegisterNode r10 = new HardwareRegisterNode("r10");

		public static HardwareRegisterNode RAX { get { return rax; } }
		public static HardwareRegisterNode RBX { get { return rbx; } }
		public static HardwareRegisterNode RCX { get { return rcx; } }
		public static HardwareRegisterNode R9  { get { return r9;  } }
		public static HardwareRegisterNode R10 { get { return r10; } }
		public static HardwareRegisterNode SPECIAL { get { return rspecial; } }

		public static Dictionary<RegisterNode, HardwareRegisterNode> createMapping() {
			return new Dictionary<RegisterNode, HardwareRegisterNode> ();
		}

		public static void updateMapping( IEnumerable<Instruction> instructions,
			IDictionary<RegisterNode, HardwareRegisterNode> mapping)
		{
			foreach (var ins in instructions)
				foreach (var reg in ins.RegistersUsed) {
					var target = rspecial;

					var hardwareRegisterNode = reg as HardwareRegisterNode;
					if (hardwareRegisterNode != null)
						target = hardwareRegisterNode;

					if (!mapping.Keys.Contains (reg))
						mapping.Add (reg, target);
				}
		}

		public static string getASM( IEnumerable<Instruction> instructions, 
			IReadOnlyDictionary<RegisterNode, HardwareRegisterNode> mapping)
		{
			var coverBuilder = new StringBuilder ();
			foreach (var instruction in instructions)
				coverBuilder.Append (instruction.ToString (mapping));
			return coverBuilder.ToString ();
		}
	}
}

