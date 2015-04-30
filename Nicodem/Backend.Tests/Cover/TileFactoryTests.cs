using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System.Text;
using System.Collections.Generic;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class TileFactoryTests
	{
		static readonly HardwareRegisterNode RSpecial = new HardwareRegisterNode("Rxx");

		static readonly HardwareRegisterNode RAX = new HardwareRegisterNode("RAX");
		static readonly HardwareRegisterNode RBX = new HardwareRegisterNode("RBX");
		static readonly HardwareRegisterNode RCX = new HardwareRegisterNode("RCX");

		static void updateMapping( IEnumerable<Instruction> instructions,
			IDictionary<RegisterNode, HardwareRegisterNode> mapping)
		{
			foreach (var ins in instructions)
				foreach (var reg in ins.RegistersUsed)
					if (!mapping.Keys.Contains(reg))
						mapping.Add (reg, RSpecial);
		}

		static string cover( IEnumerable<Instruction> instructions, 
			IReadOnlyDictionary<RegisterNode, HardwareRegisterNode> mapping)
		{
			var coverBuilder = new StringBuilder ();
			foreach (var instruction in instructions)
				coverBuilder.Append (instruction.ToString (mapping));
			return coverBuilder.ToString ();
		}

		[Test]
		public void Test_MOV_Reg_Reg() {
			var left = new TemporaryNode ();
			var right = new TemporaryNode ();
			var node = new AssignmentNode (left, right);

			var tile = TileFactory.MOV_Reg_Reg ();
			var instructions = tile.Cover (node);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			dict.Add (right, RBX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			const string expected = 
				"MOV RAX RBX\n" + 
				"MOV Rxx RAX\n";

			Assert.AreEqual (expected, got);
		}

		[Test]
		public void Test_ADD_Reg_Reg() {
			var left = new TemporaryNode ();
			var right = new TemporaryNode ();
			var node = new AddOperatorNode (left, right);

			var tile = TileFactory.ADD_Reg_Reg ();
			var instructions = tile.Cover (node);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, RAX);
			dict.Add (right, RBX);
			updateMapping (instructions, dict);

			var got = cover (instructions, dict);
			const string expected = 
				"ADD RAX RBX\n" + 
				"MOV Rxx RAX\n";

			Assert.AreEqual (expected, got);
		}
	}
}

