using NUnit.Framework;
using Nicodem.Backend.Builder;
using System.Collections.Generic;
using Nicodem.Backend.Cover;
using Nicodem.Backend.Representation;
using System;

namespace Nicodem.Backend.Tests
{
	[TestFixture]
	public class LivenessAnalysisTest
	{
		private Func<IReadOnlyDictionary<RegisterNode, HardwareRegisterNode>, String>
			instructionBuilder = null;

		private static HardwareRegisterNode x = new HardwareRegisterNode("x");
		private static HardwareRegisterNode y = new HardwareRegisterNode("y");
		//private static HardwareRegisterNode z = new HardwareRegisterNode("z");

		[Test]
		public void SimpleTest ()
		{
			RegisterNode[] firstUsed = {};
			RegisterNode[] firstDef = { x };
			Instruction firstIns = new Instruction (instructionBuilder, 
				firstUsed, firstDef);
			RegisterNode[] secondUsed = {};
			RegisterNode[] secondDef = { y };
			Instruction secondIns = new Instruction (instructionBuilder, 
				secondUsed, secondDef);
			RegisterNode[] thirdUsed = {x, y};
			RegisterNode[] thirdDef = {};
			Instruction thirdIns = new Instruction (instructionBuilder, 
				thirdUsed, thirdDef);
			LivenessAnalysis aL = new LivenessAnalysis ();

			Instruction[] instructions = {firstIns, secondIns, thirdIns};
			var result = aL.AnalyzeLiveness (instructions);

			Assert.AreEqual (2, result.Vertices.Count);
			Assert.AreEqual (1, result.Vertices [0].NonCopyNeighbors.Count);

			ICollection<string> temp = new HashSet<string>();
			if (result.Vertices [0].Register.Id.CompareTo ("x") == 0)
				temp.Add ("y");
			else
				temp.Add ("x");
			foreach(var v in result.Vertices[0].NonCopyNeighbors)
					Assert.IsTrue(temp.Contains(v.Register.Id));

			temp.Clear ();

			if (result.Vertices [1].Register.Id.CompareTo ("x") == 0)
				temp.Add ("y");
			else
				temp.Add ("x");
			foreach(var v in result.Vertices[1].NonCopyNeighbors)
				Assert.IsTrue(temp.Contains(v.Register.Id));


		}

		[Test]
		public void SimpleTest2 ()
		{
			RegisterNode[] firstUsed = {};
			RegisterNode[] firstDef = { x, y };
			Instruction firstIns = new Instruction (instructionBuilder, 
				firstUsed, firstDef);
			RegisterNode[] secondUsed = {x};
			RegisterNode[] secondDef = {};
			Instruction secondIns = new Instruction (instructionBuilder, 
				secondUsed, secondDef);
			LivenessAnalysis aL = new LivenessAnalysis ();

			Instruction[] instructions = {firstIns, secondIns};
			var result = aL.AnalyzeLiveness (instructions);

			Assert.AreEqual (2, result.Vertices.Count);
			Assert.AreEqual (0, result.Vertices [0].NonCopyNeighbors.Count);
			Assert.AreEqual (0, result.Vertices [0].CopyNeighbors.Count);
			Assert.AreEqual (0, result.Vertices [1].NonCopyNeighbors.Count);
			Assert.AreEqual (0, result.Vertices [1].CopyNeighbors.Count);

		}

		[Test]
		public void LabelJumpTest ()
		{
			RegisterNode[] used = {};
			RegisterNode[] def = {};
			Instruction firstIns = Instruction.JumpInstruction (instructionBuilder, used, def,
				"third");
			Instruction secondIns = new Instruction (instructionBuilder, 
				used, def);
			Instruction thirdIns = Instruction.LabelInstruction (instructionBuilder,
				"third");
			LivenessAnalysis aL = new LivenessAnalysis ();

			Instruction[] instructions = {firstIns, secondIns, thirdIns};
			aL.AnalyzeLiveness (instructions);

			Assert.AreEqual (0, firstIns.PrevInstructions.Count);
			Assert.AreEqual (1, secondIns.PrevInstructions.Count);
			Assert.AreEqual (2, thirdIns.PrevInstructions.Count);

		}
	}
}

