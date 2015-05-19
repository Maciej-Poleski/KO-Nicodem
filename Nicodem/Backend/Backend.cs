using System;
using System.Linq;
using System.Collections.Generic;
using Nicodem.Backend.Cover;
using Nicodem.Backend.Builder;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    public class Backend
    {
        private static int MAX_ALLOC_TRY = 3;

        void FromFunctionsToNasm(IEnumerable<Function> funcList)
        {
            foreach (var f in funcList) {
                f.Body = TraceBuilder.BuildTrace(f.Body.First());
            }

            var instrSelector = new InstructionSelection();
            var livenessAnalyzer = new LivenessAnalysis();
            var regAllocator = new RegisterAllocator(Target.AllHardwareRegisters);

            foreach (var f in funcList) {
                int tryCount = 0;
                while(true){
                    if (tryCount == MAX_ALLOC_TRY) {
                        throw new ArgumentException("Your input is stupid... (or our compiler).");
                    }
                    var fBody = f.GenerateTheWholeBody();
                    var fInstr = instrSelector.SelectInstructions(fBody);
                    var inGraph = livenessAnalyzer.AnalyzeLiveness(fInstr);
                    regAllocator.AllocateRegisters(inGraph);
                    var toCorrect = regAllocator.SpilledRegisters;
                    if (toCorrect.Any()) {
                        // apply changes toCorrect
                    } else {
                        break;
                    }
                    tryCount++;
                }
                // use regAllocator.RegistersColoring
                // produce nasm
            }
        }
    }
}

