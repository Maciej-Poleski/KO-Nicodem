using NUnit.Framework;
using Nicodem.Backend.Representation;
using Nicodem.Backend.Cover;
using System.Text;
using System.Collections.Generic;

namespace Nicodem.Backend.Tests
{
	class RaxRegister : HardwareRegisterNode {
		public override string ToString () {
			return "RAX";
		}

	    public RaxRegister() : base("RAX")
	    {
	    }
	}

	class RdxRegister : HardwareRegisterNode {
		public override string ToString () {
			return "RDX";
		}

	    public RdxRegister() : base("RDX")
	    {
	    }
	}

	[TestFixture]
	public class TileFactoryTests
	{
		[Test]
		public void Test_MOV_Reg_Reg() {
			var left = new TemporaryNode ();
			var right = new TemporaryNode ();
			var node = new AssignmentNode (left, right);

			var dict = new Dictionary<RegisterNode, HardwareRegisterNode> ();
			dict.Add (left, new RaxRegister ());
			dict.Add (right, new RdxRegister ());

			var tile = TileFactory.MOV_Reg_Reg ();

			var coverBuilder = new StringBuilder ();
			foreach (var instruction in tile.Cover(node))
				coverBuilder.Append (instruction.ToString (dict));

			Assert.AreEqual ("", coverBuilder.ToString ());
		}
	}
}

