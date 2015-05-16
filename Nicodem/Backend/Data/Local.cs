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

        public override LocationNode AccessLocal(Function function, LocationNode stackFrame)
        {
            if (function == _owningFunction)
            {
                return new MemoryNode(new SubOperatorNode(stackFrame, new ConstantNode<long>(_offsetFromStackFrame)));
            }
            else
            {
                return AccessLocal(function.EnclosedIn,function.GetEnclosingFunctionStackFrame());
            }
        }
    }
}