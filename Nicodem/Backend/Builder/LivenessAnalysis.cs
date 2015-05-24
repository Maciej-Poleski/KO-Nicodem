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

		private void DFS(Instruction instruction, RegisterNode register) {
		}

		private void AnalyzeRegister(RegisterNode register, Dictionary<RegisterNode, Vertex> registerToVertex, IEnumerable<Instruction> instructions) {
			ISet<Instruction> whereUsed = new HashSet<Instruction> ();

			foreach (var instruction in instructions) {
				foreach(var tempRegister in instruction.RegistersUsed ) {
					if (register == tempRegister) {
						whereUsed.Add (instruction);
					}
				}
			}

			foreach (var instruction in whereUsed) {
				DFS (instruction, register);
			}
		}

		public InterferenceGraph AnalyzeLiveness(IEnumerable<Instruction> instructions) {
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

			throw new NotImplementedException();
		}
			
	}
}

