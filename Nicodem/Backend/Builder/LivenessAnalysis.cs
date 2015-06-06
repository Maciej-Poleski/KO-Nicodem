using System;
using Nicodem.Backend.Cover;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Builder
{
	public class LivenessAnalysis
	{

		public LivenessAnalysis ()
		{
		}

		private ISet<RegisterNode> findRegisters(IEnumerable<Instruction> instructions) {
			ISet<RegisterNode> registers = new HashSet<RegisterNode> ();

			foreach(var instruction in instructions) {

				foreach (var register in instruction.RegistersDefined) {
					registers.Add (register);
				}

				foreach (var register in instruction.RegistersUsed) {
					registers.Add (register);
				}

			}

			return registers;
		}

		private ICollection<Instruction> DFS(Instruction instruction, RegisterNode register, Dictionary<RegisterNode, Vertex> registerToVertex, ICollection<Instruction> visited) {
			visited.Add (instruction);

			if (instruction.RegistersDefined.Contains (register))
				return visited;

			foreach (var reg in instruction.RegistersDefined) {
				if(instruction.IsCopyInstruction) {
					registerToVertex [reg].CopyNeighbors.Add (registerToVertex [register]);
					registerToVertex [register].CopyNeighbors.Add (registerToVertex [reg]);
				} else {
					registerToVertex [reg].NonCopyNeighbors.Add (registerToVertex [register]);
					registerToVertex [register].NonCopyNeighbors.Add (registerToVertex [reg]);
				}
			}

			foreach (var ins in instruction.PrevInstructions) {
				if (!visited.Contains (ins))
					visited = DFS (ins, register, registerToVertex, visited);
			}

			return visited;
		}

		private void AnalyzeRegister(RegisterNode register, Dictionary<RegisterNode, Vertex> registerToVertex, IEnumerable<Instruction> instructions) {
			ISet<Instruction> whereUsed = new HashSet<Instruction> ();

			foreach (var instruction in instructions) {
				if(instruction.RegistersUsed.Contains(register))
						whereUsed.Add (instruction);
			}

			ICollection<Instruction> visited = new HashSet<Instruction> ();

			foreach (var instruction in whereUsed) {
				visited = DFS (instruction, register, registerToVertex, visited);
			}
		}

		void prepareInstructions(IEnumerable<Instruction> instructions) {
			Dictionary<String, Instruction> names = new Dictionary<string, Instruction> ();

			Instruction prev = null;
			foreach (var ins in instructions) {
				if (ins.IsLabel)
					names.Add (ins.Label, ins);

				if (prev != null)
					ins.PrevInstructions.Add (prev);
				prev = ins;

			}

			foreach (var ins in instructions) {
				if (ins.IsJumpInstruction)
					names [ins.Label].PrevInstructions.Add (ins);
			}
		}

		public InterferenceGraph AnalyzeLiveness(IEnumerable<Instruction> instructions) {
			prepareInstructions (instructions);

			ISet<Vertex> vertices = new HashSet<Vertex> ();

			ISet<RegisterNode> registers = findRegisters (instructions);
			Dictionary<RegisterNode, Vertex> registerToVertex = new Dictionary<RegisterNode, Vertex>();

			foreach (var register in registers) {
				var vertex = new Vertex (register);

				vertices.Add (vertex);
				registerToVertex.Add (register, vertex);
			}

			foreach(var register in registers) {
				AnalyzeRegister (register, registerToVertex, instructions);
			}

			List<Vertex> result = new List<Vertex> ();

			foreach (var vertex in registerToVertex.Values)
				result.Add (vertex);

			return new InterferenceGraph (result);
		}
			
	}
}

