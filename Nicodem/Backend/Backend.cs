using System;
using System.Linq;
using System.Collections.Generic;
using Nicodem.Backend.Cover;
using Nicodem.Backend.Builder;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    /// <summary>
    /// This is main back-end class. Use it to go through back-end compilation phase.
    /// </summary>
    public class Backend
    {
        private static int MAX_ALLOC_TRY = 3;

        /// <summary>
        /// Construct trace for each function and then in a loop:
        ///     - generate the whole body of the function
        ///     - select instructions
        ///     - analyze liveness
        ///     - allocate registers.
        /// Loop untill success or MAX_ALLOC_TRY. After success - generate nasm.
        /// </summary>
        /// <returns>sequence of nasm instructions</returns>
        /// <param name="funcList">List of backend functions created by frontend.</param>
        IEnumerable<string> FromFunctionsToNasm(IEnumerable<Function> funcList)
        {
            foreach (var f in funcList) {
                f.Body = TraceBuilder.BuildTrace(f.Body.First());
            }

            var livenessAnalyzer = new LivenessAnalysis();
            var regAllocator = new RegisterAllocator(Target.AllHardwareRegisters);

            var output = new List<string>();

            foreach (var f in funcList) {
                int tryCount = 0;
                IEnumerable<Instruction> fInstr;
                while(true){
                    if (tryCount == MAX_ALLOC_TRY) {
                        throw new ArgumentException("Your input is stupid... (or our compiler).");
                    }
                    var fBody = f.GenerateTheWholeBody();
					fInstr = InstructionSelection.SelectInstructions(fBody);
                    var inGraph = livenessAnalyzer.AnalyzeLiveness(fInstr);
                    regAllocator.AllocateRegisters(inGraph);
                    var toCorrect = regAllocator.SpilledRegisters;
                    if (toCorrect.Any()) {
                        // apply changes
                        f.MoveRegistersToMemory(toCorrect);
                    } else {
                        // success
                        break;
                    }
                    tryCount++;
                }

                // use regAllocator.RegistersColoring to produce nasm
                var regMapping = regAllocator.RegistersColoring;
                foreach (var instr in fInstr) {
                    output.Add(instr.ToString(regMapping));
                }
                output.Add("\n");
            }
            return output;
        }
    }
}

