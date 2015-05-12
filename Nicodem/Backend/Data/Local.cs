using System;
using Nicodem.Backend.Representation;

namespace Nicodem.Backend
{
    public class Local : Location
    {
        private readonly int _offsetFromStackFrame;
        private readonly Function _owningFunction;

        public Local(Function owningFunction, int offset)
        {
            _owningFunction = owningFunction;
            _offsetFromStackFrame = offset;
        }

        public override LocationNode AccessLocal(Function function)
        {
            if (function != _owningFunction)
                throw new NotImplementedException();
            return new MemoryNode(new SubOperatorNode(Target.RBP, new ConstantNode<long>(_offsetFromStackFrame)));
        }
    }
}