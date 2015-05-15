using System;
using System.Collections.Generic;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend.Builder
{
    public class RegisterAllocator
    {
        public RegisterAllocator()
        {
        }

        public bool AllocateRegisters(TemporariesGraph interferenceGraph, int availableRegisters){
            throw new NotImplementedException();
        }

        public IReadOnlyDictionary<Temporary, int> TempsMapping { 
            get {
                throw new NotImplementedException();
            }
        }

        public IReadOnlyList<Temporary> TempsToMemory { 
            get {
                throw new NotImplementedException();
            }
        }
    }
}

